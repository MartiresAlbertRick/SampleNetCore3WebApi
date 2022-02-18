using AD.CAAPS.Common;
using AD.CAAPS.Entities;
using AD.CAAPS.Importer.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AD.CAAPS.Importer.Logic
{
    public class LoadDataResponse<T>
    {
        public bool Success { get; set; }
        public List<T> Data { get; } = new List<T>();
    }

    public class ImportService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string[] validImportTypes;
        private readonly ImportObjectType importObjectType;
        private readonly string importFilePath;

        public IImportBuilder ImportAppConfiguration{ get; set; }

        public ImportService(IImportBuilder builder, ImportObjectType importObjectType, string importFilePath = null)
        {
            ImportAppConfiguration = builder ?? throw new ArgumentNullException(nameof(builder));
            validImportTypes = ImportAppConfiguration.ImportTypes.Keys.ToArray();
            this.importObjectType = importObjectType;
            this.importFilePath = importFilePath;
        }

        public async Task<ExitCode> Start()
        {
            int processedImport = 0, successfulImport = 0, failedImport = 0;
            Uri origApiUriPlaceHolder = ImportAppConfiguration.ApiUrl ?? throw new ConfigurationException($"No value set for \"{ImportAppConfiguration.ApiUrl}\"");
            var filteredImportTypes = new Dictionary<string, ImportType>();
            if (!importObjectType.Equals(ImportObjectType.Unknown))
            {
                // this condition will set the filteredImportTypes specific for importObjectType set in the argument in case it is defined (not unknown)
                string importObjectTypeAsString = importObjectType.ToString();
                if (ImportAppConfiguration.ImportTypes.TryGetValue(importObjectTypeAsString, out ImportType importType))
                {
                    if (!string.IsNullOrWhiteSpace(importFilePath))
                    {
                        // this condition overrides the existing values for an import type if file importFilePath is specified in the arguments
                        // if there is no filepath is set then the file configurations stays as it is
                        importType.FileNameIsRegex = false;
                        importType.DefaultImportFilePath = Path.GetPathRoot(importFilePath);
                        importType.DefaultImportFileName = Path.GetFileName(importFilePath);
                    }
                    filteredImportTypes.Add(importObjectTypeAsString, importType);
                }
                else
                {
                    throw new ConfigurationException($"No configuration found for import type \"{importObjectType}\"");
                }
            }
            else
            {
                filteredImportTypes = ImportAppConfiguration.ImportTypes;
            }    
            foreach (KeyValuePair<string, ImportType> importType in filteredImportTypes)
            {
                processedImport++;
                ImportAppConfiguration.ApiUrl = new Uri(Utils.EnsureTrailingUrlSeparator(origApiUriPlaceHolder) + importType.Value.Route);
                List<string> filenames = FileReader.RetrieveImportFilesFromFolder(importType.Value.DefaultImportFilePath,
                                                                                  importType.Value.FileNameIsRegex,
                                                                                  importType.Value.DefaultImportFileName,
                                                                                  importType.Value.MatchCurrentDateAndFileCreationDate,
                                                                                  importType.Value.MatchCurrentDateAndFileNameDate,
                                                                                  importType.Value.GetTopOneFileByCreationDate,
                                                                                  importType.Value.FileNameDateTimePattern);
                int processedFiles = 0, successfulFiles = 0, failedFiles = 0;
                ClearTargetTableSetting clearTargetTableSetting = importType.Value.ClearTargetTableSetting;
                foreach (string filename in filenames)
                {
                    processedFiles++;
                    logger.Debug($"Attempting to import file {filename}");
                    try
                    {
                        await ReadImportType((ImportObjectType)Enum.Parse(typeof(ImportObjectType), importType.Key),
                                             filename,
                                             importType.Value.CaapsApiModelDbFieldsMapping,
                                             importType.Value.FieldDelimiter,
                                             clearTargetTableSetting,
                                             importType.Value.Culture).ConfigureAwait(false);
                        logger.Info($"Successfully imported file {filename}");
                        await ActionsAfterImport(filename, importType.Value.TargetFolderAfterImport, importType.Value.ActionAfterImport).ConfigureAwait(false);
                        successfulFiles++;
                        //setting this value to ensure no more truncation happen on succeeding import files for the current import type
                        clearTargetTableSetting = ClearTargetTableSetting.None;
                    }
                    catch (Exception e)
                    {
                        logger.Error(e, $"An exception caught while importing a file {filename}.");
                        failedFiles++;
                    }
                }
                logger.Info($"Import processing completed for {importType.Key}. Processed Files: {processedFiles} Successful Files: {successfulFiles} Failed Files:{failedFiles}");
                if (successfulFiles != 0 && successfulFiles == processedFiles)
                {
                    successfulImport++;
                }
                else
                {
                    failedImport++;
                }
            }

            logger.Info($"Import processing completed. Processed Import Types: {processedImport} Successful Import Types: {successfulImport} Failed Import Types:{failedImport}");
            if (successfulImport == processedImport)
            {
                return ExitCode.SuccessfulNoError;
            }
            else
            {
                return ExitCode.CompletedWithFailedImport;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
        public async Task BulkImport<T>(List<T> dataset, ClearTargetTableSetting clearTargetTableSetting)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Utils.CheckListLengthIsZeroThrowException(dataset, () => new InvalidDataException($"Dataset is empty, please check the source file if it contains data."));
            logger.Info($"Data loaded - {dataset.Count:N0} record(s) read in {stopwatch.ElapsedMilliseconds:N0}ms"); // null checking happens on Utils.CheckListLengthIsZeroThrowException
            var bulkImporter = new CAAPSApiBulkDataImport<T>();
            bulkImporter.ConfigureBulkImport(ImportAppConfiguration);
            await bulkImporter.Start(dataset, clearTargetTableSetting);
            logger.Info($"Data uploaded - {dataset.Count:N0} record(s) posted to CAAPS API in {stopwatch.ElapsedMilliseconds:N0}ms");
        }

        public static async Task ActionsAfterImport(string sourceFile, string targetFolder, ActionAfterImport action)
        {
            if (action == ActionAfterImport.Move)
            {
                string fileName = Path.GetFileName(sourceFile);
                string destinationFile = Path.Combine(targetFolder, fileName);
                await FileUtils.MoveFile(sourceFile, destinationFile);
            }
            else if (action == ActionAfterImport.Delete)
            {
                await FileUtils.DeleteFile(sourceFile).ConfigureAwait(false);
            }
        }

        public async Task ReadImportType(ImportObjectType importType, 
                             string filename, 
                             Dictionary<string, string> modelDbFieldMapping,
                             string fieldDelimiter,
                             ClearTargetTableSetting clearTargetTableSetting,
                             string culture
                             )
        {
            switch (importType)
            {
                case ImportObjectType.Vendor:
                    await ReadFileAsync<Vendor>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture);
                    break;
                case ImportObjectType.GoodsReceipt: 
                    await ReadFileAsync<GoodsReceipt>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture); 
                    break;
                case ImportObjectType.PurchaseOrder: 
                    await ReadFileAsync<PurchaseOrder>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture); 
                    break;
                case ImportObjectType.ImportConfirmation:
                    await ReadFileAsync<ImportConfirmation>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture);
                    break;
                case ImportObjectType.Payment:
                    await ReadFileAsync<Payment>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture);
                    break;
                case ImportObjectType.Entity:
                    await ReadFileAsync<Entity>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture);
                    break;
                case ImportObjectType.ValidAdditionalCharges:
                    await ReadFileAsync<ValidAdditionalCharges>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture);
                    break;
                case ImportObjectType.GLCodeDetails: 
                    await ReadFileAsync<GLCodeDetails>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture); 
                    break;
                case ImportObjectType.ClosedPurchaseOrder: 
                    await ReadFileAsync<ClosedPurchaseOrder>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture); 
                    break;
                case ImportObjectType.NonPoVendor: 
                    await ReadFileAsync<NonPoVendor>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture); 
                    break;
                case ImportObjectType.PaymentTerms: 
                    await ReadFileAsync<PaymentTerms>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture); 
                    break;
                case ImportObjectType.Product: 
                    await ReadFileAsync<Product>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture); 
                    break;
                case ImportObjectType.RoutingCodes: 
                    await ReadFileAsync<RoutingCodes>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture); 
                    break;
                case ImportObjectType.TaxCodeDetails: 
                    await ReadFileAsync<TaxCodeDetails>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture); 
                    break;
                case ImportObjectType.UnitOfMeasure: 
                    await ReadFileAsync<UnitOfMeasure>(filename, modelDbFieldMapping, fieldDelimiter, clearTargetTableSetting, culture); 
                    break;
                default:
                    throw new NotImplementedException($"Invalid import type \"{importType}\". The client date import for this import type has not been implemented. Valid import types are {String.Join(", ", validImportTypes)}.");
            }
        }

        public async Task ReadFileAsync<T>(string filename,
                             Dictionary<string, string> modelDbFieldMapping,
                             string fieldDelimiter,
                             ClearTargetTableSetting clearTargetTableSetting,
                             string culture
                             )
        {
            var response = FileReader.IfCsvThenRead<T>(filename, modelDbFieldMapping, fieldDelimiter, culture);

            if (response.Success)
            {
                await BulkImport(response.Data, clearTargetTableSetting).ConfigureAwait(false);
            }
            else
            {
                response = FileReader.IfExcelThenRead<T>(filename, modelDbFieldMapping);
                if (response.Success)
                {
                    await BulkImport(response.Data, clearTargetTableSetting).ConfigureAwait(false);
                }
                else
                {
                    throw new InvalidDataException($"Neither CSV nor EXCEL based import data configured, please check the configuration file.");
                }
            }
        }
    }
}