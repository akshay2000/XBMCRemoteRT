using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using XBMCRemoteRT.Helpers;

namespace XBMCRemoteRT.RPCWrappers
{
    public enum InputCommands { Home, Back, Select, Left, Up, Right, Down, ShowOSD, ShowCodec, Info, ContextMenu};
    public enum SystemCommands { Suspend, Shutdown }  // option to send system commands to Kodi server, like "System.Suspend"
    public class Input
    {
        public static async Task ExecuteAction(SystemCommands command)
        {
            await ConnectionManager.ExecuteRPCRequest("System." + command.ToString());
        }
        public static async Task ExecuteAction(InputCommands command)
        {           
            await ConnectionManager.ExecuteRPCRequest("Input." + command.ToString());
        }

        public static async void ExecuteAction(string action)
        {
            JObject parameters = new JObject(new JProperty("action", action));
            await ConnectionManager.ExecuteRPCRequest("Input.ExecuteAction", parameters);
        }

        public static async void SendText(string textToSend, bool isDone)
        {
           JObject parameters = new JObject(
               new JProperty("text", textToSend),
               new JProperty("done", isDone));
            await ConnectionManager.ExecuteRPCRequest("Input.SendText", parameters);
        }
    }
}
