using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using XBMCRemoteRT.Helpers;

namespace XBMCRemoteRT.RPCWrappers
{
    public class GUI
    {
        public static void ShowSubtitleSearch()
        {
            JObject parameters = new JObject(new JProperty("window", "subtitlesearch"));
            ConnectionManager.ExecuteRPCRequest("GUI.ActivateWindow", parameters);
        }

    }
}
