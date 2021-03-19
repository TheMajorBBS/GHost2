#!/bin/bash
# Stop script for GHost/2 start.sh script
file="/var/run/ghost/start_sh.pid"
# Check the effective user id to see if it's root (EUID works with sudo, UID does not)
if ! [ $(id -u) = 0 ]; then
   echo "######## ########  ########   #######  ########"
   echo "##       ##     ## ##     ## ##     ## ##     ##"
   echo "##       ##     ## ##     ## ##     ## ##     ##"
   echo "######   ########  ########  ##     ## ########"
   echo "##       ##   ##   ##   ##   ##     ## ##   ##"
   echo "##       ##    ##  ##    ##  ##     ## ##    ##"
   echo "######## ##     ## ##     ##  #######  ##     ##"
   echo ""
   echo ""
   echo "####### ERROR: ROOT PRIVILEGES REQUIRED #########"
   echo "This script must be run as root to work properly!"
   echo "You could also try running 'sudo start.sh' too."
   echo "##################################################"
   echo ""
   exit 1
fi

# Find the PID we created on startup and placed in /var/run/ghost/start_sh.pid 
if [ -e $file ];
then
   # Writes the file GHostConsole.stop to the /ghost Directory
   # GHostConsole.exe checks every 2 sec for the file and if it exist
   # Gracefully shuts down the process
   echo $$ >> /ghost/ghostconsole.stop
   # start_sh.pid is then removed cleaning things up for next time
   rm $file
# If the start_sh.pid does not exist then process is not running
else echo "Process is NOT running."
fi
