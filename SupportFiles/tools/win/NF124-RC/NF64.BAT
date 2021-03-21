@echo off
rem do not load ANSI.COM before NetFoss64! 
c:\netfoss\netfos64.com
rem ** If running a DOS based BBS, add " %1" to the end of above line **
if errorlevel 1 goto end
c:\netfoss\netcom.exe %1 %2 %3 %4 %5 %6 %7 %8 %9
c:\netfoss\netfos64.com /u
:end
