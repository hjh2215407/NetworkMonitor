using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    public partial class ServerForm : Form
    {
        public const string PORT = "5000";

        string hostType = null;

        string serverIP = "192.168.10.127";
        string localIP;

        int serverPort = 5000;
        bool isAlive = false;

        NetworkStream ns = null;
        TcpClient client = null;

        Dictionary<TcpClient, string> clientList = new Dictionary<TcpClient, string>();

        RichTextBox messageBox = null;
        TcpListener server = null;

        string userName = string.Empty;

        public ServerForm(string type)
        {
            InitializeComponent();

            hostType = type;

            // Get local IP address and set server ip
            IPHostEntry hostIP = Dns.GetHostEntry(Dns.GetHostName());
            localIP = hostIP.AddressList[1].MapToIPv4().ToString();

            serverIP = localIP;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = hostType;
            listBox1.Items.Clear();
            messageBox = this.richTextBox1;

            if (hostType == "SERVER")
            {
                Thread t = new Thread(InitSocket);
                t.IsBackground = true;
                t.Start();

                txtServerIP.Text = serverIP;
                txtPort.Text = PORT;

                txtServerIP.Enabled = false;
                txtPort.Enabled = false;

                label4.Visible = false;
                txtRecvIP.Visible = false;
            }
            else if (hostType == "CLIENT")
            {
                txtServerIP.Text = serverIP;
                txtPort.Text = PORT;
                txtPort.Enabled = true;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = this.textMessage.Text;

            if (msg == "")
            {
                MessageBox.Show("Input messages");
                return;
            }

            SendMessageAll(msg.Trim() + "$", "SERVER", true);
            this.textMessage.Clear();
            this.textMessage.SelectionStart = 0;
        }

        private void textMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)Keys.Enter == e.KeyChar)
            {
                string msg = this.textMessage.Text;

                if (msg == "")
                {
                    MessageBox.Show("Input Messages");
                    return;
                }

                SendMessageAll(msg.Trim() + "$", "SERVER", true);
                this.textMessage.Clear();
                this.textMessage.SelectionStart = 0;
            }
        }

        private void InitSocket()
        {
            // set server side tcp listener
            server = new TcpListener(IPAddress.Parse(txtServerIP.Text), int.Parse(txtPort.Text));
            client = default(TcpClient);
            server.Start();
            AppendMessage("Server Start");

            while (true)
            {
                try
                {
                    client = server.AcceptTcpClient();
                    Socket socket = client.Client;
                    
                    ns = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytes = ns.Read(buffer, 0, buffer.Length);
                    userName = Encoding.UTF8.GetString(buffer, 0, bytes);
                    userName = userName.Substring(0, userName.IndexOf("$"));

                    AppendMessage("System : [ " + userName +" ] Connected");

                    clientList.Add(client, userName);// client list add
                    SendMessageAll(userName + " connected", "", false);// send message to connected client
                    SetUserList(userName, "I");//update connect user listbox

                    // set client service handler(connection, data recv/send)
                    HandleClient h_client = new HandleClient();
                    h_client.OnReceived += new HandleClient.MessageDisplayHandler(OnReceived);
                    h_client.OnDisconnected += new HandleClient.DisconnectedHandler(h_client_OnDisconnected);
                    h_client.StartClient(client, clientList);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("InitSocket Exception : " + ex.ToString());
                }
            }

            client.Close();
            server.Stop();
        }

        private void h_client_OnDisconnected(TcpClient clientSocket)
        {
            Console.WriteLine("Client Disconnected Event");
            if (clientList.ContainsKey(clientSocket))
            {
                Console.WriteLine("Removed");
                clientList.Remove(clientSocket);
            }

            string sendMessage = "ClientList";
            foreach (var item in clientList)
            {
                Console.WriteLine("Client : " + item.Key + ", Value : " + item.Value);
                sendMessage += "@" + item.Value;
            }
            sendMessage += "$";
            SendMessageAll(sendMessage, "SYSTEM", true);
        }

        private void OnReceived(string message, string userName)
        {
            Console.WriteLine("OnReceived Message // [" + userName + "]: " + message);

            if (message.Contains("Disconnected"))
            {
                string displayMessage = userName + " disconnected";
                AppendMessage(displayMessage);
                SendMessageAll("Disconnected", userName, true);
                SetUserList(userName, "D");
            }
            else if (message.Contains("Connected"))
            {
                string sendMessage = "ClientList";
                foreach (var item in clientList)
                {
                    Console.WriteLine("Client : " + item.Key + ", Value : " + item.Value);
                    sendMessage += "@" + item.Value ;
                }
                sendMessage += "$";
                SendMessageAll(sendMessage, "SYSTEM", true);
            }
            else
            {
                string displayMessage = string.Empty;
                displayMessage = "[" + userName + "]" + ": " + message;
               
                AppendMessage(displayMessage);
                SendMessageAll(message, userName, true);
            }
        }

        private void AppendMessage(string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (this.richTextBox1 != null && this.txtPort != null)
                {
                    string date = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss"); // get current datetime

                    if (hostType == "SERVER")
                    {
                        this.richTextBox1.AppendText("[" + date + "]" + msg + "\r\n");
                        this.richTextBox1.Focus();
                    }
                    else if (hostType == "CLIENT")
                    {
                        this.richTextBox1.AppendText(msg + "\r\n");
                        this.textMessage.Focus();
                    }

                    this.richTextBox1.ScrollToCaret();
                }
            });
        }
        private void SendMessageAll(string message, string userName, bool flag)
        {
            foreach (var pair in clientList)
            {
                string date = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss"); // get current datetime

                TcpClient client = pair.Key as TcpClient;
                NetworkStream stream = client.GetStream();
                byte[] buffer = null;

                if (flag)
                {
                    if (message.Contains("Disconnected"))
                    {
                        buffer = Encoding.UTF8.GetBytes(userName + " disconnected");
                    }
                    else if (message.Contains("ClientList"))
                    {
                        buffer = Encoding.UTF8.GetBytes("[" + userName + "]" + " : " + message);
                    }
                    else
                    {
                        if (userName.Equals("SERVER"))
                        {
                            buffer = Encoding.UTF8.GetBytes("[" + userName + "]" + " : " + message.Replace("$", ""));
                            stream.Write(buffer, 0, buffer.Length);
                            stream.Flush();
                            continue;
                        }

                        string[] msgArry = message.Split('$');
                        string recvUser = msgArry[1].Replace("R:", "");
                        string msg = msgArry[2].Replace("M:", "");

                        // show message to send user, recv user
                        if (pair.Value.Equals(recvUser) || pair.Value.Equals(userName))
                        {
                            buffer = Encoding.UTF8.GetBytes("[" + date + "] " + "[" + userName + "]" + " : " + msg);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    buffer = Encoding.UTF8.GetBytes(message);
                }

                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }

        private void SetUserList(string userName, string div)
        {
            try
            {
                if (div.Equals("I"))
                {
                    listBox1.Invoke((MethodInvoker)delegate { listBox1.Items.Add(userName); });
                }
                else if (div.Equals("D"))
                {
                    listBox1.Invoke((MethodInvoker)delegate { listBox1.Items.Remove(userName); });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetUserList Exception : " + ex.ToString());
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
            Environment.Exit(0);
        }
    }
}
