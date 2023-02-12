using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace tcp.server
{
    public partial class Server : Form
    {
        private SimpleTcpServer _server;


        public Server()
        {
            InitializeComponent();
        }

        private void Server_Load(object sender, EventArgs e)
        {
            txtIP.Text = GetLocalIpAddress() + @":9001";
            btnSend.Enabled = false;
        }
        


        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $@"{e.IpPort} connected.{Environment.NewLine}";
                lstClientIP.Items.Add(e.IpPort);
                
            });
        }

        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $@"{e.IpPort} disconnected.{Environment.NewLine}";
                lstClientIP.Items.Remove(e.IpPort);

            });
        }
        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            
            
            
            var message = Encoding.UTF8.GetString(e.Data.ToArray());

            if (message.Contains("name"))
            {
                string name = message.Remove(0, 3);
            }
            this.Invoke((MethodInvoker)delegate 
            {
                txtInfo.Text += $@"{e.IpPort}: {Encoding.UTF8.GetString(e.Data.ToArray())}{Environment.NewLine}";
            });
            
        }



        private void btnStart_Click(object sender, EventArgs e)
        {
            _server = new SimpleTcpServer(txtIP.Text);
            _server.Events.ClientConnected += Events_ClientConnected;
            _server.Events.ClientDisconnected += Events_ClientDisconnected;
            _server.Events.DataReceived += Events_DataReceived;
            _server.Start();
            txtInfo.Text += $@"Starting...{Environment.NewLine}";
            btnStart.Enabled = false;
            btnSend.Enabled = true;

        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (_server.IsListening)
            {
                // check if message box is empty or if no client is selected
                if(!string.IsNullOrEmpty(txtMessage.Text) && lstClientIP.SelectedItem != null)
                {
                    _server.Send(lstClientIP.SelectedItem.ToString(), txtMessage.Text);
                    txtInfo.Text += $@"Server: {txtMessage.Text}{Environment.NewLine}";
                    txtMessage.Text = string.Empty;
                }
            }
        }

        // get local host ip address
        public static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }




    }
}
