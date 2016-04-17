using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.Models.Network;

namespace XBMCRemoteRT.RPCWrappers
{
    public class JSONRPC
    {
        public async static Task<bool> Ping(ConnectionItem connectionItem)
        {
            JObject requestObject = new JObject(
                new JProperty("jsonrpc", "2.0"),
                new JProperty("id", 234),
                new JProperty("method", "JSONRPC.ping"));

            string requestData = requestObject.ToString();

           // httpClient.Timeout = new TimeSpan(0, 0, 2);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/jsonrpc?request=");

            request.Content = new StringContent(requestData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await connectionItem.HttpClient.SendAsync(request);
            string responseString = await response.Content.ReadAsStringAsync();
            if (responseString.Length == 0)
                return false;
            //dynamic responseObject = JObject.Parse(responseString);
            JObject responseObject = JObject.Parse(responseString);
            bool isSuccessful = responseObject["result"].ToString() == "pong";
            return isSuccessful;
        }
    }
}
