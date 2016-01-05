using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using XBMCRemoteRT.Models.Network;

namespace XBMCRemoteRT.Helpers
{
    public class ConnectionManager
    {
        /// <summary>
        /// Class for providing methods and members related to making and maintaining connections to the server.
        /// </summary>
        /// 

        private static int lastRequestID = 0;
        private static SemaphoreSlim rpcSemaphore = new SemaphoreSlim(2);  // Limit the maximum number of simultaneous RPC requests to Kodi

        //consteuctor
        static ConnectionManager()
        {
            JsonRequestQueue.NewRequestQueued += JsonRequestQueue_NewRequestQueued;
        }

        private static async void JsonRequestQueue_NewRequestQueued(object sender, EventArgs e)
        {
            if (rpcSemaphore.Wait(0))
            {
                List<JsonRequest> que = (List<JsonRequest>)sender;

                while (que.Any(r => r.state == JsonRequestState.Waiting))
                { 
                    JArray requestObject = new JArray();
                    List<int> sentRequests = new List<int>();

                    lock (que)
                    {
                        IEnumerable<JsonRequest> newreqs = que.Where(r => r.state == JsonRequestState.Waiting);

                        foreach (JsonRequest r in newreqs)
                        {
                            r.state = JsonRequestState.Sent;
                            requestObject.Add(r.request);
                            sentRequests.Add(r.id);
                        }
                    }

                    string requestData = requestObject.ToString();
                    HttpResponseMessage response = await ExecuteRequest(requestData);
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        JArray responseObject = JArray.Parse(responseString);

                        foreach (JObject resp in responseObject)
                        {
                            int id = (int)resp["id"];
                            JsonRequest req = que.Where(r => r.id == id).FirstOrDefault();
                            if (req != null)
                            {
                                req.state = JsonRequestState.Replied;
                                req.job.SetResult(resp);
                            }
                        }
                    }

                    lock (que)
                        que.RemoveAll(r => sentRequests.Contains(r.id));
                }

                rpcSemaphore.Release();
            }
        }

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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/jsonrpc?request=");
            
            request.Content = new StringContent(requestData, Encoding.UTF8, "application/json");

            if (CurrentConnection.HasCredentials())
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", CurrentConnection.Username, CurrentConnection.Password))));
            HttpResponseMessage response = await CurrentConnection.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            return response;
        }

        private static JObject ConstructRequestObject(string methodName, JObject parameters = null)
        {
            JObject requestObject =
                   new JObject(
                       new JProperty("jsonrpc", "2.0"),
                       new JProperty("id", lastRequestID++),
                       new JProperty("method", methodName));

            if (parameters != null)
                requestObject["params"] = parameters;

            return requestObject;
        }

        public static Task<JObject> ExecuteRPCRequest(string methodName, JObject parameters = null)
        {
            JObject requestObject = ConstructRequestObject(methodName, parameters);

            System.Diagnostics.Debug.WriteLine(requestObject.ToString());

            return JsonRequestQueue.Add(requestObject);
        }

        public static async void ManageSystemTray(bool isActive, string message = "Loading...")
        {
#if WINDOWS_PHONE_APP
            StatusBarProgressIndicator progressIndicator = StatusBar.GetForCurrentView().ProgressIndicator;
            progressIndicator.Text = message;
            if (isActive)
            {
                await progressIndicator.ShowAsync();
            }
            else
            {
                await progressIndicator.HideAsync();
            }
#endif
        }
    }
}
