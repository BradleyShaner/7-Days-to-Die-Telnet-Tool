using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DT
{
    static class LoginParser
    {
        public static bool ParseWelcomeLine(string line, ref _ServerConfig config)
        {

            //if no colon is in the line, 
            if (!line.Contains(":") && config.gameMode.Length > 0)
            {
                formMain._serverData.TelnetState = TelnetState.connected;
                Logger.AddLog("Authenticated successfully!");
                return true;
            }

            if (line.Contains("***") && line.Contains("version"))
            {
                //just get the whole version string, including compatability version fornow
                config.serverVersion = line.Substring(line.IndexOf(": ") + 2).Trim();
                return true;
            }

            if (line.Contains("Server IP:"))
            {
                config.serverIP = line.Substring(line.IndexOf(":")+2).Trim();
                return true;
            }

            if (line.Contains("Server port:"))
            {
                config.serverPort = Int32.Parse(line.Substring(line.IndexOf(":") + 2).Trim());
                return true;
            }

            if (line.Contains("Max players:"))
            {
                config.maxPlayers = Int32.Parse(line.Substring(line.IndexOf(":") + 2).Trim());
                return true;
            }

            if (line.Contains("Game mode:"))
            {
                config.gameMode = line.Substring(line.IndexOf(":") + 2).Trim();
                return true;
            }

            if (line.Contains("World:"))
            {
                config.worldName = line.Substring(line.IndexOf(":") + 2).Trim();
                return true;
            }

            if (line.Contains("Game name:"))
            {
                config.gameName = line.Substring(line.IndexOf(":") + 2).Trim();
                return true;
            }

            if (line.Contains("Difficulty:"))
            {
                config.difficulty = Int32.Parse(line.Substring(line.IndexOf(":") + 2).Trim());
                return true;
            }

            return false;
        }
    }
}
