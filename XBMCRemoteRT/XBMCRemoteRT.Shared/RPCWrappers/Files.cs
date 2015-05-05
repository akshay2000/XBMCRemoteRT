using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.Models.Common;
using XBMCRemoteRT.Models.Files;

namespace XBMCRemoteRT.RPCWrappers
{
    public class Files
    {
        public static Task<List<File>> GetDirectory(string directory, string media = "files", Sort sort = null)
        {
            throw new NotImplementedException();
        }
    }
}
