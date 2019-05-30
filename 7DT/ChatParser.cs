using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DT
{
    class ChatParser
    {

        //GMSG: Player 'Dragoon' left the game
        //Also messages from players
        //possibility of scripting server actions to commands, based on user's GM level
        //Chat: 'Dragoon': !test test2

        //Chat: 'DefiledBeing': going to go take a nap

        public static bool ParseChatLine(string line)
        {

            if (ParseTeleportLine(line))
                return true;

            if (ParseMessageLine(line))
                return true;

            return false;
        }

        public static bool ParseMessageLine(string line)
        {
            Regex regex = new Regex(@"(?:to 'Global'\): ')(?<player>\w+)(?:': )(?<message>.+)");

            GroupCollection groups = regex.Match(line).Groups;
            //Logger.AddLog(line);

            Match match = regex.Match(line);

            if (match.Success)
            {
                string player = groups["player"].Value;
                string message = groups["message"].Value.ToLower();

                if (player.ToLower() == "server")
                {
                    Logger.AddLog("Server!");
                    return true;
                }
                Logger.AddLog(player + ": " + message);
                return true;
            }

            return false;
        }

        public static bool ParseTeleportLine(string line)
        { 
            Regex regex = new Regex(@"(?:to 'Global'\): ')(?<player>\w+)(?:': !tp )(?<target>\w+)");

            GroupCollection groups = regex.Match(line).Groups;
            //Logger.AddLog(line);
            //Logger.AddLog("Groups: " + groups.Count);

            Match match = regex.Match(line);

            if (match.Success)
            {
                string player = groups["player"].Value;
                string target = groups["target"].Value.ToLower();
                

                if (target.ToLower() == player.ToLower())
                {
                    formMain._server.Send("say \"" + player + " is a dumbass" + "\"");
                    return true;
                }
                formMain._server.Send("say \"Teleporting " + player + " to " + target + "\"");
                formMain._server.Send("teleportplayer " + player + " " + target);
                Logger.AddLog("Teleporting " + player + " to " + target);
                return true;
            }

            return false;
        }
        
    }
}
