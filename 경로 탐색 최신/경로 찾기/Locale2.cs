using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 경로_찾기
{
    
    internal class Locale2
    {
        internal double TotalDistance; //출발지와 목적지의 총 거리
                                                        
        internal string Pname { get; } //장소명
        internal double Lat { get; } //위도
        internal double Lng { get; } //경도
        

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
