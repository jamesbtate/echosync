@ECHO OFF

REM The following directory is for .NET 4.0
REM set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
REM set PATH=%PATH%;%DOTNETFX2%

echo Re-Installing EchoBackupService...
echo ---------------------------------------------------
call "uninstall service.bat"
call "install service.bat"
echo ---------------------------------------------------
echo Done reinstall.