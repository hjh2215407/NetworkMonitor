using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkMonitor
{
    public class Server
    {
        public const int PORT = 5000;
        TcpListener listener = null;
        public static ArrayList handleList = new ArrayList(20);
        private RichTextBox richTextBox;

        public Server(RichTextBox textBox)
        {
            richTextBox = textBox;
            handleList.Clear();
        }

        public void Echo()
        {
            try
            {
                IPAddress address = Dns.GetHostEntry("").AddressList[0];
                //listener = new TcpListener(address, PORT);
                listener = new TcpListener(IPAddress.Any, PORT);
                listener.Start();
                Console.WriteLine("Server ready 1-------");

                richTextBox.Invoke((MethodInvoker)delegate
                {
                    richTextBox.AppendText("Server ready 1-------" + "\r\n");
                    richTextBox.Focus();
                    richTextBox.ScrollToCaret();
                });

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient(); //클라이언트 개당 소켓생성
                    EchoHandler handler = new EchoHandler(this, client, richTextBox);
                    Add(handler);

                    handler.start();
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine("2--------------------");
                Console.WriteLine(ee.Message);

                richTextBox.Invoke((MethodInvoker)delegate
                {
                    richTextBox.AppendText("2--------------------" + "\r\n");
                    richTextBox.AppendText(ee.Message + "\r\n");
                    richTextBox.Focus();
                    richTextBox.ScrollToCaret();
                });
            }
            finally
            {
                Console.WriteLine("3--------------------");
                richTextBox.Invoke((MethodInvoker)delegate
                {
                    
                    richTextBox.AppendText("3--------------------" + "\r\n");
                    richTextBox.Focus();
                    richTextBox.ScrollToCaret();
                });

                listener.Stop();
            }
        }

        public void Add(EchoHandler handler)
        {
            lock (handleList.SyncRoot)
                handleList.Add(handler);
        }

        public void broadcast(String str)
        {
            lock (handleList.SyncRoot)
            {
                string dstes = DateTime.Now.ToString() + " : ";
                Console.Write("Server) " + dstes);
                Console.WriteLine(str);

                richTextBox.Invoke((MethodInvoker)delegate
                {
                    richTextBox.AppendText(dstes + str + "\r\n");
                    richTextBox.Focus();
                    richTextBox.ScrollToCaret();
                });

                foreach (EchoHandler handler in handleList)
                {
                    EchoHandler echo = handler as EchoHandler;
                    if (echo != null)
                    {
                        echo.sendMessage(str);
                    }
                }
            }
        }
        public void Remove(EchoHandler handler)
        {
            lock (handleList.SyncRoot)
            {
                handleList.Remove(handler);
            }
        }
    }

    // server-client message recv/send handler
    public class EchoHandler
    {
        Server server;
        TcpClient client;
        NetworkStream ns = null;
        StreamReader sr = null;
        StreamWriter sw = null;
        string str = string.Empty;

        RichTextBox richTextBox;

        string clientName;
        public EchoHandler(Server server, TcpClient client, RichTextBox richTextBox)
        {
            this.server = server;
            this.client = client;
            this.richTextBox = richTextBox;

            try
            {
                ns = client.GetStream();
                Socket socket = client.Client;
                clientName = socket.RemoteEndPoint.ToString();

                richTextBox.Invoke((MethodInvoker)delegate
                {
                    richTextBox.AppendText(clientName + " Connected" + "\r\n");
                    richTextBox.Focus();
                    richTextBox.ScrollToCaret();
                });

                Console.WriteLine(clientName + " Connected");
                sr = new StreamReader(ns, Encoding.UTF8);
                sw = new StreamWriter(ns, Encoding.UTF8);
            }
            catch (Exception e) 
            {
                richTextBox.Invoke((MethodInvoker)delegate
                {
                    richTextBox.AppendText("Connection Failed" + "\r\n");
                    richTextBox.Focus();
                    richTextBox.ScrollToCaret();
                });
                Console.WriteLine("Server) " + "Connection Failed : " + e.ToString()); 
            
            }
        }
        public void start()
        {
            Thread t = new Thread(new ThreadStart(ProcessClient));
            t.Start();
        }
        public void ProcessClient()
        {
            try
            {
                while ((str = sr.ReadLine()) != null)
                {
                    server.broadcast(str);
                }
                    
            }
            catch (Exception e)
            {
                richTextBox.Invoke((MethodInvoker)delegate
                {
                    richTextBox.AppendText(clientName + " Disconnected" + "\r\n");
                    richTextBox.Focus();
                    richTextBox.ScrollToCaret();
                });
                Console.WriteLine("Server) " + clientName + " Disconnected({0})", e.ToString());
                sw.Flush();
            }
            finally
            {
                server.Remove(this);
                sw.Close();
                sr.Close();
                client.Close();
            }
        }
        public void sendMessage(string message)
        {
            if ((message.Contains("S:") && message.Contains("R:") && message.Contains("M:")))
            {
                // check format
                Console.WriteLine("Server) " + "Recv Message : " + message);
                string[] strArry = message.Split('/');
                string sendIP = strArry[0].Replace("S:", "");
                string recvIP = strArry[1].Replace("R:", "");
                string sendMsg = strArry[2].Replace("M:", "");

                Console.WriteLine("Server) " + "Send IP : {0}, Recv IP : {1}, Send Message : {2}", sendIP, recvIP, sendMsg);

                // check sendIP, receiveIP
                if (clientName.Contains(recvIP) || clientName.Contains(sendIP))
                {
                    sw.WriteLine(sendMsg);
                    sw.Flush();
                }
            }
            else
            {
                // just for check should be blocked
                sw.WriteLine(message + " ... format difference");
                sw.Flush();
            }

            /*sw.WriteLine(message);
            sw.Flush();*/
        }

    }
}
