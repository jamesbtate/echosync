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
                Console.WriteLine("Starting Accept Task...");
                Task<TcpClient> t = serverSocket.AcceptTask(new Object());
                t.ContinueWith(task => AcceptClient(task.Result));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey(false);
        }

        private void AcceptClient(TcpClient client)
        {
            Task<TcpClient> t = serverSocket.AcceptTask(new Object());
            t.ContinueWith(task => AcceptClient(task.Result));
            if (client != null)
            {
                Console.WriteLine("Server.AcceptClient\t" + client.Client.RemoteEndPoint);
                //do something with client
            }
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            //callback code
            Socket handler = ((Socket)ar.AsyncState).EndAccept(ar);
        }
    }
}
