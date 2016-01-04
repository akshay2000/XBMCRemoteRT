using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XBMCRemoteRT.Helpers
{
    class JsonRequestQueue
    {
        private static List<JsonRequest> que = new List<JsonRequest>();
        public static event EventHandler NewRequestQueued;

        public static Task<JObject> Add(JObject request)
        {
            int id = (int)request["id"];
            JsonRequest req = new JsonRequest();
            req.request = request;
            req.id = id;
            req.state = JsonRequestState.Waiting;
            req.job = new TaskCompletionSource<JObject>();
            
            lock(que)
            {
                que.Add(req);
            }

            if (NewRequestQueued != null)
                NewRequestQueued(que, null);

            return req.job.Task;
        }
    }

    class JsonRequest
    {
        public JObject request;
        public int id;
        public TaskCompletionSource<JObject> job = null;
        public JsonRequestState state = JsonRequestState.Unknown;
    }

    enum JsonRequestState
    {
        Unknown = 0,
        Waiting,
        Sent,
        Replied
    }
}
