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
        public const int SERVER_PORT = 12345;

        public static Socket BindTo(IPAddress address, int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Bind(new IPEndPoint(address, 12345));
            return socket;
        }

        public static Socket BindTo(int port)
        {
            return BindTo(IPAddress.Any, 12345);
        }

        public static bool ConnectTo(Socket client, IPAddress address, int port)
        {
            try
            {
                client.Connect(address, port);
            }
            catch (SocketException)
            {
                return false;
            }
            return true;
        }

        public static NetworkEvent DecodeNetworkEventHeader(byte[] bytes)
        {
            //todo
            NetworkEvent ne = new NetworkEvent();

            return ne;
        }
    }
}
