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
using System;

namespace MajorBBS.GHost
{
    public class NodeEventArgs : EventArgs
    {
        public NodeInfo NodeInfo { get; private set; }
        public string Status { get; private set; }
        public NodeEventType EventType { get; private set; }

        public NodeEventArgs(NodeInfo nodeInfo, string status, NodeEventType eventType)
        {
            NodeInfo = nodeInfo;
            Status = status;
            EventType = eventType;
        }
    }
}
