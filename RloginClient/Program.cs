/**
 * RLoginTester
 * Simpler console program for testing rlogin connections.
 */
using System;
using System.Net;
using System.Net.Sockets;

namespace MajorBBS.GHost
{
    class RloginClientSocket
    {
        /// <summary>
        /// This routine repeatedly copies a string message into a byte array until filled.
        /// </summary>
        /// <param name="dataBuffer">Byte buffer to fill with string message</param>
        /// <param name="message">String message to copy</param>
        static public void FormatBuffer(byte[] dataBuffer, string message)
        {
            byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(message);
            int index = 0;

            // First convert the string to bytes and then copy into send buffer
            while (index < dataBuffer.Length)
            {
                for (int j = 0; j < byteMessage.Length; j++)
                {
                    dataBuffer[index] = byteMessage[j];
                    index++;
                    // Make sure we don't go past the send buffer length
                    if (index >= dataBuffer.Length)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Prints simple usage information.
        /// </summary>
        static public void usage()
        {
            Console.WriteLine("usage: RLoginClient [-n server] [-p port] [-r user] [-l user]");
            Console.WriteLine("                       [-t term] [-x size]");
            Console.WriteLine("     -n server       Server name or address to connect/send to. (localhost)");
            Console.WriteLine("     -p port         Port number to connect/send to. (513)");
            Console.WriteLine("     -r user         Remote user name. (tester)");
            Console.WriteLine("     -l user         Local user name. (tester)");
            Console.WriteLine("     -t term         Terminal type. Optional baudrate. (vt100)");
            Console.WriteLine("     -x size         Size of send and receive buffers. (4096)");
            Console.WriteLine(" Else, default values will be used...");
        }

        /// <summary>
        /// Main loop for the rlogin client.  This a very simple client to test rlogin connections on the server.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            SocketType sockType = SocketType.Stream;
            ProtocolType sockProtocol = ProtocolType.Tcp;
            string remoteHostName = "localhost";
            int remotePort = 513;
            string remoteUser = "tester";
            string localUser = "tester";
            string terminal = "vt100";
            int bufferSize = 4096;

            Console.SetWindowSize(80, 25);
            Console.WriteLine("RloginClient");

            sockType = SocketType.Stream;
            sockProtocol = ProtocolType.Tcp;

            // Parse the command line
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if ((args[i][0] == '-') || (args[i][0] == '/'))
                    {
                        switch (Char.ToLower(args[i][1]))
                        {
                            case 'n':       // Destination address to connect to or send to
                                remoteHostName = args[++i];
                                break;

                            case 'p':       // Port number for the destination
                                remotePort = System.Convert.ToInt32(args[++i]);
                                break;

                            case 'r':
                                remoteUser = args[++i];
                                break;

                            case 'l':
                                localUser = args[++i];
                                break;

                            case 't':
                                terminal = args[++i];
                                break;

                            case 'x':       // Size of the send and receive buffers
                                bufferSize = System.Convert.ToInt32(args[++i]);
                                break;

                            default:
                                usage();
                                return;
                        }
                    }
                }
                catch
                {
                    usage();
                    return;
                }
            }

            Socket clientSocket = null;
            IPHostEntry resolvedHost = null;
            IPEndPoint destination = null;
            byte[] sendBuffer = new byte[bufferSize];
            byte[] recvBuffer = new Byte[bufferSize];
            int rc;

            try
            {
                // Try to resolve the remote host name or address
                resolvedHost = Dns.GetHostEntry(remoteHostName);
                Console.WriteLine("Client: GetHostEntry() is OK...");

                // Try each address returned
                foreach (IPAddress addr in resolvedHost.AddressList)
                {
                    // Create a socket corresponding to the address family of the resolved address
                    clientSocket = new Socket(
                        addr.AddressFamily,
                        sockType,
                        sockProtocol
                        );
                    Console.WriteLine("Client: Socket() is OK...");
                    try
                    {
                        // Create the endpoint that describes the destination
                        destination = new IPEndPoint(addr, remotePort);
                        Console.WriteLine("Client: IPEndPoint() is OK. IP Address: {0}, server port: {1}", addr, remotePort);

                        clientSocket.Connect(destination);
                        Console.WriteLine("Client: Connect() is OK...");
                        break;
                    }
                    catch (SocketException)
                    {
                        // Connect failed, so close the socket and try the next address
                        clientSocket.Close();
                        Console.WriteLine("Client: Close() is OK...");
                        clientSocket = null;
                        continue;
                    }
                }

                // Make sure we have a valid socket before trying to use it
                if ((clientSocket != null) && (destination != null))
                {
                    try
                    {
                        var msg = String.Format("\0\0{0}\0{1}\0{2}\0", localUser, remoteUser, terminal);
                        FormatBuffer(sendBuffer, msg);
                      
                        rc = clientSocket.Send(sendBuffer);
                        Console.WriteLine("Client: send() is OK...TCP...");
                        Console.WriteLine("Client: Sent request of {0} bytes", rc);

                        while (true)
                        {
                            if (clientSocket.Available > 0)
                            {
                                rc = clientSocket.Receive(recvBuffer);
                                string strBuf = System.Text.Encoding.ASCII.GetString(recvBuffer, 0, recvBuffer.Length);
                                Console.Write(strBuf);
                            }

                            if (Console.KeyAvailable)
                            {
                                string inKey = String.Format("{0}", Console.ReadKey(true).KeyChar);
                                sendBuffer = System.Text.Encoding.UTF8.GetBytes(inKey);
                                clientSocket.Send(sendBuffer);
                            }

                            // Exit loop if server indicates shutdown
                            if (rc == 0)
                            {
                                clientSocket.Close();
                                Console.WriteLine("Client: Close() is OK...");
                                break;
                            }
                        }
                    }
                    catch (SocketException err)
                    {
                        Console.WriteLine("Client: Error occurred while sending or receiving data.");
                        Console.WriteLine("   Error: {0}", err.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Client: Unable to establish connection to server!");
                }
            }
            catch (SocketException err)
            {
                Console.WriteLine("Client: Socket error occurred: {0}", err.Message);
            }
        }
    }
}
