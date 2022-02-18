@echo off
echo publishing started %date% %time% >> publish-all.log
REM call clean-directories.bat >> publish-all.log
call publish-api.bat dll >> publish-all.log
call publish-caaps-task-runner.bat dll >> publish-all.log
call publish-importer.bat dll >> publish-all.log
call publish-urbanise-importer.bat dll >> publish-all.log
call publish-document-uploader.bat dll >> publish-all.log
call publish-elanor-exporter.bat dll >> publish-all.log
call publish-elders-exporter.bat dll >> publish-all.log
call publish-hallmarc-exporter.bat dll >> publish-all.log
call publish-jll-ifm-india-gst-exporter.bat dll >> publish-all.log
call publish-porirua-exporter.bat dll >> publish-all.log
call publish-think-child-care-exporter.bat dll >> publish-all.log
call publish-urbanise-exporter.bat dll >> publish-all.log
echo publishing completed %date% %time% >> publish-all.log
