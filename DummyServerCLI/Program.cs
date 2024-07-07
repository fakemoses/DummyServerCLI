using DummyServerCLI.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
            string filePath = string.Empty;

            //// Parse arguments
            var argDict = ArgumentHelper.ParseArguments(args);

            if (argDict.ContainsKey("-h"))
            {
                ArgumentHelper.ShowHelp();
            }

            if (argDict.ContainsKey("-c"))
            {
                if (argDict["-c"].ToLower() == "tcp")
                {
                    connType = ConnectionType.Tcp;
                }
                else
                {
                    connType = ConnectionType.Serial;
                }
            }

            if (argDict.ContainsKey("-ip"))
            {
                ip = argDict["-ip"];
            }

            if (argDict.ContainsKey("-p") && int.TryParse(argDict["-p"], out int parsedPort))
            {
                port = parsedPort;
            }

            if (argDict.ContainsKey("-f"))
            {
                filePath = argDict["-f"];
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    commands = new List<string>(File.ReadAllLines(filePath));
                }
            }
            else
            {
                Console.WriteLine("Running on TCP connection with default setting..");
            }

            //// Start connection
            connManager.StartConnection(connType, ip, comPort, baudRate, port, commands);
            Thread.Sleep(1000);

            Console.WriteLine($"Connection created at {ip}:{port}");

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.Q)
                {
                    connManager.StopConnection(connType);
                    break;
                }
                else if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.R)
                {
                    connManager.StopConnection(connType);
                    break; //// Break connection for now
                }
            }
        }
    }

}
