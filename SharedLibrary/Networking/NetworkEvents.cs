using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace SharedLibrary
{
    public enum NetworkEventType { Hello, Push, Pull, Query };
    public enum ResponseType { Yes, CanNot, WillNot, NotImplemented };
    public enum QueryType
    {
        IPAddress, Hostname, MACAddress, FreeSpace,
        TotalSpace, BackupSpace, NonBackupSpace
    };

    /// <summary>
    /// An abstract representation of a network event. Used to provide a base for the definition of push, pull and query requests.
    /// </summary>
    [Serializable()]
    public abstract class NetworkEvent
    {
        protected IPAddress mySourceIPAddress;
        public IPAddress SourceIPAddress
        {
            get { return mySourceIPAddress; }
            set { mySourceIPAddress = value; }
        }

        /*protected PhysicalAddress mySourceMacAddress;
        public PhysicalAddress SourceMacAddress
        {
            get { return mySourceMacAddress; }
            set { mySourceMacAddress = value; }
        }*/

        //The GUID of the sending node to uniquely identify it
        protected Guid mySourceGuid;
        public Guid SourceGuid
        {
            get { return mySourceGuid; }
            set { mySourceGuid = value; }
        }

        //the type of network event. See functional requirements 6.1.4 and 6.2.1.
        //Using inheritance instead of 
        //protected NetworkEventType eventType;

        //A unique identifier for this conversation. Allows one node to ask another node multiple questions without mixing-up responses
        protected int mySequenceNumber;
        public int SequenceNumber
        {
            get { return mySequenceNumber; }
            set { mySequenceNumber = value; }
        }
    }

    /// <summary>
    /// Abstract class with base code for network requests.
    /// </summary>
    [Serializable()]
    public abstract class NetworkRequest : NetworkEvent
    {
        /*private TcpClient myTcpClient;
        public TcpClient TcpClient
        {
            get { return myTcpClient; }
            set { myTcpClient = value; }
        }*/

        public NetworkRequest(IPAddress ipAddress, /*PhysicalAddress macAddress,*/ Guid guid, int sequenceNumber)
        {
            mySourceIPAddress = ipAddress;
            /*mySourceMacAddress = macAddress;*/
            mySourceGuid = guid;
            mySequenceNumber = sequenceNumber;
        }

        public NetworkRequest()
        {
            //mySourceIPAddress = Node.GetIPAddress();
            mySequenceNumber = new Random().Next();
            //mySourceGuid = Node.GetGuid();
        }
    }

    /// <summary>
    /// Represents a push request to store a backup file.
    /// </summary>
    [Serializable()]
    public class PushRequest : NetworkRequest
    {
        public PushRequest(IPAddress ipAddress, /*PhysicalAddress macAddress,*/ Guid guid, int sequenceNumber)
            : base(ipAddress, /*macAddress,*/ guid, sequenceNumber)
        {
        }

        // The id of the backup this file is a part of
        private long myBackupNumber;
        public long BackupNumber
        {
            get { return myBackupNumber; }
            set { myBackupNumber = value; }
        }

        // The chunk ID of this backup file
        private long myChunkNumber;
        public long ChunkNumber
        {
            get { return myChunkNumber; }
            set { myChunkNumber = value; }
        }

        // The size of the backup file
        private long myFileSize;
        public long FileSize
        {
            get { return myFileSize; }
            set { myFileSize = value; }
        }

        // The path to the backup file on the requesting node
        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }
    }

    /// <summary>
    /// Represents a pull request to recover a backup file or download some other file from the remote computer.
    /// </summary>
    [Serializable()]
    public class PullRequest : NetworkRequest
    {
        // The id of the backup this file is a part of
        private long myBackupNumber;
        public long BackupNumber
        {
            get { return myBackupNumber; }
            set { myBackupNumber = value; }
        }

        // The chunk ID of this backup file
        private long myChunkNumber;
        public long ChunkNumber
        {
            get { return myChunkNumber; }
            set { myChunkNumber = value; }
        }

        // The size of the backup file
        private long myFileSize;
        public long FileSize
        {
            get { return myFileSize; }
            set { myFileSize = value; }
        }

        // The path where the backup file should be restored to on the requesting node.
        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public PullRequest(IPAddress ipAddress, /*PhysicalAddress macAddress,*/ Guid guid, int sequenceNumber)
            : base(ipAddress, /*macAddress,*/ guid, sequenceNumber)
        {
        }
    }

    /// <summary>
    /// Represents a request to get information from another node.
    /// </summary>
    [Serializable()]
    public class QueryRequest : NetworkRequest
    {
        // The name of the data we want, e.g. "CPU usage." The names need to be stadardized somewhere, however.
        private QueryType queryType;
        public QueryType QueryType
        {
            get { return queryType; }
            set { queryType = value; }
        }

        public QueryRequest(IPAddress ipAddress, /*PhysicalAddress macAddress,*/ Guid guid, int sequenceNumber)
            : base(ipAddress, /*macAddress,*/ guid, sequenceNumber)
        {
        }
    }

    /// <summary>
    /// Represents a request to push an updated SQLite DB to another node.
    /// </summary>
    [Serializable()]
    public class PushIndexRequest : NetworkRequest
    {
        // The size of the SQLite DB in bytes
        private long dbSize;
        public long DBSize
        {
            get { return dbSize; }
            set { dbSize = value; }
        }

        // The path to the DB file that will be transmitted to another node
        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public PushIndexRequest(string path, long size)
            : base()
        {
            this.path = path;
            this.dbSize = size;
        }
    }

    /// <summary>
    /// Class which represents a response to a network response. Includes a response type and a reason string.
    /// </summary>
    [Serializable()]
    public class NetworkResponse : NetworkEvent
    {
        private ResponseType type;
        public ResponseType Type
        {
            get { return type; }
            set { type = value; }
        }

        //A string containing any details about the response type. For successful query responses, this string will contain the requested information.
        private string reason;
        public string Reason
        {
            get { return reason; }
            set { reason = value; }
        }

        public NetworkResponse(ResponseType type, string reason, Guid guid, int seqNum)
        {
            this.type = type;
            this.reason = reason;
            this.mySourceGuid = guid;
            this.mySequenceNumber = seqNum;
        }
    }
}
