using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary;
using SharedLibrary.Network;
using System.Net;
using System.Net.Sockets;

namespace TestDriver
{
    class TestDriver
    {
        static void Main(string[] args)
        {
            //byte[] address = {127,127,127,127};
            //TcpClient client = Network.ConnectTo(new IPAddress(address), 12345);
            Console.WriteLine("Machine GUID: {0}", Node.GetMachineGUID());
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
