@echo off
g:\tools\win\NetFoss\ansi.com 
g:\tools\win\NetFoss\netfoss.com %1
rem ** If running a DOS based BBS, add " %1" to the end of above line **
if errorlevel 1 goto end
g:\tools\win\NetFoss\netcom.exe %1 %2 %3 %4 %5 %6 %7 %8 %9
g:\tools\win\NetFoss\netfoss.com /u
:end
