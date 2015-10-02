using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErgometerLibrary
{
    class Helper
    {
        public static double Now { get { return (DateTime.Now - DateTime.Parse("1/1/1870 0:0:0")).TotalMilliseconds; } }
    }
}
