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

namespace Server
{
    class Server
    {
        public const int ServerPort = 17777;
        static void Main(string[] args)
        {
            Console.WriteLine("=== Running EchoSync Server ===");
            try{

                Socket serverSocket = Network.BindTo(ServerPort);
                serverSocket.Listen(5);
                while (true)
                {
                    serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), serverSocket);
                    
                }
            }
            catch (Exception e)
            {

            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            //callback code
            Socket handler = ((Socket)ar.AsyncState).EndAccept(ar);
        }
    }
}
