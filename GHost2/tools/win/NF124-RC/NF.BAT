@echo off
g:\tools\nf124-rc\ansi.com 
g:\tools\nf124-rc\netfoss.com %1
rem ** If running a DOS based BBS, add " %1" to the end of above line **
if errorlevel 1 goto end
g:\tools\nf124-rc\netcom.exe %1 %2 %3 %4 %5 %6 %7 %8 %9
g:\tools\nf124-rc\netfoss.com /u
:end
