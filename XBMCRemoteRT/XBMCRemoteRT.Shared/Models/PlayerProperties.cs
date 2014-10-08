using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBMCRemoteRT.Models
{
    public class PlayerProperties : NotifyBase
    {
        private bool partyMode;
        public bool PartyMode
        {
            get { return partyMode; }
            set
            {
                partyMode = value;
                NotifyPropertyChanged("PartyMode");
            }
        }

        private int speed;
        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                NotifyPropertyChanged("Speed");
            }
        }

        private bool shuffled;
        public bool Shuffled
        {
            get { return shuffled; }
            set
            {
                shuffled = value;
                NotifyPropertyChanged("Shuffled");
            }
        }

        private string repeat;
        public string Repeat
        {
            get { return repeat; }
            set
            {
                repeat = value;
                NotifyPropertyChanged("Repeat");
            }
        }
    }
}
