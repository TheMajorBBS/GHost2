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

namespace MajorBBS.GHost
{
    public class GHost : IDisposable
    {
        private LogHandler _LogHandler = null;
        private GHostStatus _Status = GHostStatus.Stopped;

        public event EventHandler<StatusEventArgs> StatusChangeEvent = null;

        public GHost()
        {
            _LogHandler = new LogHandler(Config.Instance.TimeFormatLog);
        }

        public void Pause()
        {
            if (_Status == GHostStatus.Paused)
            {
                UpdateStatus(GHostStatus.Resuming);
                ServerThreadManager.ResumeThreads();
                UpdateStatus(GHostStatus.Started);
            }
            else if (_Status == GHostStatus.Started)
            {
                UpdateStatus(GHostStatus.Pausing);
                ServerThreadManager.PauseThreads();
                UpdateStatus(GHostStatus.Paused);
            }
        }

        public void Start()
        {
            if (_Status == GHostStatus.Paused)
            {
                // If we're paused, call Pause() again to un-pause
                Pause();
            }
            else if (_Status == GHostStatus.Stopped)
            {
                // Clean up the files not needed by this platform
                Helpers.CleanUpFiles();

                // Check for 3rd party software
                Helpers.CheckFor3rdPartySoftware();

                // Load the Global settings
                Config.Instance.Init();

                // Start the node manager
                NodeManager.Start();

                // Start the server threads
                ServerThreadManager.StartThreads();

                // Start the ignored ips thread
                IgnoredIPsThread.StartThread();

                // Start the timed events thread
                TimedEventsThread.StartThread();

                // Drop root, if necessary
                try
                {
                    Helpers.DropRoot(Config.Instance.UnixUser);
                }
                catch (ArgumentOutOfRangeException aoorex)
                {
                    RMLog.Exception(aoorex, "Unable to drop from root to '" + Config.Instance.UnixUser + "'");

                    // Undo previous actions on error
                    TimedEventsThread.StopThread();
                    IgnoredIPsThread.StopThread();
                    ServerThreadManager.StopThreads();
                    NodeManager.Stop();

                    // If we get here, we failed to go online
                    UpdateStatus(GHostStatus.Stopped);
                    return;
                }

                // If we get here, we're online
                UpdateStatus(GHostStatus.Started);
            }
        }

        public GHostStatus Status
        {
            get { return _Status; }
        }

        // TODOX I don't really like this shutdown parameter, or the Offline vs Stopped states.  Need to make that more clear
        public void Stop()
        {
            if ((_Status == GHostStatus.Paused) || (_Status == GHostStatus.Started))
            {
                UpdateStatus(GHostStatus.Stopping);

                TimedEventsThread.StopThread();
                IgnoredIPsThread.StopThread();
                ServerThreadManager.StopThreads();
                NodeManager.Stop();

                UpdateStatus(GHostStatus.Stopped);
            }
        }

        private void UpdateStatus(GHostStatus newStatus)
        {
            // Record the new status
            _Status = newStatus;

            StatusChangeEvent?.Invoke(this, new StatusEventArgs(newStatus));

            switch (newStatus)
            {
                case GHostStatus.Paused:
                    RMLog.Info("Server(s) are paused");
                    break;
                case GHostStatus.Pausing:
                    RMLog.Info("Server(s) are pausing...");
                    break;
                case GHostStatus.Resuming:
                    RMLog.Info("Server(s) are resuming...");
                    break;
                case GHostStatus.Started:
                    RMLog.Info("Server(s) have started");
                    break;
                case GHostStatus.Starting:
                    RMLog.Info("Server(s) are starting...");
                    break;
                case GHostStatus.Stopped:
                    RMLog.Info("Server(s) have stopped");
                    break;
                case GHostStatus.Stopping:
                    RMLog.Info("Server(s) are stopping...");
                    break;
            }
        }

        public static string Version
        {
            get { return ProcessUtils.ProductVersionOfCallingAssembly; }
        }

        #region IDisposable Support
        private bool _Disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    TimedEventsThread.StopThread();
                    IgnoredIPsThread.StopThread();
                    ServerThreadManager.StopThreads();
                    if (_LogHandler != null)
                    {
                        _LogHandler.Dispose();
                        _LogHandler = null;
                    }
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                _Disposed = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~GHost() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // uncomment the following line if the finalizer is overridden above.
            // or, if code analysis gives a CA1816
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
