using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using XBMCRemoteRT.Models;

namespace XBMCRemoteRT.ViewModels
{
    public class ConnectionsViewModel : NotifyBase
    {

        public ConnectionsViewModel()
        {
            LoadConnections();
        }

        private ObservableCollection<ConnectionItem> connectionItems;

        public ObservableCollection<ConnectionItem> ConnectionItems
        {
            get { return connectionItems; }
            set {
                if (connectionItems != value)
                {
                    connectionItems = value;
                    NotifyPropertyChanged("ConnectionItems");
                }
            }
        }

        public void LoadConnections()
        {
            if (ConnectionItems == null)
            {
                ConnectionItems = new ObservableCollection<ConnectionItem>();
            }
            ConnectionItems.Add(new ConnectionItem() { ConnectionId = 1, ConnectionName = "KalEl", IpAddress = "10.0.0.3", Port = 8080 });
        }

        public void AddConnectionItem(ConnectionItem itemToAdd)
        {
            ConnectionItems.Add(itemToAdd);
        }

        public void RemoveConnectionItem(ConnectionItem itemToRemove)
        {           
            ConnectionItems.Remove(itemToRemove);
        }

        public void UpdateConnectionItem(ConnectionItem itemToUpdate)
        {
            
        }
        
    }
}
