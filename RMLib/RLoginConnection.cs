/*
  RMLib: Nonvisual support classes used by multiple R&M Software programs
  Copyright (C) Rick Parrish, R&M Software

  This file is part of RMLib.

  RMLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU Lesser General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  any later version.

  RMLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public License
  along with RMLib.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Net.Sockets;

namespace RandM.RMLib
{
    public class RLoginConnection : TcpConnection
    {
        // RLogin commands
        public const byte RLC_COOKIE = 0xFF; // RLogin "cookie" character
        public const byte RLC_S = 0x73; // 0x73="s" Screen size report character
        public const byte RLC_FLAG = 0x00;

        // RLogin states
        const Int16 RLS_HEADER = -1;
        const byte RLS_DATA = 0x00; // In Data mode
        const byte RLS_COOKIE1 = 1;    // Received first "cookie" character
        const byte RLS_COOKIE2 = 2;    // Received second "cookie" character
        const byte RLS_S1 = 3;    // Received first "s" character
        const byte RLS_SS = 5;    // Received second "s" character.  Transmitting screensize info
        const byte RLS_SESSION = 6;

        private bool _ExpectHeader = true;
        private Int16 _RLoginSSBytes = 0;
        private Int16 _RLoginState = RLS_DATA;
        private string _rloginSessionData = "";

        public RLoginConnection() : this(true) { }

        public RLoginConnection(bool expectHeader)
            : base()
        {
            _ExpectHeader = expectHeader;
        }

        public string ClientUserName { get; private set; }

        // NB: The base class constructor calls InitSocket(), which means this method will run before this classes constructor, so
        //     everything accessed here needs to be initialized already (ie can't rely on the constructor to initialize it)
        protected override void InitSocket()
        {
            base.InitSocket();

            LineEnding = "\r";
            _RLoginSSBytes = 0;
        }

        protected override void NegotiateInbound(byte[] data, int numberOfBytes)
        {
            for (int i = 0; i < numberOfBytes; i++)
            {
                switch (_RLoginState)
                {
                    case RLS_HEADER:
                        AddToInputQueue(data[i]);
                        break;
                    case RLS_DATA:
                        if (RLC_COOKIE == data[i])
                        {
                            _RLoginState = RLS_COOKIE1;
                        }
                        else if (RLC_FLAG == data[i])
                        {
                            _RLoginState = RLS_SESSION;
                        }
                        else
                        {
                            AddToInputQueue(data[i]);
                        }
                        break;
                    case RLS_COOKIE1:
                        if (RLC_COOKIE == data[i])
                        {
                            _RLoginState = RLS_COOKIE2;
                        }
                        else
                        {
                            _RLoginState = RLS_DATA;
                        }
                        break;
                    case RLS_COOKIE2:
                        if (RLC_S == data[i])
                        {
                            _RLoginState = RLS_S1;
                        }
                        else
                        {
                            _RLoginState = RLS_DATA;
                        }
                        break;
                    case RLS_S1:
                        if (RLC_S == data[i])
                        {
                            _RLoginState = RLS_SS;
                        }
                        else
                        {
                            _RLoginState = RLS_DATA;
                        }
                        break;
                    case RLS_SS:
                        _RLoginSSBytes += 1;
                        if (_RLoginSSBytes >= 8)
                        {
                            _RLoginSSBytes = 0;
                            _RLoginState = RLS_DATA;
                        }
                        break;
                    case RLS_SESSION:
                        if (RLC_FLAG != data[i])
                        {
                            _rloginSessionData += Convert.ToChar(data[i]);
                        }
                        else
                        {
                            _rloginSessionData = "";
                            _RLoginState = RLS_DATA;
                        }
                        break;
                    default:
                        //_RLoginState = RLS_DATA;
                        break;
                }
            }
        }

        public override bool Open(Socket socket)
        {
            if (base.Open(socket))
            {
                if (_ExpectHeader)
                {
                    _RLoginState = RLS_HEADER;
                    return ParseHeader();
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        //private bool ParseHeader()
        //{
        //    // Get the 4 null terminated strings
        //    string NullByte = ReadLn("\0", 5000);
        //    if (ReadTimedOut) return false;

        //    ClientUserName = ReadLn("\0", 1000);
        //    if (ReadTimedOut) return false;

        //    ServerUserName = ReadLn("\0", 1000);
        //    if (ReadTimedOut) return false;

        //    TerminalType = ReadLn("\0", 1000);
        //    // TODOX SyncTerm doesn't always send final \0, so we'll ignore a read timeout for now
        //    // if (ReadTimedOut) return false;

        //    Write("\0");

        //    return true;
        //}

        private bool ParseHeader()
        {
            const byte END_OF_PHASE = 6;
            const UInt16 timeOut = 5000;
            byte headPhase = 0;
            DateTime StartTime = DateTime.Now;

            while ((headPhase < END_OF_PHASE) && (DateTime.Now.Subtract(StartTime).TotalMilliseconds < timeOut))
            {
                byte incoming = Convert.ToByte(ReadChar(100));
                switch (headPhase)
                {
                    case 0: // Waiting for the initial header
                        if (0 == incoming) headPhase++;
                        break;
                    case 1:
                        if (0 != incoming)
                        {
                            ClientUserName += Convert.ToChar(incoming);
                        }
                        else
                        {
                            if (ClientUserName != null)
                                headPhase++;
                        }
                        break;
                    case 2:
                        if (0 != incoming)
                        {
                            ServerUserName += Convert.ToChar(incoming);
                        }
                        else
                        {
                            headPhase++;
                        }
                        break;
                    case 3:
                        if (0 != incoming)
                        {
                            TerminalType += Convert.ToChar(incoming);
                        }
                        else
                        {
                            if (TerminalType.Contains("/"))
                            {
                                TerminalType = TerminalType.Split('/')[0];
                            }
                            Write("\0");
                            headPhase = END_OF_PHASE;
                        }
                        break;
                    default:
                        break;
                }

            }

            ReadLn("\0", 100);
            ReadString();
            _RLoginState = RLS_DATA;
            if (headPhase == END_OF_PHASE) return true;

            return false;
        }


        public string ServerUserName { get; private set; }

        public string TerminalType { get; private set; }
    }
}
