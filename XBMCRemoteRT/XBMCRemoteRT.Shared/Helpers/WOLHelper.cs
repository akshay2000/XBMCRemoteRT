using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using XBMCRemoteRT.Models;
namespace XBMCRemoteRT.Helpers
{
    public class WOLHelper
    {
        private const string SERVICE_DISCARD = "discard";
        /// <summary>
        /// Tries to wake a machine by sending a magic package with the
        /// corresponding MAC address to the broadcast address on the
        /// discard port.
        /// </summary>
        /// <param name="server">The machine to wake, MAC address needs to be given.</param>
        /// <returns></returns>
        public static async Task<uint> WakeUp(ConnectionItem server)
        {
            byte[] magicPackage = CreateMagicPackage(server.MACAddress);

            IPAddress serverIP;
            if (!IPAddress.TryParse(server.IpAddress, out serverIP))
                return 10;

            IPAddress broadcastIP = GetBroadcastIP(serverIP, server.SubnetMask);
            HostName target = new HostName(broadcastIP.ToString());
            using (var socket = new DatagramSocket())
            {
                await socket.ConnectAsync(target, SERVICE_DISCARD);
                
                DataWriter writer = new DataWriter(socket.OutputStream);

                writer.WriteBytes(magicPackage);
                uint response = await writer.StoreAsync();

                return response;
            }
        }

        /// <summary>
        /// Computes the broadcast IP address of a given IP address
        /// with its net mask.
        /// </summary>
        /// <param name="ip">The client IP</param>
        /// <param name="mask">The client IP's subnet mask</param>
        /// <returns>The broadcast IP</returns>
        private static IPAddress GetBroadcastIP(IPAddress ip, IPAddress mask)
        {
            if (ip == null || ip.Bytes == null)
                throw new ArgumentNullException("ip");
            if (mask == null || mask.Bytes == null)
                throw new ArgumentNullException("mask");

            int broadcastInt =  BitConverter.ToInt32(ip.Bytes, 0) | ~(BitConverter.ToInt32(mask.Bytes, 0));
            IPAddress broadcastIP = new IPAddress();
            broadcastIP.Bytes = BitConverter.GetBytes(broadcastInt);

            return broadcastIP;
        }

        /// <summary>
        /// Creates a magic package for the target computer's given MAC address.
        /// </summary>
        /// <param name="macAddress">MAC address of the computer to wake up</param>
        /// <returns>The magic package for waking the computer up.</returns>
        private static byte[] CreateMagicPackage(MacAddress macAddress)
        {
            if (macAddress == null || macAddress.Bytes == null)
                throw new ArgumentNullException("macAddress");
            if (macAddress.Bytes.Length != 6)
                throw new ArgumentException("macAddress must have length of 6 bytes");

            byte[] package = new byte[102];

            // 6 bytes of 0xFF
            for (int i = 0; i < 6; i++)
            {
                package[i] = 0xFF;
            }

            // sixteen repititions of MAC address
            for (int i = 1; i < 17; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    package[i * 6 + j] = macAddress.Bytes[j];
                }
            }

            return package;
        }
    }
}
