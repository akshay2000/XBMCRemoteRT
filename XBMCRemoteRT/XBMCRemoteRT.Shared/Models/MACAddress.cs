using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XBMCRemoteRT.Models
{
    public class MacAddress
    {
        private const string PATTERN = "([\\da-fA-F]{2}:){5}[\\da-fA-F]{2}";
        private static Regex REGEX = new Regex(PATTERN);

        public byte[] Bytes { get; set; }

        /// <summary>
        /// Tries to parse a MAC address from a string. A semikolon (":") has to be used as the byte's delimiter.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns></returns>
        public static bool TryParse(string input, out MacAddress mac)
        {
            mac = null;
            if (!REGEX.IsMatch(input))
            {
                return false;
            }

            string[] byteStrs = input.Split(':');
            bool[] parseResults = new bool[byteStrs.Length];
            byte[] macBytes = new byte[byteStrs.Length];

            for (int i = 0; i < byteStrs.Length; i++)
            {
                parseResults[i] = Byte.TryParse(byteStrs[i], System.Globalization.NumberStyles.HexNumber, null, out macBytes[i]);
            }

            bool parsedSuccessfully = parseResults.All((b) => { return b; });
            if (parsedSuccessfully)
            {
                mac = new MacAddress();
                mac.Bytes = macBytes;
            }

            return parsedSuccessfully;
        }
    }
}
