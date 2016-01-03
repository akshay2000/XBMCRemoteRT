using Newtonsoft.Json.Linq;
using XBMCRemoteRT.Helpers;

namespace XBMCRemoteRT.RPCWrappers
{
    public class GUI
    {
        public static async void ShowSubtitleSearch()
        {
            JObject parameters = new JObject(new JProperty("window", "subtitlesearch"));
            await ConnectionManager.ExecuteRPCRequest("GUI.ActivateWindow", parameters);
        }

    }
}
