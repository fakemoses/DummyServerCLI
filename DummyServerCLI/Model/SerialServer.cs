using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;

namespace DummyServerCLI.Model
{
    public class SerialServer
    {
        private SerialPort _serialPort;
        private bool _isRunning;
        private CancellationTokenSource _cancellationTokenSource;
        private List<string> _commands;
        private int _commandIndex = 0;
        private bool _finalCommand = false;
        private StringBuilder _accumulatedBuffer;

        public SerialServer(string portName, int baudRate)
        {
            _accumulatedBuffer = new StringBuilder();
            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            _serialPort.Handshake = Handshake.None;
            _serialPort.DataReceived += SerialPort_DataReceived;
        }

        public void Start(List<string> commands)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            this._commands = commands;
            try
            {
                _serialPort.Open();
                _isRunning = true;

                //// Task running on another thread. Please make sure to notify the main thread or something. Cross thread exception
                Task.Run(() => RunServer(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                Stop();
            }
        }

        private void RunServer(CancellationToken token)
        {
            try
            {
                while (_isRunning && !token.IsCancellationRequested)
                {
                    // Keep the server running
                    Thread.Sleep(100); // Sleep to prevent high CPU usage
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();

            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Close();
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort serialPort = (SerialPort)sender;
                string data = serialPort.ReadExisting();

                _accumulatedBuffer.Append(data);

                //echo back the received data
                serialPort.Write(data);

                while (_accumulatedBuffer.ToString().Contains("\r"))
                {
                    // Extract the full command from the buffer
                    int commandEndIndex = _accumulatedBuffer.ToString().IndexOf("\r\n");
                    string fullCommand = _accumulatedBuffer.ToString(0, commandEndIndex + 2);
                    _accumulatedBuffer.Remove(0, commandEndIndex + 2);

                    // Process the full command
                    if (_commandIndex < _commands.Count)
                    {
                        // Send the next command in the list
                        string commandToSend = _commands[_commandIndex++];
                        byte[] response = Encoding.ASCII.GetBytes(commandToSend + "\r\n");
                        serialPort.Write(response, 0, response.Length);
                    }
                    else
                    {
                        // No more commands to send
                        if (!_finalCommand)
                        {
                            string finalMessage = "No more commands to send. Restart the server.\r\n";
                            byte[] response = Encoding.ASCII.GetBytes(finalMessage);
                            serialPort.Write(response, 0, response.Length);
                            _finalCommand = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
