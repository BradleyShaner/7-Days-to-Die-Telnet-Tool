using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TentacleSoftware.Telnet;

namespace _7DT
{
    public partial class Form1 : Form
    {
        System.Threading.CancellationToken _serverCancellationToken;
        TelnetClient _server;
        string _serverEndpoint;
        bool _connected;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Logger.SetUpLogOutput(textLog);
            Logger.AddLog("Initializing Logger..");
            _serverEndpoint = "127.0.0.1:8081";
            _connected = false;
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            string serverEndpoint = _serverEndpoint;

            if (_connected)
            {
                _server.Disconnect();
                _connected = false;
                this.buttonConnect.Text = "Connect to Server";
                return;
            }

            //Get the server uri
            if (Utilities.ShowInputDialog(ref serverEndpoint, "Server IP:Port") == DialogResult.Cancel)
                return;
            
            Logger.AddLog("IPEndpoint: " + serverEndpoint);

            IPEndPoint endpoint;
            endpoint = Utilities.CreateIPEndPoint(serverEndpoint);

            connectToServer(endpoint.Address.ToString(), endpoint.Port, TimeSpan.FromSeconds(0.3));

        }

        private void connectToServer(string ip, int port, TimeSpan delay)
        {

            if (_connected)
            {
                _server.Disconnect();
                _connected = false;
            }

            Logger.AddLog("connectToServer.. Delay: " + delay.ToString() + ". EndPoint: " + ip + ": " + port);
            _server = new TelnetClient(ip, port, delay, _serverCancellationToken);

            _server.ConnectionClosed += ConnectionClosedEvent;
            _server.MessageReceived += MessageReceivedEvent;

            _server.Connect();
            
        }

        private void MessageReceivedEvent(object sender, string e)
        {
            if (!_connected)
            {

                //if we weren't previously connected, that means it's a new connection..
                
                _connected = true;
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate () { this.Text = "Connected to " + _serverEndpoint; ; });
                    this.BeginInvoke((MethodInvoker)delegate () { this.buttonConnect.Text = "Disconnect"; ; });
                }

                string telnetPass = "";
                if (Utilities.ShowInputDialog(ref telnetPass, "Password", true) == DialogResult.OK)
                {
                    Logger.AddLog("Attempting to login..");
                    _server.Send(telnetPass + "\n");
                    return;
                }

            }

            Logger.AddLog(e);
        }

        private void ConnectionClosedEvent(object sender, EventArgs e)
        {
            _connected = false;

            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate () { this.Text = "Disconnected from " + _serverEndpoint; ; });
                this.BeginInvoke((MethodInvoker)delegate () { this.buttonConnect.Text = "Connect to Server"; ; });
            }

            Logger.AddLog("Disconnected from " + _serverEndpoint);
        }

        private void textInput_TextChanged(object sender, EventArgs e)
        {

        }

        private void textInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendInput(textInput.Text);
                textInput.Clear();
            }
        }

        private void SendInput(string text)
        {
            if (_connected)
            {
                Logger.AddLog(text);
                _server.Send(text + "\n");
            }
        }

    }
}
