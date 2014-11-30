using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBMCRemoteRT.RPCWrappers;

namespace XBMCRemoteRT.Models
{
    public class PlayerItem : NotifyBase
    {
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
            set {
                if (artist != value)
                {
                    artist = value;
                    NotifyPropertyChanged("Artist"); 
                }
            }
        }
        public string Label { get; set; }
        public int Id { get; set; }
        public string Type { get; set; }

        public override bool Equals(object obj)
        {
            var newObject = obj as PlayerItem;
            bool isEqual = this.Artist == newObject.Artist && 
                this.Fanart == newObject.Fanart && 
                this.Id == newObject.Id && 
                this.Label == newObject.Label && 
                this.ShowTitle == newObject.ShowTitle && 
                this.Tagline == newObject.Tagline && 
                this.Thumbnail == newObject.Thumbnail && 
                this.Title == newObject.Title && 
                this.Type == newObject.Type;
            return isEqual;
        }

    }
}
