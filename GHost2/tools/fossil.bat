@echo off

if "%comspec%" == "Z:\COMMAND.COM" goto DOSBox
goto NetFoss

:DOSBox
rem DOSBox
rem Loading ADF for COM1
G:\tools\dos\adf COM1 3F8  4  38400  8192  8192  8
goto end

rem NTVDM
:NetFoss
G:\tools\win\nf125-rc\nf.bat %1 %2 %3 %4 %5 %6 %7 %8 %9
goto end

:end