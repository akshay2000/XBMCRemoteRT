using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.Models;

namespace XBMCRemoteRT.Helpers
{
    public class ConnectionManager
    {
        /// <summary>
        /// Class for providing methods and members related to making and maintaining connections to the server.
        /// An object is created from this class at the app initiation time (see App.xaml.cs) and used universally across the app.
        /// </summary>
        /// 

        //consteuctor
        public ConnectionManager()
        { }

        private static ConnectionItem _currentConnection;
        public static ConnectionItem CurrentConnection
        {
            get 
            {
                return _currentConnection; 
            }
            set
            {
                if (_currentConnection != value)
                {
                    _currentConnection = value;
                }
            }
        }        

        private static async Task<HttpResponseMessage> ExecuteRequest(string requestData)
        {
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient httpClient = new HttpClient(handler);
            string uriString = "http://" + CurrentConnection.IpAddress + ":" + CurrentConnection.Port.ToString() + "/jsonrpc?request=";
            httpClient.BaseAddress = new Uri(uriString);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");

            request.Content = new StringContent(requestData);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"); //Required to be recognized as valid JSON request.

            if (CurrentConnection.Password != String.Empty)
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", CurrentConnection.Username, CurrentConnection.Password))));
            
            HttpResponseMessage response = await httpClient.SendAsync(request);
            return response;            
        }

        private static JObject ConstructRequestObject(string methodName, JObject parameters = null)
        {
            JObject requestObject =
                   new JObject(
                       new JProperty("jsonrpc", "2.0"),
                       new JProperty("id", 234),
                       new JProperty("method", methodName));

            if (parameters != null)
                requestObject["params"] = parameters;

            return requestObject;
        }

        public static async Task<JObject> ExecuteRPCRequest(string methodName, JObject parameters = null)
        {
            JObject requestObject = ConstructRequestObject(methodName, parameters);
            string requestData = requestObject.ToString();
            HttpResponseMessage response = await ConnectionManager.ExecuteRequest(requestData);
            string responseString = await response.Content.ReadAsStringAsync();
            JObject responseObject = JObject.Parse(responseString);
            return responseObject;
        }

        public static bool IsIpAddressValid(string ipAddress)
        {
            bool isValid = true;
            List<string> fragments = ipAddress.Split('.').ToList<string>();
            if (fragments.Count != 4)
                return false;
            foreach (var t in fragments)
            {
                int fragment;
                bool isFragmentInt = int.TryParse(t, out fragment);
                if (!isFragmentInt || (fragment > 255 || fragment < 0))
                    return false;
            }
            return isValid;
        }
    }
}
