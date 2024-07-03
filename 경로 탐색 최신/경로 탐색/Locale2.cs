using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 경로_탐색
{
    internal class Locale2
    {
        internal double TotalDistance;

        internal string Pname { get; }
        internal double Lat { get; }
        internal double Lng { get; }
        

        public Locale2(string pname, double lng, double lat, double totalDistance)
        {
            this.Pname = pname;
            this.Lat = lat;
            this.Lng = lng;
            this.TotalDistance = totalDistance;
        }

        public override string ToString()
        {
            return Pname;
        }
    }
}
