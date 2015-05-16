using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models.Common;
using XBMCRemoteRT.Models.Files;
using XBMCRemoteRT.Models.Video;

namespace XBMCRemoteRT.RPCWrappers
{
    public class Files
    {
        public static async Task<List<Source>> GetSources(string media = "files", Limits limits = null, Sort sort = null)
        {
            JObject parameters = new JObject();

            parameters["media"] = media;

            if (limits != null)
            {
                parameters["limits"] = JObject.FromObject(limits);
            }

            if (sort != null)
            {
                parameters["sort"] = JObject.FromObject(sort);
            }

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("Files.GetSources", parameters);
            JArray fileListObject = (JArray)responseObject["result"]["sources"];
            List<Source> listToReturn = fileListObject != null ? fileListObject.ToObject<List<Source>>() : new List<Source>();
            return listToReturn;
        }

        public static async Task<List<File>> GetDirectory(string directory, string media = "files", Sort sort = null)
        {
            JObject parameters = new JObject(
                                new JProperty("properties",
                                    new JArray("size", "mimetype", "file", "lastmodified")
                                    ));

            parameters["directory"] = directory;
            parameters["media"] = media;

            if (sort != null)
            {
                parameters["sort"] = JObject.FromObject(sort);
            }

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("Files.GetDirectory", parameters);
            JArray fileListObject = (JArray)responseObject["result"]["files"];
            List<File> listToReturn = fileListObject != null ? fileListObject.ToObject<List<File>>() : new List<File>();
            return listToReturn;
        }

        public static async Task<File> GetFileDetails(string file, string media = "files")
        {
            JObject parameters = new JObject(
                                new JProperty("properties",
                                    new JArray("filetype", "size", "mimetype", "file", "lastmodified")
                                    ));

            parameters["file"] = file;

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("Files.GetFileDetails", parameters);
            JArray fileDetailsObject = (JArray)responseObject["result"]["filedetails"];
            File fileToReturn = fileDetailsObject != null ? fileDetailsObject.ToObject<File>() : new File();
            return fileToReturn;
        }
    }
}
