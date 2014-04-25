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
    //We are currently using the Task-based Asynchronous Pattern (TAP)
    //http://msdn.microsoft.com/en-us/library/hh873175.aspx

    /* event-based async
    public delegate void AcceptCompletedEventHandler(object sender, AcceptCompletedEventArgs e);
    public delegate void ReadCompletedEventHandler(object sender, ReadCompletedEventArgs e);
    */

    public class EchoSyncSocket
    {
        private TcpListener listener;
        private TcpClient client;
        private SslStream sslStream;
        private X509Certificate2 certificate;
        private bool serverInitialized = false;
        private readonly string username;
        //the unique user/account identifier
        public string Username
        { get { return username; } }
        private readonly Guid guid;
        //the unique computer/host identifier
        public Guid Guid
        { get { return guid; } }

        /* event-based async
        private SendOrPostCallback acceptCompletedDelegate;
        private HybridDictionary userStateToLifetime = new HybridDictionary(); */

        public EchoSyncSocket()
        {
            //InitializeDelegates();
        }

        public EchoSyncSocket(SslStream sslStream, TcpClient client)
        {
            this.sslStream = sslStream;
            this.client = client;
        }

        /* event-based async
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
        }*/

        public void InitServer()
        {
            Console.WriteLine("InitServer()");
            listener = new TcpListener(IPAddress.Any, Network.SERVER_PORT);
            listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listener.Start(5);
            //need to set "path" below to a real certificate signed by a self-signed CA.
            //certificate = X509Certificate.CreateFromCertFile("..\\..\\..\\server1.echosync.tate2.com.p7b");
            certificate = new X509Certificate2("..\\..\\..\\server1.echosync.tate2.com.pfx", "");
            serverInitialized = true;
        }

        /*public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
        }*/

        public SslStream InitClient(string host, int port)
        {
            client = new TcpClient(host, port);
            sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(Security.ManuallyVerifyCA), null);
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
            Console.WriteLine("Successfully established TLS");
            return sslStream;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return sslStream.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            sslStream.Write(buffer, offset, count);
        }

        public EchoSyncSocket AcceptEchoSyncSocket()
        {
            Console.WriteLine("Accept()");
            if (!serverInitialized) InitServer();
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("New TCP client: " + client.Client.RemoteEndPoint);
            return EchoSyncSocketFromTcpClient(client);
        }

        public TcpClient Accept()
        {
            Console.WriteLine("Accept()");
            if (!serverInitialized) InitServer();
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("New TCP client: " + client.Client.RemoteEndPoint);
            return client;
        }

        public EchoSyncSocket EchoSyncSocketFromTcpClient(TcpClient client)
        {
            Console.WriteLine("EchoSyncSocketFromTcpClient");
            SslStream sslStream = new SslStream(client.GetStream(), false);
            Console.WriteLine("about to auth as server");
            try
            {
                sslStream.AuthenticateAsServer(certificate, false, SslProtocols.Tls, true);
                Console.WriteLine("authed as server. returning ESS.");
                return new EchoSyncSocket(sslStream, client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType() + e.Message);
                if (sslStream != null) sslStream.Close();
                return null;
            }
        }

        public Task<TcpClient> AcceptTask(object state)
        {
            Console.WriteLine("EchoSyncSocket.AcceptTask");
            var tcs = new TaskCompletionSource<TcpClient>();
            listener.BeginAcceptTcpClient(ar =>
            {
                try
                {
                    TcpClient client = listener.EndAcceptTcpClient(ar);
                    Console.WriteLine("New TCP client: " + client.Client.RemoteEndPoint);
                    tcs.SetResult(client);
                }
                catch (Exception exc)
                {
                    tcs.SetException(exc);
                }
            }, state);
            return tcs.Task;
        }

        public Task<EchoSyncSocket> EchoSyncSocketFromTcpClientTask(object state, TcpClient client)
        {
            Console.WriteLine("EchoSyncSocket.EchoSyncSocketFromTcpClientTask");
            var tcs = new TaskCompletionSource<EchoSyncSocket>();
            EchoSyncSocket ess = EchoSyncSocketFromTcpClient(client);
            tcs.SetResult(ess);
            return tcs.Task;
        }

        public EndPoint RemoteEndPoint
        {
            get { return client.Client.RemoteEndPoint; }
        }
        

        /* APM/IAsyncResult Pattern
        public IAsyncResult BeginAccept(AsyncCallback callback, Object state)
        {
            
        }

        public SslStream EndAccept(IAsyncResult result)
        {

        }
        */

        /* event-based async
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
        */

        public void doStuff()
        {
           
        }
    }

    /* event-based async
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
    */
}
