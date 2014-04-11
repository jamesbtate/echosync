using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using SharedLibrary.Network;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        public const int ServerPort = 17777;
        private EchoSyncSocket serverSocket;
        static void Main(string[] args)
        {
            Console.WriteLine("=== Running EchoSync Server ===");
            Server s = new Server();
           
        }

        public Server()
        {
            try
            {
                serverSocket = new EchoSyncSocket();
                serverSocket.InitServer();
                AcceptClient();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey(false);
        }

        private void AcceptClient()
        {
            Task<TcpClient> t = serverSocket.AcceptTask(new Object());
            t.ContinueWith(task => AcceptClient());
            AcceptedClient(t.Result);
        }

        private void AcceptedClient(TcpClient client)
        {
            Console.WriteLine("Server.AcceptClient\t" + client.Client.RemoteEndPoint);
            EchoSyncSocket ess = serverSocket.EchoSyncSocketFromTcpClient(client);
            Console.WriteLine("AcceptedClient77");
            if (ess == null)
            {
                Console.WriteLine("failed to auth as server");
                return;
            }
            Console.WriteLine("Authed as server to client: " + ess.RemoteEndPoint);
        }
    }
}
