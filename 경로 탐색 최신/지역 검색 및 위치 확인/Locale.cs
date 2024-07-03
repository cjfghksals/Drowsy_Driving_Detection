using System;

namespace 지역_검색_및_위치_확인
{
    internal class Locale
    {
        internal string Pname { get; }
        internal double Lng { get; }
        internal double Lat { get; }
        internal double TotalDistance { get; set; } // 추가된 부분

        public Locale(string pname, double lng, double lat, double totalDistance) // 생성자 수정
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
