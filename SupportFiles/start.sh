#!/bin/bash

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
   echo "You could also try running 'sudo ./start.sh' too."
   echo "##################################################"
   echo ""
   exit 1
fi

# Create GHost directory in /var/run, if necessary
if [ ! -d /var/run/ghost ]; then
  # Makes GHost directory in /var/run to place our pid file
  mkdir -p /var/run/ghost
fi

# Creates a PID file to keep track of what the process ID is
echo $$ >> /var/run/ghost/start_sh.pid

# for DOSEMU -- may be able to comment out in newer releases
sysctl -w vm.mmap_min_addr="0"

# Runs our application as user and group GHost using mono
cd /ghost
privbind -u ghost -g ghost mono GHostConsole.exe DEBUG
