using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErgometerLibrary
{
    class NetCommand
    {

        public int Session { get; set; }
        public string DisplayName { get; set; }
        public double Timestamp { get; set; }

        public NetCommand(int session, string displayname)
        {
            Session = session;
            DisplayName = displayname;
            Timestamp = (DateTime.Now - DateTime.Parse("1/1/1870 0:0:0")).TotalMilliseconds;
        }
    }
}
