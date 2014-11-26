using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.Models;

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

            HttpClientHandler handler = new HttpClientHandler();
            HttpClient httpClient = new HttpClient(handler);

            string uriString = "http://" + connectionItem.IpAddress + ":" + connectionItem.Port.ToString() + "/jsonrpc?request=";
            httpClient.BaseAddress = new Uri(uriString);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

            request.Content = new StringContent(requestData);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"); //Required to be recognized as valid JSON request.

            if (connectionItem.Password != String.Empty)
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", connectionItem.Username, connectionItem.Password))));

            HttpResponseMessage response = await httpClient.SendAsync(request);
            string responseString = await response.Content.ReadAsStringAsync();
            if (responseString.Length == 0)
                return false;
            dynamic responseObject = JObject.Parse(responseString);
            bool isSuccessful = responseObject.result == "pong";
            return isSuccessful;
        }
    }
}
