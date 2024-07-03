using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyServerCLI.Model
{
    enum ConnectionStatus
    {
        Disconnected,
        Connected
    }

    /// <summary>
    /// Type of connections/protocols that can be used
    /// </summary>
    public enum ConnectionType
    {
        Tcp,
        Serial
    }

    class ConnectionManager
    {
        private TcpServer tcpServer;
        private SerialServer serialServer;

        public ConnectionManager()
        {
            // Initialize the connection manager
        }

        public async void StartConnection(ConnectionType connectionType, string inputIPAddr, string portName, int baudRate, int port, List<string> commands)
        {
            switch (connectionType)
            {
                case ConnectionType.Tcp:
                    tcpServer = new TcpServer(inputIPAddr, port);
                    await tcpServer.Start(commands);
                    break;
                case ConnectionType.Serial:
                    serialServer = new SerialServer(portName, baudRate);
                    serialServer.Start(commands);
                    break;
                default:
                    break;
            }
        }

        public async void StopConnection(ConnectionType connectionType)
        {
            switch (connectionType)
            {
                case ConnectionType.Tcp:
                    tcpServer.Stop();
                    break;
                case ConnectionType.Serial:
                    serialServer.Stop();
                    break;
                default:
                    break;
            }
        }

        public string GetCurrentIP()
        {
            return tcpServer.GetcurrentIP;
        }
    }
}
