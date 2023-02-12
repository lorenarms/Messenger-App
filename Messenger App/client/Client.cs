using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
    public partial class Client : Form
    {

        private SimpleTcpClient _client;
        private string _localComputerName;



        public Client()
        {
            InitializeComponent();
        }


        private void Client_Load_1(object sender, EventArgs e)
        {

            btnSend.Enabled = false;
            _localComputerName = GetLocalComputerName();

        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            var messageReceived = Encoding.UTF8.GetString(e.Data.ToArray());

            // REQUESTNAME
            if (messageReceived.Contains("REQUESTNAME"))
            {
                _client.Send("NAMEREQUEST+" + _localComputerName);
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    txtInfo.Text +=
                        $@"{e.IpPort}: {messageReceived}{Environment.NewLine}";
                });
            }



        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate {

                txtInfo.Text += $@"Server disconnected.{Environment.NewLine}";
                btnSend.Enabled = false;
                btnConnect.Enabled = true;

            });
        }

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate {

                txtInfo.Text += $@"Server connected.{Environment.NewLine}";

            });
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                _client = new SimpleTcpClient(txtIP.Text);
                _client.Events.Connected += Events_Connected;
                _client.Events.Disconnected += Events_Disconnected;
                _client.Events.DataReceived += Events_DataReceived;
                _client.Connect();
                btnSend.Enabled = true;
                btnConnect.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Message", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
            }
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            if (_client.IsConnected)
            {
                if (!string.IsNullOrEmpty(txtMessage.Text))
                {
                    _client.Send(txtMessage.Text);
                    txtInfo.Text += $@"Me: {txtMessage.Text}{Environment.NewLine}";
                    txtMessage.Text = string.Empty;
                }
            }
        }

        public string GetLocalComputerName()
        {
            return Dns.GetHostName();
        }

    }
}
