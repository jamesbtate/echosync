using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace SharedLibrary.Network
{
    public enum NetworkEventType
    {
        Ping = 1,
        Pong = 2,
        Notification = 3,
        Request = 4,
        Response = 6,
        Data = 7,
        Hello = 8,
        Goodbye = 9,
    };
    public enum ResponseType { Yes = 1, CanNot = 2, WillNot = 3, NotImplemented = 4 };
    public enum RequestType
    {
        IPAddress = 1,
        Hostname = 2,
        Checksum = 3,
        Authentication = 4,
    };

    /// <summary>
    /// An abstract representation of a network event. Used to provide a base for the definition of push, pull and query requests.
    /// </summary>
    [Serializable()]
    public abstract class NetworkEvent
    {
        //The GUID of the sending node to uniquely identify it
        protected Guid mySourceGuid;
        public Guid SourceGuid
        {
            get { return mySourceGuid; }
            set { mySourceGuid = value; }
        }

        //the type of network event
        protected NetworkEventType MyEventType;
        public NetworkEventType EventType
        {
            get { return MyEventType; }
            set { MyEventType = value; }
        }

        /// <summary>
        /// The attributes (name-value pairs) of the network event
        /// </summary>
        protected ArrayList MyOptions;
        public ArrayList Options
        {
            get { return MyOptions; }
            set { MyOptions = value; }
        }

        //A unique identifier for this conversation. Allows one node to ask another node multiple questions without mixing-up responses
        protected int mySequenceNumber;
        public int SequenceNumber
        {
            get { return mySequenceNumber; }
            set { mySequenceNumber = value; }
        }
    }
}