﻿/*
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
using System.Text;

namespace MajorBBS.GHost
{
    public static class NodeManager
    {
        private static Dictionary<int, ClientThread> _ClientThreads = new Dictionary<int, ClientThread>();
        private static object _ListLock = new Object();

        public static event EventHandler<IntEventArgs> ConnectionCountChangeEvent = null;
        public static event EventHandler<NodeEventArgs> NodeEvent = null;

        private static void ClientThread_FinishEvent(object sender, EventArgs e)
        {
            var FinishedClientThread = sender as ClientThread;

            // Free up the node that the finished thread was using
            bool FoundClientThread = false;
            lock (_ListLock)
            {
                for (int NodeLoop = Config.Instance.FirstNode; NodeLoop <= Config.Instance.LastNode; NodeLoop++)
                {
                    if (_ClientThreads[NodeLoop] == FinishedClientThread)
                    {
                        _ClientThreads[NodeLoop].Dispose();
                        _ClientThreads[NodeLoop] = null;
                        FoundClientThread = true;
                        break;
                    }
                }
            }

            if (FoundClientThread)
            {
                // Raise event and update data files
                UpdateConnectionCount();
                UpdateWhoIsOnlineFile();
            }
        }

        static void ClientThread_NodeEvent(object sender, NodeEventArgs e)
        {
            if (e.EventType == NodeEventType.LogOn)
            {
                // Kill other session if the user is a logged in user (ie not a RUNBBS.INI connection) and the user isn't allowed on multiple nodes
                if ((e.NodeInfo.User.UserId > 0) && (!e.NodeInfo.User.AllowMultipleConnections))
                {
                    KillOtherSession(e.NodeInfo.User.Alias, e.NodeInfo.Node);
                }

            }

            NodeEvent?.Invoke(sender, e);
            UpdateWhoIsOnlineFile();
        }

        static void ClientThread_WhosOnlineEvent(object sender, WhoIsOnlineEventArgs e)
        {
            lock (_ListLock)
            {
                for (int NodeLoop = Config.Instance.FirstNode; NodeLoop <= Config.Instance.LastNode; NodeLoop++)
                {
                    // Make sure this node has a client
                    if (_ClientThreads[NodeLoop] == null)
                    {
                        e.WhoIsOnline.Add("WHOSONLINE_" + NodeLoop.ToString() + "_ALIAS", "");
                        e.WhoIsOnline.Add("WHOSONLINE_" + NodeLoop.ToString() + "_IPADDRESS", "");
                        e.WhoIsOnline.Add("WHOSONLINE_" + NodeLoop.ToString() + "_STATUS", "Waiting for caller");
                    }
                    else
                    {
                        e.WhoIsOnline.Add("WHOSONLINE_" + NodeLoop.ToString() + "_ALIAS", _ClientThreads[NodeLoop].Alias);
                        e.WhoIsOnline.Add("WHOSONLINE_" + NodeLoop.ToString() + "_IPADDRESS", _ClientThreads[NodeLoop].IPAddress);
                        e.WhoIsOnline.Add("WHOSONLINE_" + NodeLoop.ToString() + "_STATUS", _ClientThreads[NodeLoop].Status);
                    }
                }
            }
        }

        public static int ConnectionCount
        {
            get
            {
                int Result = 0;

                lock (_ListLock)
                {
                    // Check for a free node
                    for (int i = Config.Instance.FirstNode; i <= Config.Instance.LastNode; i++)
                    {
                        if (_ClientThreads[i] != null) Result += 1;
                    }
                }

                return Result;
            }
        }

        public static void DisconnectNode(int node)
        {
            bool Raise = false;

            if (IsValidNode(node))
            {
                lock (_ListLock)
                {
                    if (_ClientThreads[node] != null)
                    {
                        _ClientThreads[node].Stop();
                        _ClientThreads[node] = null;
                        Raise = true;
                    }
                }
            }

            if (Raise)
            {
                UpdateConnectionCount();
                UpdateWhoIsOnlineFile();
            }
        }

        private static void DisplayAnsi(string ansi, int node)
        {
            if (IsValidNode(node))
            {
                lock (_ListLock)
                {
                    if (_ClientThreads[node] != null)
                    {
                        _ClientThreads[node].DisplayAnsi(ansi);
                    }
                }
            }
        }

        public static int GetFreeNode(ClientThread clientThread)
        {
            int Result = 0;
            bool Raise = false;

            if (clientThread != null)
            {
                lock (_ListLock)
                {
                    // Check for a free node
                    for (int i = Config.Instance.FirstNode; i <= Config.Instance.LastNode; i++)
                    {
                        if (_ClientThreads[i] == null)
                        {
                            clientThread.FinishEvent += ClientThread_FinishEvent;
                            clientThread.NodeEvent += ClientThread_NodeEvent;
                            clientThread.WhoIsOnlineEvent += ClientThread_WhosOnlineEvent;
                            _ClientThreads[i] = clientThread;

                            Result = i;
                            Raise = true;

                            break;
                        }
                    }
                }
            }

            if (Raise) UpdateConnectionCount();
            return Result;
        }

        private static bool IsValidNode(int node)
        {
            return ((node >= Config.Instance.FirstNode) && (node <= Config.Instance.LastNode));
        }

        public static void KillOtherSession(string alias, int node)
        {
            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentNullException("alias");
            }

            int NodeToKill = 0;

            lock (_ListLock)
            {
                for (int NodeLoop = Config.Instance.FirstNode; NodeLoop <= Config.Instance.LastNode; NodeLoop++)
                {
                    // Make sure we don't kill our own node!
                    if (NodeLoop != node)
                    {
                        // Make sure this node has a client
                        if (_ClientThreads[NodeLoop] != null)
                        {
                            // Make sure this node matches the alias
                            if (_ClientThreads[NodeLoop].Alias.ToUpper() == alias.ToUpper())
                            {
                                NodeToKill = NodeLoop;
                                break;
                            }
                        }
                    }
                }
            }

            if (NodeToKill > 0)
            {
                // Show "you're on too many nodes" message before disconnecting
                DisplayAnsi("LOGON_TWO_NODES", NodeToKill);
                DisconnectNode(NodeToKill);
            }
        }

        public static void Start()
        {
            lock (_ListLock)
            {
                _ClientThreads.Clear();
                for (int Node = Config.Instance.FirstNode; Node <= Config.Instance.LastNode; Node++)
                {
                    _ClientThreads[Node] = null;
                }
            }

            UpdateConnectionCount();
            UpdateWhoIsOnlineFile();
        }

        public static void Stop()
        {
            lock (_ListLock)
            {
                // Shutdown any client threads that are still active
                for (int Node = Config.Instance.FirstNode; Node <= Config.Instance.LastNode; Node++)
                {
                    if (_ClientThreads[Node] != null)
                    {
                        _ClientThreads[Node].Stop();
                        _ClientThreads[Node] = null;
                    }
                }
            }

            UpdateConnectionCount();
            UpdateWhoIsOnlineFile();
        }

        private static void UpdateConnectionCount()
        {
            ConnectionCountChangeEvent?.Invoke(null, new IntEventArgs(ConnectionCount));
        }

        private static void UpdateWhoIsOnlineFile()
        {
            try
            {
                var SB = new StringBuilder();
                SB.AppendLine("Node,RemoteIP,User,Status");
                lock (_ListLock)
                {
                    // Get status from each node
                    for (int Node = Config.Instance.FirstNode; Node <= Config.Instance.LastNode; Node++)
                    {
                        if (_ClientThreads[Node] == null)
                        {
                            SB.AppendLine($"{Node}\t\t\tWaiting for caller");
                        }
                        else
                        {
                            SB.AppendLine($"{Node}\t{_ClientThreads[Node].IPAddress}\t{_ClientThreads[Node].Alias}\t{_ClientThreads[Node].Status}");
                        }
                    }
                }

                string WhoIsOnlineFilename = StringUtils.PathCombine(ProcessUtils.StartupPath, "whoisonline.txt");
                FileUtils.FileWriteAllText(WhoIsOnlineFilename, SB.ToString());
            }
            catch (Exception ex)
            {
                RMLog.Exception(ex, "Unable to update whoisonline.txt");
            }
        }
    }
}
