using CommunicationDummyServer.MVVM.Model;
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

            //// Get Input from user
            Console.WriteLine("To start the server, enter the following informations: ");

            //// start connection on input
            connManager.StartConnection(ConnectionType.Tcp, "127.0.0.1", "COM1", 152000, 6000, commands);

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
                }
            }

            //// stop connection on input
            //// proper disposal required. In case the previous program blocks the ip and port
        }
    }
}
