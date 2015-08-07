@echo off
echo copy KepwareClientCOM.dll and jacob dlls to windows\System32

echo Windows Directory %windir%
set DLLDir=%~dp0
echo %DLLDir%
set SYS32=%windir%\System32
set NT4=%windir%\Microsoft.NET\Framework\v4.0.30319

xcopy %DLLDir%KepwareClientCOM.dll %NT4% /s /y
xcopy %DLLDir%KepwareClientCOM.tlb %NT4% /s /y
xcopy %DLLDir%jacob-1.18-M2-x64.dll %NT4% /s /y
xcopy %DLLDir%jacob-1.18-M2-x86.dll %NT4% /s /y
xcopy %DLLDir%Kepware.ClientAce.OpcClient.dll %NT4% /s /y

xcopy %DLLDir%jacob-1.18-M2-x64.dll %SYS32% /s /y
xcopy %DLLDir%jacob-1.18-M2-x86.dll %SYS32% /s /y

echo regasm KepwareClientCOM.dll


%NT4%\regasm.exe /unregister %NT4%\KepwareClientCOM.dll

%NT4%\regasm.exe %NT4%\KepwareClientCOM.dll /codebase

pause