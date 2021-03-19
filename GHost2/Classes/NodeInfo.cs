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

namespace MajorBBS.GHost
{
    public class NodeInfo
    {
        public TcpConnection Connection { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public DoorInfo Door { get; set; }
        public int Node { get; set; }
        public int SecondsThisSession { get; set; }
        public TerminalType TerminalType { get; set; }
        public DateTime TimeOn { get; set; }
        public UserInfo User { get; set; }
        public bool UserLoggedOn { get; set; }

        public NodeInfo()
        {
            Connection = null;
            ConnectionType = ConnectionType.None;
            Door = new DoorInfo("");
            Node = -1;
            SecondsThisSession = 300; // Default to 5 minutes during authentication, will be set accordingly at successful logon
            TerminalType = TerminalType.ANSI;
            TimeOn = DateTime.Now;
            User = new UserInfo("");
            UserLoggedOn = false;
        }

        public int MinutesLeft
        {
            get
            {
                return SecondsLeft / 60;
            }
        }

        public int ReadTimeout
        {
            get
            {
                return Math.Min(5 * 60, SecondsLeft) * 1000;
            }
        }

        public int SecondsLeft
        {
            get
            {
                return SecondsThisSession - (int)DateTime.Now.Subtract(TimeOn).TotalSeconds;
            }
        }

    }
}
