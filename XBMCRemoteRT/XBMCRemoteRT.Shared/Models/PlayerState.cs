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
            PlayerType = Players.None;
        }

        #region PLAYER ITEMS

        private string thumbnail;
        public string Thumbnail
        {
            get { return thumbnail; }
            set
            {
                if (thumbnail != value)
                {
                    thumbnail = value;
                    NotifyPropertyChanged("Thumbnail");
                }
            }
        }

        private string fanart;
        public string Fanart
        {
            get { return fanart; }
            set
            {
                if (fanart != value)
                {
                    fanart = value;
                    NotifyPropertyChanged("Fanart");
                }
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string showTitle;
        public string ShowTitle
        {
            get { return showTitle; }
            set
            {
                if (showTitle != value)
                {
                    showTitle = value;
                    NotifyPropertyChanged("ShowTitle");
                }
            }
        }

        private string tagline;
        public string Tagline
        {
            get { return tagline; }
            set
            {
                if (tagline != value)
                {
                    tagline = value;
                    NotifyPropertyChanged("Tagline");
                }
            }
        }

        private List<string> artist;
        public List<string> Artist
        {
            get { return artist; }
            set
            {
                if (artist != value)
                {
                    artist = value;
                    NotifyPropertyChanged("Artist");
                }
            }
        }

        #endregion


        #region PLAYER PROPERTIES

        private int speed;
        public int Speed
        {
            get { return speed; }
            set
            {
                if (speed != value)
                {
                    speed = value;
                    NotifyPropertyChanged("Speed");
                }
            }
        }

        private int timeSeconds;
        public int TimeSeconds
        {
            get { return timeSeconds; }
            set
            {
                if (timeSeconds != value)
                {
                    timeSeconds = value;
                    NotifyPropertyChanged("TimeSeconds");
                }
            }
        }

        private int totalTimeSeconds;
        public int TotalTimeSeconds
        {
            get { return totalTimeSeconds; }
            set
            {
                if (totalTimeSeconds != value)
                {
                    totalTimeSeconds = value;
                    NotifyPropertyChanged("TotalTimeSeconds");
                }
            }
        }

        #endregion

        private Players playerType;
        public Players PlayerType
        {
            get { return playerType; }
            set {
                if (value != playerType)
                {
                    playerType = value;
                    NotifyPropertyChanged("PlayerType");
                }
            }
        }

        public void SetDefaultState()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var nothingIsPlaying = loader.GetString("NothingIsPlaying");
            Title = nothingIsPlaying;

            Thumbnail = Fanart = ShowTitle = Tagline = "";
            Artist = new List<string>();
            Speed = -1;
            TimeSeconds = 0;
            TotalTimeSeconds = 100;

            PlayerType = Players.None;
        }
    }
}
