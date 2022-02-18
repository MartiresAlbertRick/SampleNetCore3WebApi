using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.Services;
using AD.CAAPS.ErpPaymentRequest.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AD.CAAPS.ErpPaymentRequest.Elders
{
    public class PaymentRequest
    {
        private readonly AppSettings appSettings;
        private readonly PaymentRequestServices paymentRequestServices;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        const string EXPORT_FAILED = "EXPORT FAILED";
        const string XSasTokenKey = "x-sas-token";
        const string EDIDC_40 = "q2:EDI_DC40";
        const string ZMIRO1000 = "q2:ZMIRO01000";
        const string TNS_SEND = "tns:Send";
        const string TNS_IDOCDATA = "tns:idocData",
                     PO_DOC_TYPE_DEFAULT_VALUE = "RE",
                     PO_TAX_CALC_IND_DEFAULT_VALUE = "X",
                     POITEM = "q2:PO_ITEM",
                     POLINENO = "q2:PO_LINE_NO",
                     POTAXCODE = "q2:PO_TAX_CODE",
                     POUNIT = "q2:PO_UNIT",
                     POGRLINEAMT = "q2:PO_GRLINE_AMT",
                     POQTY = "q2:PO_QTY",
                     POREFDOC = "q2:PO_REF_DOC",
                     POREFDOCYR = "q2:PO_REF_DOC_YR",
                     BSB_BANK_ACCOUNT = "q2:BSB_BANK_ACCOUNT",
                     CAAPS_UNIQUE_ID = "q2:CAAPS_UNIQUE_ID",
                     PO_DOC_TYPE = "q2:PO_DOC_TYPE",
                     PO_REF_DOC_NO = "q2:PO_REF_DOC_NO",
                     PO_URL_LINK = "q2:PO_URL_LINK",
                     PO_VENDOR = "q2:PO_VENDOR",
                     PO_NUMBER = "q2:PO_NUMBER",
                     PO_REF_DOC_YR = "q2:PO_REF_DOC_YR",
                     PO_TAX_CALC_IND = "q2:PO_TAX_CALC_IND",
                     PO_CURR = "q2:PO_CURR",
                     PO_GROSS_AMT = "q2:PO_GROSS_AMT",
                     PO_COMP_CODE = "q2:PO_COMP_CODE",
                     PO_POST_DATE = "q2:PO_POST_DATE",
                     PO_DOC_DATE = "q2:PO_DOC_DATE";

        public PaymentRequest(DBConfiguration dbConfiguration, AppSettings appSettings)
        {
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            paymentRequestServices = new PaymentRequestServices(dbConfiguration ?? throw new ArgumentNullException(nameof(dbConfiguration)), appSettings.PaymentRequestOptions);
        }

        public async Task<ExitCode> PaymentRequestExportAsync()
        {
            logger.Debug("Started running PaymentRequestExportAsync()");
            ExitCode exitCode = ExitCode.CompletedButWithUnprocessed;

            IList<PaymentRequestHeader> paymentRequests = await paymentRequestServices.GetPaymentRequests().ConfigureAwait(false);

            JObject xmlProperties = GetXmlProperties();
            JObject ediDc40 = GetEdiDc40Segment();

            foreach (PaymentRequestHeader paymentRequest in paymentRequests)
            {
                var docUpdates = new List<PaymentRequestDocumentUpdates>();

                logger.Debug($"Processing payment request for invoice document {paymentRequest.CaapsRecordId}");

                try
                {
                    var miroPaymentInbound = new JObject {
                        { "?xml", xmlProperties },
                        { TNS_SEND, new JObject {
                                { "@xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance" },
                                { "@xmlns:q1", "http://Microsoft.LobServices.Sap/2007/03/Types/Idoc/Common/" },
                                { "@xmlns:tns", "http://Microsoft.LobServices.Sap/2007/03/Idoc/3/ZMIRO_BT//702/Send" },
                                { "@xmlns:q2", "http://Microsoft.LobServices.Sap/2007/03/Types/Idoc/3/ZMIRO_BT//702" },
                                { "@xsi:schemaLocation","http://Microsoft.LobServices.Sap/2007/03/Idoc/3/ZMIRO_BT//702/Send IDocOperation.ZMIRO_BT.702.3.Send.Send" },
                                { TNS_IDOCDATA,  new JObject {
                                        { EDIDC_40, ediDc40 },
                                        { ZMIRO1000, await BuildZMIRO1000(paymentRequest).ConfigureAwait(false) }
                                    }
                                }
                            }
                        }
                    };

                    RestSharp.RestRequest request = GetRequestForPosting();
                    logger.Debug($"Attempting to send request to {appSettings.DataUploadEndpoint}. JSON: {JsonConvert.SerializeObject(miroPaymentInbound)}");
                    request.AddParameter("application/json", JsonConvert.SerializeObject(miroPaymentInbound), RestSharp.ParameterType.RequestBody);

                    var restUtils = new RestSharpUtils(new Uri(appSettings.DataUploadEndpoint), request);
                    
                    RestSharp.IRestResponse response = await restUtils.SendRequestAsync().ConfigureAwait(false);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        logger.Debug($"Payment request export success for document with id {paymentRequest.CaapsRecordId}");
                        docUpdates.Add(new PaymentRequestDocumentUpdates
                        {
                            ID = paymentRequest.ID,
                            Status = appSettings.DocumentStatusAfterExtract,
                            ExportDate = DateTime.UtcNow,
                            IsImportConfirmationExpected = true,
                            ErpImportDate = DateTime.UtcNow,
                            UserComments = "Payment request export succesful"
                        });
                        if (exitCode == ExitCode.CompletedButWithUnprocessed)
                        {
                            exitCode = ExitCode.Successful;
                        }
                    }
                    else
                    {
                        string responseErrorDetails = RestSharpUtils.GetRestResponseErrorDetails(response);
                        logger.Error(responseErrorDetails);
                        docUpdates.Add(new PaymentRequestDocumentUpdates
                        {
                            ID = paymentRequest.ID,
                            Status = EXPORT_FAILED,
                            ExportDate = DateTime.UtcNow,
                            IsImportConfirmationExpected = false,
                            UserComments = $"Payment request export failed. Response: {responseErrorDetails}"
                        });
                        exitCode = ExitCode.CompletedButWithErrors;
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Payment request {paymentRequest.ID} export failed");
                    docUpdates.Add(new PaymentRequestDocumentUpdates
                    {
                        ID = paymentRequest.ID,
                        Status = EXPORT_FAILED,
                        ExportDate = DateTime.UtcNow,
                        IsImportConfirmationExpected = false,
                        UserComments = $"Payment request {paymentRequest.ID} export failed"
                    });
                }
                await paymentRequestServices.BulkUpdateDocumentStatus(docUpdates).ConfigureAwait(false);
                exitCode = ExitCode.CompletedButWithErrors;
            }

            return exitCode;
        }

        private static JObject GetXmlProperties()
        {
            return new JObject {
                { "@version", "1.0" },
                { "@encoding", "UTF-8" }
            };
        }

        private JObject GetEdiDc40Segment()
        {
            if (appSettings.IDocHeaders.Count == 0)
                throw new ConfigurationException($"Value is required for configuration \"IDocHeaders\"");

            var transformedHeaders = new Dictionary<string, string>();
            // append q1: prefix to headers
            string q1Prefix = "q1:";
            foreach (KeyValuePair<string, string> keyValuePair in appSettings.IDocHeaders)
            {
                transformedHeaders.Add(q1Prefix + keyValuePair.Key, keyValuePair.Value);
            }

            return JObject.FromObject(transformedHeaders);
        }

        private RestSharp.RestRequest GetRequestForPosting()
        {
            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader(XSasTokenKey, appSettings.XSasToken);
            return request;
        }

        private async Task<JArray> BuildZMIRO1000(PaymentRequestHeader paymentRequest)
        {
            var miroArray = new JArray();
            
            if (paymentRequest.PurchaseOrderGoodsReceiptAllocation.Count < 1)
            {
                throw new PaymentRequestModuleException("No purchase orders allocated. Purchase order details is mandatory for export");
            }

            foreach (PurchaseOrder purchaseOrder in paymentRequest.PurchaseOrderGoodsReceiptAllocation)
            {
                if (purchaseOrder.GoodsReceipts.Count < 1)
                {
                    throw new PaymentRequestModuleException("No goods receipts allocated. Goods receipts details is mandatory for export.");
                }

                foreach (GoodsReceipt goodsReceipt in purchaseOrder.GoodsReceipts)
                {
                    var miroObjectHeaderData = new JObject {
                        { PO_DOC_DATE, Utils.ConvertDateTimeToString(paymentRequest.DocDateIssued, appSettings.ExportDateFormat) },
                        { PO_POST_DATE, Utils.ConvertDateTimeToString(DateTime.UtcNow, appSettings.ExportDateFormat) },
                        { PO_COMP_CODE, paymentRequest.EntityCode },
                        { PO_GROSS_AMT, paymentRequest.DocAmountTotal },
                        { PO_CURR, paymentRequest.DocAmountCurrency },
                        { PO_TAX_CALC_IND, PO_TAX_CALC_IND_DEFAULT_VALUE },
                        { PO_NUMBER, paymentRequest.PoNumber },
                        { PO_REF_DOC_YR, goodsReceipt.ReceivedDate.Value.Year },
                        { PO_VENDOR, paymentRequest.VendorCode },
                        { PO_URL_LINK, await paymentRequestServices.GetFileURL(paymentRequest.ID).ConfigureAwait(false) },
                        { PO_REF_DOC_NO, paymentRequest.DocRefNumberA },
                        { PO_DOC_TYPE, PO_DOC_TYPE_DEFAULT_VALUE },
                        { CAAPS_UNIQUE_ID, paymentRequest.CaapsRecordId },
                        { BSB_BANK_ACCOUNT, paymentRequest.VendorBankAccountNumber },
                        { POITEM, purchaseOrder.LineNumber },
                        { POLINENO, purchaseOrder.LineNumber },
                        { POUNIT, purchaseOrder.LineUOM },
                        { POTAXCODE, purchaseOrder.LineTaxCode },
                        { POGRLINEAMT, goodsReceipt.ReceiptedValueTaxEx },
                        { POQTY, goodsReceipt.ReceiptedQty },
                        { POREFDOC, goodsReceipt.GoodsReceivedNumber }
                    };
                    miroArray.Add(miroObjectHeaderData);
                }
            }
            return miroArray;
        }
    }
}