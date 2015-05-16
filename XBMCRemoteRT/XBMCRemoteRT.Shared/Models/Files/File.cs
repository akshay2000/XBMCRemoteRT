using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;

namespace XBMCRemoteRT.Models.Files
{
    public class File : FileBase
    {
        public string FileType { get; set; }

        public string MimeType { get; set; }

        public string LastModified { get; set; }
    }
}
