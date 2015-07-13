using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using XBMCRemoteRT.Models;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Helpers
{
    public class PlayerHelper
    {
        public static async Task RefreshPlayerState()
        {
            var activePlayers = await Player.GetActivePlayers();
            if (activePlayers.Count == 1)
            {
                GlobalVariables.CurrentPlayerState.PlayerType = activePlayers[0];
                await RefreshPlayerItem(activePlayers[0]);
                await RefreshPlayerProperties(activePlayers[0]);
            }
            else if (activePlayers.Count == 0)
            {                
                GlobalVariables.CurrentPlayerState.SetDefaultState();
            }
        }

        private static async Task RefreshPlayerItem(Players player)
        {
            JArray properties = new JArray("title", "artist", "fanart", "thumbnail", "showtitle", "tagline");
            JObject result = await Player.GetItem(player, properties);
            JObject item = (JObject)result["item"];

            GlobalVariables.CurrentPlayerState.ItemId = (int)item["id"];
            GlobalVariables.CurrentPlayerState.Title = (string)item["title"];
            GlobalVariables.CurrentPlayerState.Artist = ((JArray)item["artist"]).ToObject<List<string>>();
            GlobalVariables.CurrentPlayerState.Fanart = (string)item["fanart"];
            GlobalVariables.CurrentPlayerState.Thumbnail = (string)item["thumbnail"];
            GlobalVariables.CurrentPlayerState.ShowTitle = (string)item["showtitle"];
            GlobalVariables.CurrentPlayerState.Tagline = (string)item["tagline"];
        }

        private static async Task RefreshPlayerProperties(Players player)
        {
            JArray properties = new JArray("time", "totaltime", "speed");
            JObject result = await Player.GetProperties(GlobalVariables.CurrentPlayerState.PlayerType, properties);

            JObject totalTime = (JObject)result["totaltime"];
            GlobalVariables.CurrentPlayerState.TotalTimeSeconds = ((int)totalTime["hours"] * 3600) + ((int)totalTime["minutes"] * 60) + (int)totalTime["seconds"];
            
            JObject time = (JObject)result["time"];
            GlobalVariables.CurrentPlayerState.TimeSeconds = ((int)time["hours"] * 3600) + ((int)time["minutes"] * 60) + (int)time["seconds"];

            GlobalVariables.CurrentPlayerState.Speed = (int)result["speed"];
        }

        private static DispatcherTimer timer;

        public static void StartAutoRefresh(uint seconds)
        {
            if (timer != null)
            {
                timer.Stop();
            }
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(seconds);
            timer.Start();
            timer.Tick += timer_Tick;       
        }

        private static void timer_Tick(object sender, object e)
        {
            PlayerHelper.RefreshPlayerState();
        }
    }
}
