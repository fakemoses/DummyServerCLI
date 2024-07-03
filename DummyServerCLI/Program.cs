using DummyServerCLI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DummyServerCLI
{
    public class Program
    {
        static void Main(string[] args)
        {
            //// Init
            ConnectionManager connManager = new ConnectionManager();
            List<string> commands = new List<string>();
            bool restartConnection = false;

            string ip = "127.0.0.1"; 
            int port = 6000;
            string comPort = "COM1";
            int baudRate = 152000;
            ConnectionType connType = ConnectionType.Tcp;

            //// Check args. If none then run on default
            //// TODO: Search for -c -ip -p instead of putting it in order. Later integration of -f will be done to read the file
            if (args.Length >= 1)
            {
                if (args[0] == "tcp")
                {
                    connType = ConnectionType.Tcp;
                }
                else
                {
                    connType = ConnectionType.Serial;
                }
                
            }
            if (args.Length >= 2)
            {
                ip = args[1];
            }
            if (args.Length >= 3)
            {
                int.TryParse(args[2], out port);
            }
            else
            {
                Console.WriteLine("Running on TCP connection with default setting..");
            }
            
            //// start connection on input
            connManager.StartConnection(ConnectionType.Tcp, ip, comPort, baudRate, port, commands);
            Thread.Sleep(1000);

            Console.WriteLine($"Connection created at {ip}:{port}");
            ////Console.WriteLine("Press Ctrl+R to restart server. \r\nPress Ctrl+Q to quit.");

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.Q)
                {
                    connManager.StopConnection(ConnectionType.Tcp);
                    break;
                } else if(key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.R)
                {
                    connManager.StopConnection(ConnectionType.Tcp);
                    break; //// Break connection for now
                }
            }
        }
    }
}
