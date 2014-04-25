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
            /*{
                byte[] address = {127,127,127,127};
                TcpClient client = Network.ConnectTo(new IPAddress(address), 12345);
            }*/
            /*{
                Console.WriteLine("Machine GUID: {0}", Node.GetMachineGUID());
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }*/
            //{
            //    while (true)
            //    {
            //        Console.WriteLine("Select one of the below addresses to connect to:");
            //        Dictionary<string, string> addresses = new Dictionary<string, string>();
            //        addresses.Add("801", "108.39.84.113");
            //        addresses.Add("1014", "70.161.105.143");
            //        addresses.Add("brownie", "128.82.4.29");
            //        addresses.Add("antares", "128.82.4.98");
            //        addresses.Add("netserv", "192.168.0.11");
            //        int i = 0;
            //        foreach (KeyValuePair<string, string> entry in addresses)
            //        {
            //            i++;
            //            Console.WriteLine("{0}. {1}:  {2}", i, entry.Key, entry.Value);
            //        }
            //        ConsoleKeyInfo cki = Console.ReadKey(true);
            //        char c = cki.KeyChar;
            //        int index = Int32.Parse("" + c);
            //        string key = addresses.ElementAt(index - 1).Key;
            //        string val = addresses.ElementAt(index - 1).Value;
            //        Console.WriteLine("You selected {0}: {1}\n", key, val);

            //        Console.WriteLine("Enter a port to connect to:");
            //        string portLine = Console.ReadLine();
            //        int port = Int32.Parse(portLine);

            //        Socket tcp = Network.BindTo(12345);
            //        int plus = 0;
            //        while (true)
            //        {
            //            bool success = Network.ConnectTo(tcp, IPAddress.Parse(val), port);
            //            if (success)
            //            {
            //                Console.WriteLine("connect succeeded. Communicate? Enter 'y' or 'n'.");
            //                char response = Console.ReadKey().KeyChar;

            //                byte[] buffer = new byte[1024];
            //                while (response == 'y')
            //                {
            //                    string line = Console.ReadLine();
            //                    if (line == "" || line == "\n") break;
            //                    tcp.Send(System.Text.Encoding.Unicode.GetBytes(line), 0, line.Length, SocketFlags.None);
            //                    int numRead = tcp.Receive(buffer);
            //                    Console.WriteLine(Encoding.Unicode.GetString(buffer, 0, numRead));
            //                }
            //                break;
            //            }
            //            else
            //            {
            //                Console.WriteLine("connect failed. retrying.");
            //                plus++;
            //            }
            //        }
            //        Console.WriteLine("Press any key to continue...");
            //        Console.ReadKey(true);
            //    }
            //}
            String guid = SharedLibrary.Node.GetMachineGUID();
            String hash = SharedLibrary.Security.HashPasswordBase64(guid);
            bool same = SharedLibrary.Security.VerifyPasswordAgainstHash(guid, hash);
            bool test = SharedLibrary.Security.VerifyPasswordAgainstHash("sdfsdfsdfsfd", hash);
            Console.WriteLine(guid);
            Console.WriteLine(hash);
            Console.WriteLine(same);
            Console.WriteLine(test);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
