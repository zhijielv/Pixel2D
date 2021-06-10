tabtoy.exe ^
--mode=v2 ^
--json_out=ErrorCodeLocalization.json ^
--lan=zh_cn ^
ErrorCodeLocalization.xlsx

@IF %ERRORLEVEL% NEQ 0 pause