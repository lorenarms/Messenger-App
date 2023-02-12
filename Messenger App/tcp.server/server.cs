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
        private Dictionary<string, string> _table;


        public Server()
        {
            InitializeComponent();
        }

        private void Server_Load(object sender, EventArgs e)
        {
            txtIP.Text = GetLocalIpAddress() + @":9001";
            _table = new Dictionary<string, string>();
            btnSend.Enabled = false;
        }



        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $@"{e.IpPort} connected.{Environment.NewLine}";
                //lstClientIP.Items.Add(e.IpPort);

                // [HEADER] + [MESSAGE]
                _server.Send(e.IpPort, "REQUESTNAME");

            });
        }

        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            var ipAddressWithPort = e.IpPort;
            var computerToRemove = string.Empty;


            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $@"{e.IpPort} disconnected.{Environment.NewLine}";
                foreach (var item in _table)
                {
                    if (item.Key.Equals(ipAddressWithPort))
                    {
                        computerToRemove = item.Value;

                    }
                }

                lstClientIP.Items.Remove(computerToRemove);
                _table.Remove(ipAddressWithPort);

            });
        }
        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            string messageReceived = Encoding.UTF8.GetString(e.Data.ToArray());

            if (messageReceived.Contains("NAMEREQUEST"))
            {
                var clientComputerName = string.Empty;
                char[] splitter = { '+' };
                string[] messageSplit = messageReceived.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                // NAMEREQUEST+COMPUTERNAME

                foreach (var item in messageSplit)
                {
                    if (item.Equals("NAMEREQUEST"))
                    {
                        continue;
                    }
                    clientComputerName = item;

                    // COMPUTERNAME

                    _table.Add(e.IpPort, clientComputerName);

                    this.Invoke((MethodInvoker)delegate
                    {
                        lstClientIP.Items.Add(clientComputerName);
                    });


                }
            }

            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    txtInfo.Text += $@"{e.IpPort}: {Encoding.UTF8.GetString(e.Data.ToArray())}{Environment.NewLine}";
                });
            }

        }



        private void btnStart_Click(object sender, EventArgs e)
        {
            // loading server here allows user to change ip address
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
            string ipConnection = string.Empty;

            if (_server.IsListening)
            {
                // check if message box is empty or if no client is selected
                if (!string.IsNullOrEmpty(txtMessage.Text) && lstClientIP.SelectedItem != null)
                {
                    foreach (var item in _table)
                    {
                        // <socket> <computer name>
                        if (item.Value == lstClientIP.SelectedItem.ToString())
                        {
                            ipConnection = item.Key;
                        }
                    }


                    _server.Send(ipConnection, txtMessage.Text);

                    txtInfo.Text += $@"Server: {txtMessage.Text}{Environment.NewLine}";
                    txtMessage.Text = string.Empty;
                }
            }
        }

        // get local host ip address
        // populates ip address box for user
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
