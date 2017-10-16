using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DT
{
    public static class ServerStatusParser
    {
        public static bool ParseStatusLine(string line, ref _ServerStats stats)
        {
            //passed string should look like
            // Time: 2275.38m FPS: 73.19 Heap: 580.6MB Max: 948.1MB Chunks: 380 CGO: 0 Ply: 0 Zom: 6 Ent: 0 (23) Items: 0 CO: 0 RSS: 1530.9MB

            Regex regex = new Regex(@"(?:Time: (?<time>.+) FPS: (?<fps>.+) Heap: (?<heap>.+) Max: (?<max>.+) Chunks: (?<chunk>.+) CGO: (?<cgo>.+) Ply: (?<players>.+) Zom: (?<zombies>.+) Ent: (?<entities>.+) Items: (?<items>.+) CO: (?<co>.+) RSS: (?<rss>\S+))");

            GroupCollection groups = regex.Match(line).Groups;

            var grpNames = regex.GetGroupNames();

            if (groups.Count >= 11)
            {
                stats.uptime = groups["time"].Value;
                stats.fps = double.Parse(groups["fps"].Value);
                stats.heap = groups["heap"].Value;
                stats.heapMax = groups["max"].Value;
                stats.chunks = groups["chunk"].Value;
                stats.cgo = groups["cgo"].Value;
                stats.playerCount = groups["players"].Value;
                stats.zom = groups["zombies"].Value;
                stats.ent = groups["entities"].Value;
                stats.items = groups["items"].Value;
                stats.co = groups["co"].Value;
                stats.RSS = groups["rss"].Value;

                /*
                foreach (var grpName in grpNames)
                {
                    Logger.AddLog(string.Format("Group: {0} Value: {1}", grpName, groups[grpName].Value));
                }
                */

                return true;
            }

            return false;
        }
    }
}
