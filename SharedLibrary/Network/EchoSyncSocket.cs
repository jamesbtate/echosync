using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace SharedLibrary.Network
{
    public delegate void AcceptCompletedEventHandler(object sender, AcceptCompletedEventArgs e);

    public delegate void ReadCompletedEventHandler(object sender, ReadCompletedEventArgs e);

    public class EchoSyncSocket
    {
        private TcpListener listener;
        private TcpClient client;
        private X509Certificate certificate;
        private bool serverInitialized = false;

        private SendOrPostCallback acceptCompletedDelegate;

        private HybridDictionary userStateToLifetime = new HybridDictionary();

        public EchoSyncSocket()
        {
            InitializeDelegates();
        }

        protected virtual void InitializeDelegates()
        {
            acceptCompletedDelegate = new SendOrPostCallback(AcceptCompletedGateway);
        }

        private bool TaskCancelled(object taskId)
        {
            return (userStateToLifetime[taskId] == null);
        }

        private delegate void AcceptEventHandler(AsyncOperation asyncOp);

        public void CancelAsync(object taskId)
        {
            AsyncOperation asyncOp = userStateToLifetime[taskId] as AsyncOperation;
            if (asyncOp != null)
            {
                lock (userStateToLifetime.SyncRoot)
                {
                    userStateToLifetime.Remove(taskId);
                }
            }
        }

        public void InitServer()
        {
            listener = new TcpListener(IPAddress.Any, Network.SERVER_PORT);
            listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listener.Start(5);
            //need to set "path" below to a real certificate signed by a self-signed CA.
            certificate = X509Certificate.CreateFromCertFile("path");
            serverInitialized = true;
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
        }

        public SslStream InitClient(string host, int port)
        {
            client = new TcpClient(host, port);
            //need to figure out how to manually set the CA for the next line so that I can trust my self-signed CA without adding certs to the machine store.
            SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            try
            {
                sslStream.AuthenticateAsClient(host);
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                client.Close();
                return null;
            }
            return sslStream;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return -1;
        }

        public SslStream Accept()
        {
            if (!serverInitialized) InitServer();
            TcpClient client = listener.AcceptTcpClient();
            SslStream sslStream = new SslStream(client.GetStream(), false);
            try
            {
                sslStream.AuthenticateAsServer(certificate, false, SslProtocols.Tls, true);
                return sslStream;
            }
            catch
            {
                return null;
            }
        }

        public virtual void AcceptAsync(object taskId)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(taskId);
            lock (userStateToLifetime.SyncRoot)
            {
                if (userStateToLifetime.Contains(taskId))
                {
                    throw new ArgumentException("Task ID paramter must be unique", "taskId");
                }
                userStateToLifetime[taskId] = asyncOp;
            }
            AcceptEventHandler acceptDelegate = new AcceptEventHandler(AcceptWorker);
            acceptDelegate.BeginInvoke(asyncOp, null, null);
        }

        public event AcceptCompletedEventHandler AcceptCompleted;

        private void AcceptCompletedGateway(object operationState)
        {
            AcceptCompletedEventArgs e = operationState as AcceptCompletedEventArgs;
            OnAcceptCompleted(e);
        }

        protected void OnAcceptCompleted(AcceptCompletedEventArgs e)
        {
            if (AcceptCompleted != null)
            {
                AcceptCompleted(this, e);
            }
        }

        private void AcceptCompletionMethod(SslStream clientSslStream, Exception exception, bool cancelled, AsyncOperation asyncOp)
        {
            if (!cancelled)
            {
                lock (userStateToLifetime.SyncRoot)
                {
                    userStateToLifetime.Remove(asyncOp.UserSuppliedState);
                }
            }
            AcceptCompletedEventArgs e = new AcceptCompletedEventArgs(clientSslStream, exception, cancelled, asyncOp.UserSuppliedState);
            asyncOp.PostOperationCompleted(acceptCompletedDelegate, e);
        }

        private void AcceptWorker(AsyncOperation asyncOp)
        {
            Exception e = null;
            SslStream c = null;
            if (!TaskCancelled(asyncOp.UserSuppliedState))
            {
                try
                {
                    c = Accept();
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }
            this.AcceptCompletionMethod(c, e, TaskCancelled(asyncOp.UserSuppliedState), asyncOp);
        }

        //public event ReadCompletedEventHandler ReadCompleted;
        

        public void doStuff()
        {
           
        }
    }

    public class AcceptCompletedEventArgs : AsyncCompletedEventArgs
    {
        private SslStream clientSslStream;
        public AcceptCompletedEventArgs(SslStream clientSslStream, Exception e, bool cancelled, object state)
            : base(e, cancelled, state)
        {
            this.clientSslStream = clientSslStream;
        }
        public SslStream Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return clientSslStream;
            }
        }
    }

    public class ReadCompletedEventArgs : AsyncCompletedEventArgs
    {
        private int numRead;
        private byte[] bytes;
        public ReadCompletedEventArgs(int numRead, byte[] bytes, Exception e, bool cancelled, object state)
            : base(e, cancelled, state)
        {
            this.numRead = numRead;
            this.bytes = bytes;
        }
        public int Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return numRead;
            }
        }
    }
}
