using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;

namespace XBMCRemoteRT.Models.Files
{
    public class File
    {
        [JsonProperty(PropertyName = "file")]
        public string Name { get; set; }

        public string FileType { get; set; }

        public int Size { get; set; }

        public string MimeType { get; set; }

        public string LastModified { get; set; }
    }
}
