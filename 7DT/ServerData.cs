using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DT
{
    public class ServerData
    {
        public _ServerConfig ServerInfo = new _ServerConfig();
        public _ServerStats ServerStats = new _ServerStats();
        public List<PlayerInfo> players = new List<PlayerInfo>();
        public TelnetState TelnetState = new TelnetState();
    }

    public enum TelnetState
    {
        disconnected = 0,
        connecting,
        loggingIn,
        connected
    }

    public class PlayerInfo
    {
        public string name;
        public string id;
        public string ip;
        public long entityID;
        public bool eac;
        public bool remote;
        public Tuple<long, long, long> pos;
        public Tuple<long, long, long> rot;
        public int health;
        public int score;
        public int level;
        public int zombies;
        public int dealths;
    }

    public class _ServerStats
    {
        public double fps;
        public string uptime;
        public string heap;
        public string heapMax;
        public int chunks;
        public int cgo;
        public int playerCount;
        public int zom;
        public int ent;
        public int items;
        public int co;
        public string RSS;
    }

    public class _ServerConfig
    {
        public string serverVersion = "";
        public string serverIP = "";
        public int serverPort = 0;
        public int maxPlayers = 0;
        public string gameMode = "";
        public string worldName = "";
        public string gameName = "";
        public int difficulty = 0;
    }
}
