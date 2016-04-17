using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using XBMCRemoteRT.Helpers;

namespace XBMCRemoteRT.RPCWrappers
{
    /// <summary>
    /// Class to wrap around Application namespace of XBMC
    /// Do not rename since the name 'Application' already exists in namespace 'System'
    /// </summary>
    public class Applikation
    {
        public static async Task<JObject> GetProperties()
        {
            JObject parameters = new JObject(new JProperty("properties", new JArray("volume", "muted", "name", "version")));
            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("Application.GetProperties", parameters);
            return (JObject)responseObject["result"];
        }

        public static async Task<int> SetVolume(int volume)
        {
            JObject parameters = new JObject(new JProperty("volume", volume));
            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("Application.SetVolume", parameters);
            return (int)responseObject["result"];
        }

        public static async Task Quit()
        {
            await ConnectionManager.ExecuteRPCRequest("Application.Quit");
        }

        //Extra methods
        public static async Task<int> GetVolume()
        {
            JObject result = await Applikation.GetProperties();
            return (int)result["volume"];
        }
    }
}
