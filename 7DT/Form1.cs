using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TentacleSoftware.Telnet;

namespace _7DT
{
    public partial class formMain : Form
    {
        System.Threading.CancellationToken _serverCancellationToken;
        public static TelnetClient _server;
        string _serverEndpoint;
        public static ServerData _serverData = new ServerData();
        public static List<PlayerInfo> players = new List<PlayerInfo>();

        public formMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "7 Days to Die Telnet Tool";

            //Set the textbox for default log output
            Logger.SetUpLogOutput(richTextLog);
            
            //set the default server address and port
            _serverEndpoint = "192.168.10.150:8081";

            //set program connection variables to defaults
            DisconnectFromServer();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            string serverEndpoint = _serverEndpoint;

            if (_serverData.TelnetState != TelnetState.disconnected)
            {
                DisconnectFromServer();
                return;
            }

            //Get the server uri, if they hit cancel, we just return
            if (Utilities.ShowInputDialog(ref serverEndpoint, "Server IP:Port") == DialogResult.Cancel)
                return;
            
            //Create an IPEndpoint from the serverEndpoint inputbox, then connect to it

            try
            {
                IPEndPoint endpoint;
                endpoint = Utilities.CreateIPEndPoint(serverEndpoint);
                
                ConnectToServer(endpoint.Address.ToString(), endpoint.Port, TimeSpan.FromSeconds(0.3));
            } catch (Exception ex)
            {
                Logger.AddLog("Connect Err: " + ex.Message);
            }

        }

        private void ConnectToServer(string ip, int port, TimeSpan delay)
        {

            if (_serverData.TelnetState != TelnetState.disconnected)
                DisconnectFromServer();

            Logger.AddLog("Attempting to connect to " + ip + ": " + port);
            
            //create our new TelnetClient using the previous IPEndpoint
            _server = new TelnetClient(ip, port, delay, _serverCancellationToken);

            //setup our event handlers
            _server.ConnectionClosed += ConnectionClosedEvent;
            _server.MessageReceived += MessageReceivedEvent;

            //set the state to Connecting
            _serverData.TelnetState = TelnetState.connecting;

            _server.Connect();
            
        }

        private void DisconnectFromServer()
        {
            try
            {
                _server.Disconnect();
            } catch {}
            
            _serverData = new ServerData();
            _serverData.TelnetState = TelnetState.disconnected;

            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate () { this.buttonConnect.Text = "Connect to Server"; ; });
            } else
            {
                this.buttonConnect.Text = "Connect to Server";
            }
            return;
        }

        private void MessageReceivedEvent(object sender, string e)
        {
            
            switch (_serverData.TelnetState)
            {

                case TelnetState.connecting:
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke((MethodInvoker)delegate () { this.Text = "Connected to " + _serverEndpoint; ; });
                        this.BeginInvoke((MethodInvoker)delegate () { this.buttonConnect.Text = "Disconnect"; ; });
                    }

                    string telnetPass = "";
                    if (Utilities.ShowInputDialog(ref telnetPass, "Password", true) == DialogResult.OK)
                    {
                        _serverData.TelnetState = TelnetState.loggingIn;
                        Logger.AddLog("Attempting to login..");
                        _server.Send(telnetPass + "\n");
                        return;
                    }
                    break;
                    
                case TelnetState.loggingIn:
                    //Data received here should be responses to the password field

                    //Wait for "Press 'help' to get a list of all commands.Press 'exit' to end session."
                    //Process lines received before the above line, then set to connected state.
                    try
                    {
                        if (AuthParser.ParseWelcomeLine(e, ref _serverData.ServerInfo))
                            return;
                    } catch (Exception ex)
                    {
                        Logger.AddLog(ex.Message);
                    }
                    break;
                    

                case TelnetState.connected:
                    //Process commands with regex from this point forward
                    
                    Regex reg = new Regex(@"^(\d{4}-\d{2}-\d{2}T\S+) (?:\S+) (?<log>\w+)");

                    var match = reg.Match(e);
                    string logData;
                    
                    if (match.Success)
                    {
                        string logType = match.Groups[2].ToString();
                        logData = e.Substring(match.Index + match.Length).Trim();
                        //Logger.AddLog("LogType: " + logType + logData);

                        switch (logType)
                        {
                            case "INF":

                                //returns true if the passed line is a server status tick
                                if (ServerStatusParser.ParseStatusLine(logData, ref _serverData.ServerStats))
                                    return;

                                if (ChatParser.ParseChatLine(logData))
                                    return;

                                break;

                            case "WRN":
                                Logger.AddLog("Warning: " + logData);
                                return;
                                break;

                            default:
                                Logger.AddLog("Unknown logType: " + logType + ":" + logData);
                                break;
                        }
                        
                        Logger.AddLog("Initial match found, but no matches found for this data.");
                    }

                    break;
            }
            
            Logger.AddLog(e);
        }

        private void ConnectionClosedEvent(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate () { this.Text = "Disconnected from " + _serverEndpoint; ; });
            }

            _serverData.TelnetState = TelnetState.disconnected;

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
            if (_serverData.TelnetState != TelnetState.disconnected)
            {
                Logger.AddLog("#> " + text);
                _server.Send(text + "\n");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textStatus.Clear();

            textStatus.AppendText("Server Address: " + _serverData.ServerInfo.serverIP + ": " + _serverData.ServerInfo.serverPort + "\n");
            textStatus.AppendText("Server Version: " + _serverData.ServerInfo.serverVersion + "\n");
            textStatus.AppendText("Server MaxPlayers: " + _serverData.ServerInfo.maxPlayers + "\n");
            textStatus.AppendText("Server Name: " + _serverData.ServerInfo.gameName + "\n");
            textStatus.AppendText("Server World Name: " + _serverData.ServerInfo.worldName + "\n");
            textStatus.AppendText("Server Mode: " + _serverData.ServerInfo.gameMode + "\n");
            textStatus.AppendText("Server Difficulty: " + _serverData.ServerInfo.difficulty + "\n");
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string ee = "";
            Utilities.ShowInputDialog(ref ee, "test");
            Regex regex = new Regex(@"(?:Chat: ')(?<player>\w+)(': !tp )(?<target>\w+)");
            
            GroupCollection groups = regex.Match(ee).Groups;

            var grpNames = regex.GetGroupNames();

            
                foreach (var grpName in grpNames)
                {
                    Logger.AddLog(string.Format("Group: {0} Value: {1}", grpName, groups[grpName].Value));
                }
                
               
            Logger.AddLog("Initial match found, but no matches found for this data.");
            
        }
    }
}
