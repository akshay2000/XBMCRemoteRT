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
        public string Path { get; set; }

        public string Label { get; set; }

        public string FileType { get; set; }

        public string MimeType { get; set; }

        public string LastModified { get; set; }

        public int Id { get; set; }

        public string Type { get; set; }
    }
}
