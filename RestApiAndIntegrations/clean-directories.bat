echo off
echo Cleaning client project folders
call clean-dir "Clients\ELANOR\ErpPaymentRequest.Elanor"
call clean-dir "Clients\ELDERS\ErpPaymentRequest.Elders"
call clean-dir "Clients\HALLMARC\ErpPaymentRequest.Hallmarc"
call clean-dir "Clients\JLLGST\ErpVendorPortalUpload.JLLGST"
call clean-dir "Clients\PORIRUA\ErpPaymentRequest.Porirua"
call clean-dir "Clients\THINK\ErpPaymentRequest.ThinkChildCare"
call clean-dir "Clients\URBANISE\ErpPaymentRequest.Urbanise"
call clean-dir "Clients\URBANISE\Importer.Urbanise"
call clean-dir "Clients\Shared\ErpPaymentRequest.Shared"
echo Cleaning core project folders
call clean-dir "API"
call clean-dir "CAAPSTaskWrapper"
call clean-dir "DocumentUploader"
call clean-dir "Common"
call clean-dir "EmailServices"
call clean-dir "Entities"
call clean-dir "Importer"
call clean-dir "Importer.Common"
call clean-dir "Importer.Logic"
call clean-dir "Repository"
call clean-dir "Services"
echo Cleanup completed
