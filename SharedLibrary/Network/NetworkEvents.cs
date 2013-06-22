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
    public class NetworkEvent
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
        * type             byte (0-255)                (NetworkEventType)  type=255 reserved for extended types
        * subtype          byte (0-255)                (e.g. ResponseType) subtype=255 reserved for extended suptypes
        * guid             10 bytes                    (SourceGuid)
        * sequence number  unsigned int                (SequenceNumber)
        * num opts         byte (0-255)                number of options. see option format
        * options          (num opts) * (X) bytes      (Options)
        * data length      (uint)*65536 + (ushort)     this formula is identical to a uint48, if such a thing existed. MAX_UINT48 bytes == 256TB
        * data             data length
        */

        /// <summary>
        /// The GUID of the sending node to uniquely identify it
        /// </summary>
        protected Guid mySourceGuid;
        public Guid SourceGuid
        {
            get { return mySourceGuid; }
            set { mySourceGuid = value; }
        }

        /// <summary>
        /// the type of network event
        /// </summary>
        protected NetworkEventType myEventType;
        public NetworkEventType EventType
        {
            get { return myEventType; }
            set { myEventType = value; }
        }

        /// <summary>
        /// The attributes (name-value pairs) of the network event
        /// </summary>
        protected ArrayList myOptions;
        public ArrayList Options
        {
            get { return myOptions; }
            set { lock (myOptions) { myOptions = value; }  }
        }

        /// <summary>
        /// The number of options present in this NetworkEvent.
        /// </summary>
        public int NumOptions
        {
            get
            {
                lock (myOptions)
                {
                    return myOptions.Count;
                }
            }
        }

        /// <summary>
        /// Appends an option to the end of Options
        /// </summary>
        /// <param name="option">The NetworkEventOption to append</param>
        public void AddOption(NetworkEventOption option)
        {
            myOptions.Add(option);
        }

        /// <summary>
        /// A unique identifier for this conversation. Allows one node to ask another node multiple questions without mixing-up responses
        /// </summary>
        protected int mySequenceNumber;
        public int SequenceNumber
        {
            get { return mySequenceNumber; }
            set { mySequenceNumber = value; }
        }

        protected NetworkEventType myType;
        public NetworkEventType Type
        {
            get { return myType; }
            set { myType = value; }
        }

        /// <summary>
        /// The subtype of this NetworkEvent. Some NetworkEventTypes have their own enum for subtypes.
        /// </summary>
        protected byte mySubtype;
        public byte Subtype
        {
            get { return mySubtype; }
            set { mySubtype = value; }
        }
        
        /// <summary>
        /// Length in bytes of the data portion of the NetworkEvent.
        /// </summary>
        protected ulong myDataLength;
        public ulong DataLength
        {
            get { return myDataLength; }
            set { if (value < 1<<48) myDataLength=value; } //do not assign value greater than UINT48_MAX (2**48)
        }

        /// <summary>
        /// Indicates whether the entire data of this NetworkEvent is loaded in RAM (and therefore in the Data member of this object).
        /// See Data member.
        /// </summary>
        protected ulong myDataPresent;
        public ulong DataPresent
        {
            get { return myDataPresent; }
            set { myDataPresent = value; } //do not assign value greater than UINT48_MAX (2**48)
        }

        /// <summary>
        /// The data of this NetworkEvent. This member will not always be populated, particularly when it is a large file (too large for RAM).
        /// Instead, a buffer(s) should be used to manipulate the large byte stream.
        /// </summary>
        private byte[] myData;
        public byte[] Data
        {
            get { return myData; }
            set { myData = value; }
        }
        
    }

    /// <summary>
    /// An option (key_id,key,value tuple), some number of which may be included in a NetworkEvent to store metadata.
    /// </summary>
    [Serializable()]
    public class NetworkEventOption
    {
        /*
         * Option Format
         +--------+--------+--------+--------+--------+--------+--------+--------+
         |key len |     val len     | key id | ......key...... |   ....val....   |
         +--------+--------+--------+--------+--------+--------+--------+--------+
         * key len      byte (0-255)       can be zero (meaning we only care about key id)
         * val len      ushort (0-16535)    can be zero (meaning presence of key is all we care about)
         * key id       byte (0-255)
         **     key id = 0 reserved for "key id unspecified"
         **     key id = 255 reserved for "extended keys" meaning those that don't fit within the remaining 254
         * key          key len     Unicode string
         * val          val len
         * */
        /// <summary>
        /// The one-byte ID number of the key.
        /// 0 is reserved for an "unspecified" key ID, meaning we must interpret the key name field.
        /// 255 is reserved for "extended" keys, that do not fit in the range 1-254.
        /// </summary>
        protected byte myKeyID;
        public byte KeyID
        {
            get { return myKeyID; }
            set { myKeyID = value; }
        }

        /// <summary>
        /// The name of the option. Any valid string of 0-255 characters is a valid name.
        /// </summary>
        protected string myKey;
        public string Key
        {
            get { return myKey; }
            set { myKey = value; }
        }

        protected byte[] myValue;
        public byte[] Value
        {
            get { return myValue; }
            set { myValue = value; }
        }

        /// <summary>
        /// Returns the length of the key name. Not yet tested to confirm length==bytesize.
        /// </summary>
        public byte KeyLength { get { return (byte)myKey.Length; } }

        /// <summary>
        /// Returns the length of the value. Not yet tested to confirm length==bytesize.
        /// </summary>
        public ushort ValueLength { get { return (ushort)myValue.Length; } }

        /// <summary>
        /// Returns the length of the entire option, as sent on the network.
        /// </summary>
        public uint Length { get { return 4 + (uint)KeyLength + ValueLength; } }
    }
}