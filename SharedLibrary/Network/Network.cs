using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace SharedLibrary.Network
{
    public class Network
    {
        public static TcpClient ConnectTo(IPAddress address, int port)
        {
            TcpClient client = new TcpClient();
            client.Connect(address, port);
            return client;
        }

        public static NetworkEvent DecodeNetworkEventHeader(byte[] bytes)
        {
            NetworkEvent ne = new NetworkEvent();

            return ne;
        }
    }
}
