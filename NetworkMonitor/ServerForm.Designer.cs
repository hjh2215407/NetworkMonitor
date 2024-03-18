
namespace NetworkMonitor
{
    partial class ServerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtRecvIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.textMessage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 18;
            this.listBox1.Location = new System.Drawing.Point(1003, 95);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(333, 382);
            this.listBox1.TabIndex = 28;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 18);
            this.label5.TabIndex = 27;
            this.label5.Text = "SERVER IP";
            // 
            // txtRecvIP
            // 
            this.txtRecvIP.Location = new System.Drawing.Point(132, 54);
            this.txtRecvIP.Name = "txtRecvIP";
            this.txtRecvIP.Size = new System.Drawing.Size(190, 28);
            this.txtRecvIP.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(328, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 18);
            this.label3.TabIndex = 24;
            this.label3.Text = "PORT";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 18);
            this.label4.TabIndex = 23;
            this.label4.Text = "Connect To";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(387, 16);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(99, 28);
            this.txtPort.TabIndex = 22;
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(132, 16);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(190, 28);
            this.txtServerIP.TabIndex = 21;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(21, 95);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(954, 382);
            this.richTextBox1.TabIndex = 20;
            this.richTextBox1.Text = "";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(1237, 494);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(99, 28);
            this.btnSend.TabIndex = 18;
            this.btnSend.Text = "보내기";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // textMessage
            // 
            this.textMessage.Location = new System.Drawing.Point(21, 494);
            this.textMessage.Name = "textMessage";
            this.textMessage.Size = new System.Drawing.Size(1193, 28);
            this.textMessage.TabIndex = 17;
            this.textMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textMessage_KeyPress);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(1000, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 29;
            this.label1.Text = "Client List";
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1356, 538);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtRecvIP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.textMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ServerForm";
            this.Text = "User Name";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtRecvIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox textMessage;
        private System.Windows.Forms.Label label1;
    }
}