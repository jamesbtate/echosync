using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace SharedLibrary.Network
{
    public delegate void AcceptCompletedEventHandler(object sender, AcceptCompletedEventArgs e);
    public class AcceptCompletedEventArgs : AsyncCompletedEventArgs { public SslStream Result { get; private set; } }

    public delegate void ReadCompletedEventHandler(object sender, ReadCompletedEventArgs e);
    public class ReadCompletedEventArgs : AsyncCompletedEventArgs { public int Result { get; private set; } }

    public class EchoSyncSocket
    {
        private Socket socket;
        private TcpListener listener;
        private X509Certificate certificate;

        public EchoSyncSocket()
        {

        }

        public void InitServer()
        {
            listener = new TcpListener(IPAddress.Any, Network.SERVER_PORT);
            listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listener.Start(5);
            certificate = X509Certificate.CreateFromCertFile("path");
        }

        public void InitClient()
        {

        }

        public int Read(byte[] buffer, int offset, int count)
        {

            return -1;
        }

        public SslStream Accept()
        {
            TcpClient client = listener.AcceptTcpClient();
            SslStream sslStream = new SslStream(client.GetStream(), false);
            try
            {
                sslStream.AuthenticateAsServer(certificate, false, SslProtocols.Tls, true);
            }
            catch
            {

            }
        }



        public bool AcceptAsync(SocketAsyncEventArgs e)
        {

            return true;
        }
        public event AcceptCompletedEventHandler AcceptCompleted;

        public bool ReadAsync(SocketAsyncEventArgs e)
        {

            return true;
        }
        public event ReadCompletedEventHandler SendCompleted;
        

        public void doStuff()
        {
           
        }
    }
}
