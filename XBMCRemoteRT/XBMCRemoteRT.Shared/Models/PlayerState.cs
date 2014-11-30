using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Models
{
    public class PlayerState : NotifyBase
    {
        public PlayerState()
        {
            CurrentPlayerItem = new PlayerItem();
            CurrentPlayerProperties = new PlayerProperties();
            PlayerType = Players.None;
        }

        private PlayerItem currentPlayerItem;
        public PlayerItem CurrentPlayerItem
        {
            get { return currentPlayerItem; }
            set
            {
                if (currentPlayerItem !=value)
                {
                    currentPlayerItem = value;
                    NotifyPropertyChanged("CurrentPlayerItem");
                }
            }
        }

        private PlayerProperties currentPlayerProperties;
        public PlayerProperties CurrentPlayerProperties
        {
            get { return currentPlayerProperties; }
            set
            {
                if (currentPlayerProperties != value)
                {
                    currentPlayerProperties = value;
                    NotifyPropertyChanged("CurrentPlayerProperties");
                }
            }
        }

        public Players PlayerType { get; set; }
    }
}
