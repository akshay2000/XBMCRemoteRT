using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using XBMCRemoteRT.Helpers;
using XBMCRemoteRT.Models.Common;
using XBMCRemoteRT.Models.Files;

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

        public static async Task<FileList> GetDirectory(string directory, string media = "files", Sort sort = null)
        {
            JObject parameters = new JObject(
                                new JProperty("properties",
                                    new JArray("mimetype", "file", "lastmodified")
                                    ));

            parameters["directory"] = directory;
            parameters["media"] = media;

            if (sort != null)
            {
                parameters["sort"] = JObject.FromObject(sort);
            }

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("Files.GetDirectory", parameters);
            JArray fileListObject = (JArray)responseObject["result"]["files"];
            FileList listToReturn = fileListObject != null ? fileListObject.ToObject<FileList>() : new FileList();
            return listToReturn;
        }

        public static async Task<File> GetFileDetails(string file, string media = "files")
        {
            JObject parameters = new JObject(
                                new JProperty("properties",
                                    new JArray("mimetype", "file", "lastmodified")
                                    ));

            parameters["file"] = file;
            parameters["media"] = media;

            JObject responseObject = await ConnectionManager.ExecuteRPCRequest("Files.GetFileDetails", parameters);
            JObject fileDetailsObject = (JObject)responseObject["result"]["filedetails"];
            File fileToReturn = fileDetailsObject != null ? fileDetailsObject.ToObject<File>() : new File();
            return fileToReturn;
        }
    }
}
