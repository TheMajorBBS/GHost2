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
using System.Collections.Generic;

namespace MajorBBS.GHost
{
    static class ServerThreadManager
    {
        private static Dictionary<int, ServerThread> _ServerThreads = new Dictionary<int, ServerThread>();

        public static void PauseThreads()
        {
            foreach (KeyValuePair<int, ServerThread> KV in _ServerThreads)
            {
                KV.Value.Pause();
            }
        }

        public static void ResumeThreads()
        {
            foreach (KeyValuePair<int, ServerThread> KV in _ServerThreads)
            {
                KV.Value.Resume();
            }
        }

        public static void StartThreads()
        {
            if ((Config.Instance.RLoginServerPort > 0))
            {
                RMLog.Info("Starting Server Threads");

                try
                {
                    _ServerThreads.Clear();

                    if (Config.Instance.RLoginServerPort > 0)
                    {
                        // Create Server Thread and add to collection
                        _ServerThreads.Add(Config.Instance.RLoginServerPort, new RLoginServerThread());
                    }

                    // Now actually start the server threads
                    foreach (var KVP in _ServerThreads)
                    {
                        KVP.Value.Start();
                    }
                }
                catch (Exception ex)
                {
                    RMLog.Exception(ex, "Error in ServerThreadManager::StartThreads()");
                }
            }
            else
            {
                RMLog.Error("Must specify a port for RLogin servers");
            }
        }

        public static void StopThreads()
        {
            RMLog.Info("Stopping Server Threads");

            try
            {
                foreach (KeyValuePair<int, ServerThread> KV in _ServerThreads)
                {
                    KV.Value.Stop();
                }
                _ServerThreads.Clear();
            }
            catch (Exception ex)
            {
                RMLog.Exception(ex, "Error in ServerThreadManager::StopThreads()");
            }
        }
    }
}
