using DummyServerCLI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            //// Make it support from exe args 

            string ip = "127.0.0.1"; 
            int port = 6000;
            string comPort = "COM1";
            int baudRate = 152000;

            if (args.Count() <  2)
            {
                //// Get Input from user
                Console.WriteLine("To start the server, enter the following informations: ");

                Console.WriteLine("");

                GetUserInput(out ip, out port);
                GetUserInput(out comPort, out baudRate);
            }
            else if (args.Count() < 4)
            {
                //// 4 args - All infos
            } else
            {
                //// Incomplete args - complain
            }

            //// start connection on input
            connManager.StartConnection(ConnectionType.Tcp, ip, comPort, baudRate, port, commands);

            Console.WriteLine("Connection created at 127.0.0.1:6000");
            Console.WriteLine("Press Ctrl+R to restart server. \r\nPress Ctrl+Q to quit.");

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

        private static void GetUserInput(out string IP, out int port)
        {
            Console.WriteLine("Input IP/COM Number. For IP it is best to leave it depending on the computer");
            IP = Console.ReadLine();

            Console.WriteLine("Input Port/BaudRate Number");
            port = int.Parse(Console.ReadLine());
        }
    }
}
