using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkMonitor
{
    // client service handler class
    internal class HandleClient
    {
        TcpClient clientSocket = null;
        public Dictionary<TcpClient, string> clientList = null;

        public void StartClient(TcpClient clientSocket, Dictionary<TcpClient, string> clientList)
        {
            this.clientSocket = clientSocket;
            this.clientList = clientList;

            Thread t_handler = new Thread(echo);
            t_handler.IsBackground = true;
            t_handler.Start();
        }

        public delegate void MessageDisplayHandler(string message, string userName);
        public event MessageDisplayHandler OnReceived;

        public delegate void DisconnectedHandler(TcpClient clientSocket);
        public event DisconnectedHandler OnDisconnected;

        private void echo()
        {
            NetworkStream ns = null;

            try
            {
                byte[] buffer = new byte[1024];
                string msg = string.Empty;
                int bytes = 0;
                int MessageCount = 0;

                while (true)
                {
                    MessageCount++;
                    ns = clientSocket.GetStream();
                    bytes = ns.Read(buffer, 0, buffer.Length);
                    msg = Encoding.UTF8.GetString(buffer, 0, bytes);
                    msg = msg.Substring(0, msg.LastIndexOf("$"));

                    Console.WriteLine("msg : " + msg);

                    if (OnReceived != null)
                    {
                        OnReceived(msg, clientList[clientSocket].ToString());
                    }

                    if (msg.Contains("Disconnected"))
                    {
                        OnDisconnected(clientSocket);
                    }
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine("HandleClient Echo SocketException : " + se.ToString());

                if (clientSocket != null)
                {
                    if (OnDisconnected != null)
                    {
                        OnDisconnected(clientSocket);
                    }

                    clientSocket.Close();
                    ns.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HandleClient Echo Exception : " + ex.ToString());

                if (clientSocket != null)
                {
                    if (OnDisconnected != null)
                    {
                        OnDisconnected(clientSocket);
                    }

                    clientSocket.Close();
                    ns.Close();
                }
            }
        }
    }
}
