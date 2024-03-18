using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkMonitor
{
    public partial class StartDialog : Form
    {
        static string SERVER = "SERVER";
        static string CLIENT = "CLIENT";

        public StartDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Server
            this.Close();

            ServerForm form1 = new ServerForm(SERVER);
            form1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Client
            this.Close();

            ClientForm form1 = new ClientForm(CLIENT);
            form1.Show();
        }
    }
}
