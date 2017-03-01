﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using XBMCRemoteRT.Models;
using XBMCRemoteRT.Models.Network;

namespace XBMCRemoteRT.ViewModels
{
    public class ConnectionsViewModel : NotifyBase
    {
        StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
        StorageFile connections;

        public ConnectionsViewModel()
        {
            
        }

        private ObservableCollection<ConnectionItem> connectionItems;

        public ObservableCollection<ConnectionItem> ConnectionItems
        {
            get { return connectionItems; }
            set
            {
                if (connectionItems != value)
                {
                    connectionItems = value;
                    NotifyPropertyChanged("ConnectionItems");
                }
            }
        }

        public async Task ReloadConnections()
        {
            if (ConnectionItems == null)
            {
                ConnectionItems = new ObservableCollection<ConnectionItem>();
            }
            connections = await roamingFolder.CreateFileAsync("connections.json", CreationCollisionOption.OpenIfExists);
            string connectionsJsonString = await FileIO.ReadTextAsync(connections);
            if (connectionsJsonString == String.Empty)
            {
                connectionsJsonString = "[]";
            }
            JArray connectionsArray = JArray.Parse(connectionsJsonString);
            var t = connectionsArray.ToObject<List<ConnectionItem>>();
            ConnectionItems.Clear();
            foreach (var item in t)
            {
                ConnectionItems.Add(item);
            }
        }

        public async Task AddConnectionItem(ConnectionItem itemToAdd)
        {
            ConnectionItems.Add(itemToAdd);
            await SaveConnections();
        }

        public async void RemoveConnectionItem(ConnectionItem itemToRemove)
        {           
            ConnectionItems.Remove(itemToRemove);
            await SaveConnections();
        }

        public async void UpdateConnectionItem()
        {
            await SaveConnections();
        }

        private async Task SaveConnections()
        {
            connections = await roamingFolder.CreateFileAsync("connections.json", CreationCollisionOption.OpenIfExists);
            string jsonString = JArray.FromObject(ConnectionItems).ToString();
            await FileIO.WriteTextAsync(connections, jsonString);
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        //private void NotifyPropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
