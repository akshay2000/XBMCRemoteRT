using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBMCRemoteRT.Models
{
    public class ConnectionItem : NotifyBase
    {
        private int connectionId;
        public int ConnectionId
        {
            get { return connectionId; }
            set { 
                if( connectionId != value)
                {
                    connectionId = value;
                    NotifyPropertyChanged("ConnectionId");
                }
            }
        }

        private string connectionName;
        public string ConnectionName
        {
            get { return connectionName; }
            set
            {
                if (connectionName != value)
                {
                    connectionName = value;
                    NotifyPropertyChanged("ConnectionName");
                }
            }
        }
        

        
        private string ipAddress;
        public string IpAddress
        {
            get { return ipAddress; }
            set {
                if (ipAddress != value)
                {
                    ipAddress = value;
                    NotifyPropertyChanged("IpAddress");
                }
            }
        }

        private int port;
        public int Port
        {
            get { return port; }
            set
            {
                if (port != value)
                {
                    port = value;
                    NotifyPropertyChanged("Port");

                }
            }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set
            {
                if (username != value)
                {
                    username = value;
                    NotifyPropertyChanged("Username");
                }
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    NotifyPropertyChanged("Password");
                }
            }
        }      
    }
}

