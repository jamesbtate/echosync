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
    /*
    NetworkEvent Format
    +--------+--------+--------+--------+--------+--------+--------+--------+
    |  type  |subtype |            fisrt six bytes of GUID                  |
    +--------+--------+--------+--------+--------+--------+--------+--------+
    |  last four bytes of GUID          |        sequence number            |
    +--------+--------+--------+--------+--------+--------+--------+--------+
    |num opts|  ......................options.............................  |
    +--------+--------+--------+--------+--------+--------+--------+--------+
    |                      data length  '                 |   ....data....  |
    +--------+--------+--------+--------+--------+--------+--------+--------+
     * type         unsigned char (0-255)       (NetworkEventType)
     * subtype      unsigned char (0-255)       (e.g. ResponseType)
     * guid         10 bytes                    (SourceGuid)
     * sequence     number  unsigned int        (SequenceNumber)
     * num opts     unsigned char (0-255)       number of options. see option format
     * options      (num opts) * (X) bytes      (Options)
     * data length  (uint)*65536 + (ushort)     this formula is identical to a uint48, if such a thing existed
     * data         data length
     Option Format
     +--------+--------+--------+--------+--------+--------+--------+--------+
     |key len |val len | ......key...... |        ........val........        |
     +--------+--------+--------+--------+--------+--------+--------+--------+
     * key len  uint (0-255)
     * val len  uint (0-255)
     * key      key len
     * val      val len
     * */
    public class Network
    {
        public static TcpClient ConnectTo(IPAddress address, int port)
        {
            TcpClient client = new TcpClient();
            client.Connect(address, port);
            return client;
        }
    }
}
