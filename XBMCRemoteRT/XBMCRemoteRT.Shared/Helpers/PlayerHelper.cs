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

            JToken id = null;

            if (item.TryGetValue("id", out id))
            {
                GlobalVariables.CurrentPlayerState.ItemId = (int)item["id"];
                GlobalVariables.CurrentPlayerState.Title = (string)item["title"];
            }
            else
            {
                GlobalVariables.CurrentPlayerState.ItemId = -1;
                GlobalVariables.CurrentPlayerState.Title = (string)item["label"];
            }
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

            if (result != null)
            {
                JObject totalTime = (JObject)result["totaltime"];
                GlobalVariables.CurrentPlayerState.TotalTimeSeconds = ((int)totalTime["hours"] * 3600) + ((int)totalTime["minutes"] * 60) + (int)totalTime["seconds"];

                JObject time = (JObject)result["time"];
                GlobalVariables.CurrentPlayerState.TimeSeconds = ((int)time["hours"] * 3600) + ((int)time["minutes"] * 60) + (int)time["seconds"];

                GlobalVariables.CurrentPlayerState.Speed = (int)result["speed"];
            }
        }

        public static async Task<List<Player.AudioStreamExtended>> GetAudioStreams()
        {
            JArray properties = new JArray("audiostreams", "currentaudiostream");

            JObject result = await Player.GetProperties(GlobalVariables.CurrentPlayerState.PlayerType, properties);

            if (result != null)
            {
                JArray streams = (JArray)result["audiostreams"];
                if (streams.HasValues)
                {
                    List<Player.AudioStreamExtended> newstreams = new List<Player.AudioStreamExtended>();
                    foreach (JToken s in streams)
                    {
                        Player.AudioStreamExtended news = new Player.AudioStreamExtended();
                        news.index = (int)s["index"];
                        news.language = (string)s["language"];
                        news.name = (string)s["name"];
                        newstreams.Add(news);
                    }

                    JToken curstream = result["currentaudiostream"];

                    if (curstream.HasValues)
                    {
                        int id = (int)curstream["index"];
                        Player.AudioStreamExtended curitem = newstreams.FirstOrDefault(s => s.index == id);
                        if (curitem != null)
                        {
                            curitem.isInUse = true;
                            foreach (Player.AudioStreamExtended ase in newstreams)
                                if (ase.isInUse == null)
                                    ase.isInUse = false;

                            curitem.channels = (int)curstream["channels"];
                            curitem.codec = (string)curstream["codec"];
                            curitem.bitrate = (int)curstream["bitrate"];
                        }
                    }

                    return newstreams;
                }
            }

            return null;
        }

        public static async Task<List<Player.SubtitleExtended>> GetSubtitles()
        {
            JArray properties = new JArray("subtitles", "currentsubtitle", "subtitleenabled");

            JObject result = await Player.GetProperties(GlobalVariables.CurrentPlayerState.PlayerType, properties);

            if (result != null)
            {
                JArray subtitles = (JArray)result["subtitles"];
                if (subtitles.Count != 0)
                {
                    JToken subtitleenabled = result["subtitleenabled"];

                    if (subtitleenabled != null)
                    {
                        bool stenabled = (bool)subtitleenabled;

                        List<Player.SubtitleExtended> newsubtitles = new List<Player.SubtitleExtended>();
                        foreach (JToken s in subtitles)
                        {
                            Player.SubtitleExtended news = new Player.SubtitleExtended();
                            news.index = (int)s["index"];
                            news.language = (string)s["language"];
                            news.name = (string)s["name"];
                            newsubtitles.Add(news);
                        }

                        JToken cursubtitle = result["currentsubtitle"];

                        if (cursubtitle.Count() != 0 && stenabled)
                        {
                            int id = (int)cursubtitle["index"];

                            Player.SubtitleExtended curitem = newsubtitles.FirstOrDefault(s => s.index == id);

                            if (curitem != null)
                                curitem.isInUse = true;
                        }

                        foreach (Player.SubtitleExtended se in newsubtitles)
                            if (se.isInUse == null)
                                se.isInUse = false;

                        return newsubtitles;
                    }
                }
            }

            return null;
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

        private async static void timer_Tick(object sender, object e)
        {
            await PlayerHelper.RefreshPlayerState();
        }
    }
}
