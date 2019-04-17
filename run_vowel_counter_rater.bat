set file=config.txt
for /f "tokens=1,2 delims=:" %%a in (%file%) do (
    for /l %%i in (1, 1, %%b) do start "%%a" /d %%a dotnet %%a.dll
)