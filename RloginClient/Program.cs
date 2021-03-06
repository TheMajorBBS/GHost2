/**
 * RLoginTester
 * Simpler console program for testing rlogin connections.
 */
using RandM.RMLib;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MajorBBS.GHost
{
    class RloginClientSocket
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();


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
            string remoteHostName = "localhost";
            int remotePort = 513;
            string remoteUser = "tester";
            string localUser = "tester";
            string terminal = "vt100";
            int bufferSize = 4096;

            Console.SetWindowSize(80, 25);
            Console.SetBufferSize(80, 25);

            var stdOut = Console.OpenStandardOutput();
            var con = new StreamWriter(stdOut, RMEncoding.Ansi);
            con.AutoFlush = true;
            Console.SetOut(con);
            
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
            {
                Console.WriteLine("failed to get output console mode");
                Console.ReadKey();
                return;
            }

            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if (!SetConsoleMode(iStdOut, outConsoleMode))
            {
                Console.WriteLine($"failed to set output console mode, error code: {GetLastError()}");
                Console.ReadKey();
                return;
            }
            
            Console.OpenStandardInput();

            Console.WriteLine("\u001b[36mRloginClient\u001b[0m");


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

            Console.WriteLine("Host:{0}  Port:{1}", remoteHostName, remotePort);
            Console.WriteLine("Local User:{0}  Remote User:{1}", localUser, remoteUser);
            Console.WriteLine("Terminal:{0}", terminal);

            RLoginConnection rlConn = new RLoginConnection(false);

            if (rlConn.Connect(remoteHostName, remotePort))
            {
                string header = String.Format("\0{0}\0{1}\0{2}\0", localUser, remoteUser, terminal);
                rlConn.Write(header);

                while (rlConn.Connected)
                {
                    bool yield = true;

                    if (rlConn.CanRead(100))
                    {
                        Console.Write(rlConn.ReadString());
                        yield = false;
                    }

                    if (Console.KeyAvailable)
                    {
                        string inKey = String.Format("{0}", Console.ReadKey(true).KeyChar);
                        rlConn.Write(inKey);
                    }

                    // See if we need to yield
                    if (yield) Crt.Delay(1);

                }
            }
            rlConn.Close();
            Console.WriteLine("Connection closed.");
            Console.ReadKey();
        }
    }
}
