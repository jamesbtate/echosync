using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary.Networking;
using System.Net;
using System.Net.Sockets;

namespace TestDriver
{
    class TestDriver
    {
        static void Main(string[] args)
        {
            byte[] address = {127,127,127,127};
            TcpClient client = NetworkFunctions.ConnectTo(new IPAddress(address), 12345);
        }
    }
}
