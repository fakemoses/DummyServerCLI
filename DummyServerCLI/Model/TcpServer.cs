using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationDummyServer.MVVM.Model
{
    public class TcpServer
    {
        private TcpListener _listener;
        private bool _isRunning;
        private CancellationTokenSource _cancellationTokenSource;
        private TcpClient _client;

        public TcpServer(string inputIPAddr, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(inputIPAddr);
            _listener = new TcpListener(ipAddress, port);
            GetcurrentIP = inputIPAddr;
        }

        public async Task Start(List<string> commands)
        {
            try
            {
                _listener.Start();
                _cancellationTokenSource = new CancellationTokenSource();
                _isRunning = true;

                try
                {
                    while (_isRunning && !_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        try
                        {
                            _client = await _listener.AcceptTcpClientAsync();
                            _ = Task.Run(() => HandleClient(_client, commands, _cancellationTokenSource.Token));
                        }
                        catch (ObjectDisposedException)
                        {
                            // Listener has been stopped
                            break;
                        }
                        catch (InvalidOperationException)
                        {
                            // Listener is in an invalid state
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions that may occur
                }
            }
            catch (Exception)
            {

                //// Do nothing but there is exception with the IP not in context and port being used
            }
        }

        private async Task HandleClient(TcpClient client, List<string> commands, CancellationToken token)
        {
            bool finalCommand = false;
            int commandIndex = 0;

            using (client)
            {
                var buffer = new byte[1024];
                var stream = client.GetStream();
                var accumulatedMessage = new StringBuilder();

                while (!token.IsCancellationRequested)
                {
                    if (!stream.DataAvailable)
                    {
                        await Task.Delay(100, token);
                        continue;
                    }

                    try
                    {
                        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        accumulatedMessage.Append(message);

                        // Process the accumulated message
                        while (accumulatedMessage.ToString().Contains("\r\n"))
                        {
                            var commandEndIndex = accumulatedMessage.ToString().IndexOf("\r\n");
                            var fullCommand = accumulatedMessage.ToString().Substring(0, commandEndIndex + 2);
                            accumulatedMessage.Remove(0, commandEndIndex + 2);

                            // Respond with the next command in the list
                            if (commandIndex < commands.Count)
                            {
                                string commandToSend = commands[commandIndex++];
                                byte[] response = Encoding.ASCII.GetBytes(commandToSend + "\r\n");
                                await stream.WriteAsync(response, 0, response.Length, token);
                            }
                            else
                            {
                                // No more commands to send
                                if (!finalCommand)
                                {
                                    string finalMessage = "No more commands to send. Restart the server.\r\n";
                                    byte[] response = Encoding.ASCII.GetBytes(finalMessage);
                                    await stream.WriteAsync(response, 0, response.Length, token);
                                    finalCommand = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Log or handle other exceptions as needed
                        break;
                    }
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();

            try
            {
                _listener.Stop();
            }
            catch (Exception ex)
            {

            }
        }

        private string _getcurrentIP;

        public string GetcurrentIP
        {
            get { return _getcurrentIP; }
            set { _getcurrentIP = value; }
        }
    }
}
