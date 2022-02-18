@echo off
SET FILE_PATH=[SET_FILE_PATH]
SET FILE_NAME=%FILE_PATH%[SET_FILE_NAME]

REM SELECT FROM 1 TO 5
REM 1 - Vendor
REM 2 - GoodsReceipt
REM 3 - PurchaseOrder
REM 4 - ImportConfirmation
REM 5 - Payment
SET IMPORT_TYPE=[SET_IMPORT_TYPE]

.\bin\Release\netcoreapp2.2\win10-x64\publish\Importer Execute --type %IMPORT_TYPE% --path "%FILE_NAME%"
PAUSE