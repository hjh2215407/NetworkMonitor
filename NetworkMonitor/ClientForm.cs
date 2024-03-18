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
    public partial class ClientForm : Form
    {
        public const string PORT = "5000";

        string hostType = null;

        string serverIP = "192.168.10.127";
        string localIP;

        string sendIP;
        string recvUserName = string.Empty;

        string userName = null;

        int serverPort = 5000;
        bool isAlive = false;

        NetworkStream ns = null;
        StreamReader sr = null;
        StreamWriter sw = null;
        TcpClient clientSocket = null;

        Server serverProcess;

        Thread receiveThread;

        RichTextBox messageBox = null;

        public ClientForm(string type)
        {
            InitializeComponent();

            hostType = type;

            IPHostEntry hostIP = Dns.GetHostEntry(Dns.GetHostName());
            localIP = hostIP.AddressList[1].MapToIPv4().ToString();
            
            sendIP = localIP;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = hostType;
            listBox1.Items.Clear();
            messageBox = this.richTextBox1;

            if (hostType == "SERVER")
            {
                Thread t = new Thread(ServerProcess);
                t.Start();

                btnConnect.Enabled = false;
                txtServerIP.Text = serverIP;
                txtPort.Text = PORT;

                txtServerIP.Enabled = false;
                txtPort.Enabled = false;

                btnConnect.Visible = false;
                btnExit.Visible = false;
            }
            else if (hostType == "CLIENT")
            {
                txtServerIP.Text = serverIP;
                txtPort.Text = PORT;
                txtPort.Enabled = true;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            recvUserName = string.Empty;
            userName = txtUserName.Text;
            serverIP = txtServerIP.Text;

            if (string.IsNullOrEmpty(this.txtPort.Text))
            {
                MessageBox.Show("Input Port Number");
                return;
            }
            if (clientSocket != null && clientSocket.Connected)
            {
                MessageBox.Show("Already Connected.");
                return;
            }
            serverPort = Int32.Parse(this.txtPort.Text);
            isAlive = true;

            try
            {
                if (isAlive)
                {
                    Connect();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Server not working", "Connection failed");
                this.isAlive = false;
                txtServerIP.Enabled = true;
                txtPort.Enabled = true;
                txtUserName.Enabled = true;

                return;
            }

            try
            {
                this.GetMessage();
                sendMessage("Connected");
                txtServerIP.Enabled = false;
                txtPort.Enabled = false;
                txtUserName.Enabled = false;
            }
            catch (Exception ee)
            {
                this.textMessage.Clear();
                this.isAlive = false;
                txtServerIP.Enabled = true;
                txtPort.Enabled = true;
                txtUserName.Enabled = true;
                Console.WriteLine("Client) " + "Connection Exception : " + ee.ToString());
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

            sendMessage(msg.Trim());
            this.textMessage.Clear();
            this.textMessage.SelectionStart = 0;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isAlive)
                {
                    MessageBox.Show("Not connected to Server.");
                    return;
                }

                DisConnect();
                isAlive = false;
                listBox1.Items.Clear();

                txtServerIP.Enabled = true;
                txtPort.Enabled = true;
                txtUserName.Enabled = true;
            }
            catch (Exception ee)
            {
                Console.WriteLine("Client) " + "Stream Close Exception : " + ee.ToString());               
            }
            finally
            {
                //this.Dispose();
            }
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

                sendMessage(msg.Trim());
                this.textMessage.Clear();
                this.textMessage.SelectionStart = 0;
            }
        }

        private void ServerProcess()
        {
            serverProcess = new Server(this.richTextBox1);

            serverProcess.Echo();
        }

        public void GetMessage()
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(userName + "$");

                if(ns != null)
                {
                    ns.Write(buffer, 0, buffer.Length);
                    ns.Flush();
                }
                else
                {
                    return;
                }
                receiveThread = new Thread(new ThreadStart(run));
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show("Connection Fail");
                Console.WriteLine("Client) Echo Exception : " + e.ToString());
            }
        }

        public void run()
        {
            string msg = "start";

            try
            {
                while (clientSocket != null && clientSocket.Connected)
                {
                    ns = clientSocket.GetStream();
                    int BUFFERSIZE = clientSocket.ReceiveBufferSize;
                    byte[] buffer = new byte[BUFFERSIZE];

                    int bytes = ns.Read(buffer, 0, buffer.Length);
                    msg = Encoding.UTF8.GetString(buffer, 0, bytes);

                    UpdateClientList(msg);
                    if (!msg.Contains("[SYSTEM]"))
                    {
                        AppendMessage(msg);
                    }
                }
                //ns.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error !");
                Console.WriteLine("Client) " + "Run Exception : " + e.ToString());
            }
        }

        private void Connect()
        {
            clientSocket = new TcpClient();
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(this.serverIP), this.serverPort);
            clientSocket.Connect(iPEnd);
            ns = clientSocket.GetStream();
        }

        private void DisConnect()
        {
            clientSocket = null;
            byte[] buffer = Encoding.UTF8.GetBytes("Disconnected" + "$");
            if (ns != null)
            {
                ns.Write(buffer, 0, buffer.Length);
                ns.Flush();
            }
        }

        private void AppendMessage(string msg)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (this.richTextBox1 != null && this.txtPort != null)
                {
                    if (hostType == "SERVER")
                    {
                        this.richTextBox1.AppendText(msg + "\r\n");
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

        private void sendMessage(string msg)
        {
            try
            {
                if (ns != null)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(ConvertMessage(msg + "$"));
                    ns.Write(buffer, 0, buffer.Length);
                    ns.Flush();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Send Error!");
                Console.WriteLine("Client) " + "SendMessage Exception : " + e.ToString());
            }
        }

        private string ConvertMessage(string msg)
        {
            if (recvUserName == string.Empty)
            {
                recvUserName = "SERVER";
            }
            msg = "$R:" + recvUserName + "$M:" + msg + "";
            return msg;
        }

        private void UpdateClientList(string msg)
        {
            if (msg.Contains("ClientList@"))
            {
                msg = msg.Substring(msg.IndexOf("@") + 1).Replace("$", "");
                Console.WriteLine("msg : " + msg);
                string[] msgArry = msg.Split('@');

                listBox1.Invoke((MethodInvoker)delegate { listBox1.Items.Clear(); });

                // Empty
                if (listBox1.Items.Count == 0)
                {
                    foreach (var item in msgArry)
                    {
                        if(!item.Equals(userName)) SetUserList(item, "I");
                    }
                }
            }
        }

        private void SetUserList(string userName, string div)
        {
            try
            {
                if (div.Equals("I"))
                {
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        string listBoxItemText = listBox1.Items[i].ToString();
                        if (listBoxItemText.Equals(userName)) return;
                    }

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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("Client {0} selcted", listBox1.SelectedItem.ToString()));
            recvUserName = listBox1.SelectedItem.ToString();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            
            Application.ExitThread();
            Environment.Exit(0);
        }
    }
}
