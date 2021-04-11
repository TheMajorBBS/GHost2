/*
  GHost/2: Door Server
  Copyleft 2021 Major BBS (GPL3)
    original: Rick Parrish, R&M Software

  This file is part of GHost/2.

  GHost/2 is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  any later version.

  GHost/2 is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with GHost/2.  If not, see <http://www.gnu.org/licenses/>.
*/
using RandM.RMLib;
using System;
using System.IO;

namespace MajorBBS.GHost
{
    class IgnoredIPsThread : RMThread
    {
        private static IgnoredIPsThread _IgnoredIPsThread = null;

        protected override void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }

                // free unmanaged resources (unmanaged objects)
                // set large fields to null.

                // Call the base dispose
                base.Dispose(disposing);
            }
        }

        protected override void Execute()
        {
            while (!_Stop)
            {
                string IgnoredIPsFileName = StringUtils.PathCombine(ProcessUtils.StartupPath, "config", "ignored-ips.txt");
                string CombinedFileName = StringUtils.PathCombine(ProcessUtils.StartupPath, "config", "ignored-ips-combined.txt");

                // Combine the lists
                try
                {
                    FileUtils.FileWriteAllText(CombinedFileName, "");
                    if (File.Exists(IgnoredIPsFileName)) FileUtils.FileAppendAllText(CombinedFileName, FileUtils.FileReadAllText(IgnoredIPsFileName));
                }
                catch (Exception ex)
                {
                    RMLog.Exception(ex, "Unable to combine Ignored IPs lists");
                }

                // Wait for one hour before updating again
                if (!_Stop && (_StopEvent != null)) _StopEvent.WaitOne(3600000);
            }
        }

        public static void StartThread()
        {
            RMLog.Info("Starting Ignored IPs Thread");

            try
            {
                // Create Ignored IPs Thread and Thread objects
                _IgnoredIPsThread = new IgnoredIPsThread();
                _IgnoredIPsThread.Start();
            }
            catch (Exception ex)
            {
                RMLog.Exception(ex, "Error in IgnoredIPsThread::StartThread()");
            }
        }

        public static void StopThread()
        {
            RMLog.Info("Stopping Ignored IPs Thread");

            if (_IgnoredIPsThread != null)
            {
                try
                {
                    _IgnoredIPsThread.Stop();
                    _IgnoredIPsThread.Dispose();
                    _IgnoredIPsThread = null;
                }
                catch (Exception ex)
                {
                    RMLog.Exception(ex, "Error in IgnoredIPsThread::StopThread()");
                }
            }
        }
    }
}
