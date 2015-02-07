using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XBMCRemoteRT.Models.Network
{
    public class IPAddress
    {
        private const string PATTERN = "(\\d{1,3}\\.){3}\\d{1,3}";
        private static Regex REGEX = new Regex(PATTERN);

        public byte[] Bytes { get; set; }

        public static bool TryParse(string input, out IPAddress ip)
        {
            ip = null;

            if (!REGEX.IsMatch(input))
            {
                return false;
            }

            string[] byteStrs = input.Split('.');
            bool[] parseResults = new bool[byteStrs.Length];
            byte[] ipBytes = new byte[byteStrs.Length];

            for (int i = 0; i < byteStrs.Length; i++)
            {
                parseResults[i] = Byte.TryParse(byteStrs[i], out ipBytes[i]);
            }

            bool parsedSuccessfully = parseResults.All((b) => { return b; });
            if (parsedSuccessfully)
            {
                ip = new IPAddress();
                ip.Bytes = ipBytes;
            }

            return parsedSuccessfully;
        }

        public override string ToString()
        {
            return String.Join(".", Bytes);
        }
    }
}
