using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 경로_찾기
{
    internal class Locale
    {
        internal string Pname { get; } //장소이름
        internal double Lng { get; } //위도
        internal double Lat { get; } //경도

        internal double TotalDistance { get; set; } //총 거리
        public Locale(string pname, double lng, double lat, double totalDistance)
        {
            this.Pname = pname;
            this.Lng = lng;
            this.Lat = lat;
            this.TotalDistance = totalDistance;
        }

        public Locale(string pname, double lng, double lat)
        {
            Pname = pname;
            Lng = lng;
            Lat = lat;
        }

        public override string ToString()
        {
            return Pname;
        }
    }
}
