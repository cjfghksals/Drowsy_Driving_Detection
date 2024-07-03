using System;
using System.Collections.Generic; // 제네릭 컬렉션
using System.Diagnostics;
using System.IO;  //입출력 기능
using System.Net; // 네트워크 기능
using System.Web.Script.Serialization; // JSON 직렬화/역직렬화
using System.Windows.Forms;

namespace 경로_찾기
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Hide();

            // 파일에서 위도와 경도 읽어오기
            try
            {
                // 파일 경로에서 모든 줄 읽기
                string[] lines = File.ReadAllLines("C:\\Users\\user\\Desktop\\최종프로젝트\\API\\경로 탐색 최신\\closest_destination.txt");
                if (lines.Length >= 4) //파일에 최소한 4개의 줄이 있는지 확인
                {
                    // 각 줄을 읽어서 위도와 경도 값으로 변환
                    tb_start_name.Text = lines[0];
                    double latitude_start = double.Parse(lines[1]);
                    double longitude_start = double.Parse(lines[2]);

                    tb_end_name.Text = lines[3];
                    double latitude_end = double.Parse(lines[4]);
                    double longitude_end = double.Parse(lines[5]);

                    // 출발지와 목적지에 위도, 경도 값을 입력
                    tb_start_lat.Text = latitude_start.ToString();
                    tb_start_lng.Text = longitude_start.ToString();
                    tb_end_lat.Text = latitude_end.ToString();
                    tb_end_lng.Text = longitude_end.ToString();

                   

                    // 출발지와 목적지 자동 검색
                    if (tb_start_lat.Text != null && tb_start_lng.Text != null)
                    {
                        StartLocationSearch();
                    }
                    if (tb_start_lng.Text != null && tb_end_lat.Text != null)
                    {
                        EndLocationSearch();
                    }
                }
                else
                {
                    MessageBox.Show("파일 형식이 올바르지 않습니다. 파일에는 두 줄에 각각 위도와 경도 값이 있어야 합니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일을 읽는 중 오류가 발생했습니다: {ex.Message}");
            }


            // 리스트 박스의 첫 번째 항목 선택
            if (lbox_start.Items.Count > 0)
            {
                lbox_start.SelectedIndex = 0;
            }
            if (lbox_end.Items.Count > 0)
            {
                lbox_end.SelectedIndex = 0;
            }
            if (tb_start_lat.Text != null && tb_start_lng.Text != null && tb_start_lng.Text != null && tb_end_lat.Text != null)
            {
                searching();
            }
        }

        //// 폼이 로드될 때 실행되는 메서드
        //private void MainForm_Load(object sender, EventArgs e)
        //{
        //    MessageBox.Show("경로 찾기를 원하시면 버튼을 눌러주세요");
        //}

        

        // CSV 파일에서 목적지 정보 읽어오기
        private static List<Locale2> ReadDestinationsFromCSV(string filePath)
        {
            List<Locale2> destinations = new List<Locale2>();

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] data = line.Split(',');

                        if (data.Length >= 3)
                        {
                            string pname = data[0] + "졸음쉼터";

                            if (double.TryParse(data[1], out double lat) && double.TryParse(data[2], out double lng))
                            {
                                destinations.Add(new Locale2(pname, lng, lat, 0));
                            }
                        }
                        else
                        {
                            Console.WriteLine("잘못된 데이터 형식: " + line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CSV 파일 읽기 중 오류 발생: {ex.Message}");
            }

            return destinations;
        }
        // 출발지 버튼 함수 호출
        private void btn_start_Click(object sender, EventArgs e)
        {
            StartLocationSearch();
        }
        // 출발지 검색
        private void StartLocationSearch()
        {
            SearchLocation(lbox_start, tb_start_lat, tb_start_lng);
        }
        // 목적지 버튼 함수 호출
        private void btn_end_Click(object sender, EventArgs e)
        {
            EndLocationSearch();
        }
        // 목적지 검색
        private void EndLocationSearch()
        {
            SearchLocation(lbox_end, tb_end_lat, tb_end_lng);
        }

        // Kakao API를 활용한 지도 로컬 검색
        private void SearchLocation(ListBox lbox, TextBox textBoxLat, TextBox textBoxLng)
        {
            lbox.Items.Clear(); // 리스트 박스 항목 초기화
            double latitude = double.Parse(textBoxLat.Text); // 위도 값 파싱
            double longitude = double.Parse(textBoxLng.Text); // 경도 값 파싱
            string url = $"https://dapi.kakao.com/v2/local/geo/coord2regioncode.json?x={longitude}&y={latitude}";
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add("Authorization", "KakaoAK e386b052ddc547078c27f1de6ccd3db6");
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string content = sr.ReadToEnd();

            JavaScriptSerializer jss = new JavaScriptSerializer();
            dynamic d = jss.Deserialize<dynamic>(content);
            dynamic[] ddoc = d["documents"];
            foreach (dynamic elem in ddoc)
            {
                string pname = elem["address_name"];
                Locale locale = new Locale(pname, longitude, latitude);
                lbox.Items.Add(locale);
            }
        }

        // 경로 탐색에 사용할 로케일 리스트와 현재 위치 인덱스
        List<Locale> locales = new List<Locale>();
        int now;

        //경로 탐색 버튼
        private void btn_search_Click(object sender, EventArgs e)
        {
            locales.Clear();
            lbox_point.Items.Clear();

            Locale start = lbox_start.SelectedItem as Locale;
            Locale end = lbox_end.SelectedItem as Locale;

            // 경로 탐색 API 호출
            string url = "https://apis.openapi.sk.com/tmap/routes";
            string query_str = string.Format("{0}?version=1&startX={1}&startY={2}&endX={3}&endY={4}&startName={5}&endName={6}",
                url, start.Lng, start.Lat, end.Lng, end.Lat, start.Pname, end.Pname);
            WebRequest request = WebRequest.Create(query_str);
            request.Headers.Add("appKey", "tZ98CcC1IUapFfsMe7FRn6VgQ0XUKUXfInzRZLSj");
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            dynamic dres = jss.Deserialize<dynamic>(content);

            // 총 거리 계산 및 텍스트 박스에 총 거리 자동 생성
            double totalDistance = dres["features"][0]["properties"]["totalDistance"];
            string distanceUnit = totalDistance <= 1000 ? "m" : "km";
            double totaldistance = totalDistance / 1000;
            tb_total_distance.Text = string.Format("{0}{1}", totaldistance, distanceUnit);

            // 경로 정보 파싱
            dynamic dfea = dres["features"];
            foreach (dynamic d in dfea)
            {
                if (d["geometry"]["type"] == "Point")
                {
                    dynamic dd = d["geometry"]["coordinates"];
                    string pname = d["properties"]["description"];
                    double elemLng = (double)dd[0];
                    double elemLat = (double)dd[1];
                    Locale locale = new Locale(pname, elemLng, elemLat);
                    locales.Add(locale);
                    lbox_point.Items.Add(locale);
                }
                if (d["geometry"]["type"] == "LineString")
                {
                    dynamic dd = d["geometry"]["coordinates"];
                    string pname = d["properties"]["name"];
                    bool check = true;
                    foreach (dynamic d3 in dd)
                    {
                        double elemLng = (double)d3[0];
                        double elemLat = (double)d3[1];
                        Locale locale = new Locale(pname, elemLng, elemLat);
                        locales.Add(locale);
                        if (check)
                        {
                            lbox_point.Items.Add(locale);
                            check = false;
                        }
                    }
                }

                // 경로 정보를 객체 배열에 저장
                object[] objs = new object[locales.Count * 2];
                int i = 0;
                foreach (Locale lo in locales)
                {
                    objs[i] = lo.Lat;
                    i++;
                    objs[i] = lo.Lng;
                    i++;
                }
                HtmlDocument hdoc = webbr.Document;
                hdoc.InvokeScript("setLine", objs);
                object[] objs2 = new object[] {( start.Lat + end.Lat ) / 2,
                    (start.Lng + end.Lng)/ 2};
                hdoc.InvokeScript("setCenter", objs2);
            }
            simulation();
        }

        //경로 시뮬레이션 버튼
        private void btn_simulation_Click(object sender, EventArgs e)
        {
            HtmlDocument hdoc = webbr.Document;
            object[] objs = new object[] { 4 };
            hdoc.InvokeScript("setLevel", objs);
            timer_simulation.Start();
        }

        //경로 시뮬레이션을 위한 타이머
        private void timer_simulation_Tick(object sender, EventArgs e)
        {
            if (now < locales.Count)
            {
                Locale locale = locales[now];
                ViewLocale(locale);
                now++;
            }
            else
            {
                timer_simulation.Enabled = false;
            }
        }

        //장소 보여주기
        private void lbox_point_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbox_point.SelectedIndex == -1)
            {
                return;
            }
            Locale locale = lbox_point.SelectedItem as Locale;
            ViewLocale(locale);
        }

        //지도에서 특정 장소 마커 보여주기
        private void ViewLocale(Locale locale)
        {
            HtmlDocument hdoc = webbr.Document;
            string msg = string.Format("<div>{0}</div>", locale.Pname);
            object[] objs = new object[] { locale.Lat, locale.Lng };
            hdoc.InvokeScript("setCenter", objs);
            object[] objs2 = new object[] { locale.Lng, locale.Lat, msg };
            hdoc.InvokeScript("setMarkerOnly", objs2);
        }

        //자동차 경로 탐색 및 거리 계산
        private void searching()
        {
            locales.Clear();
            lbox_point.Items.Clear();

            if (lbox_start.SelectedItem == null)
            {
                MessageBox.Show("출발지를 선택해주세요.");
                return;
            }

            if (lbox_end.SelectedItem == null)
            {
                MessageBox.Show("목적지를 선택해주세요.");
                return;
            }

            Locale start = lbox_start.SelectedItem as Locale;
            Locale end = lbox_end.SelectedItem as Locale;

            string url = "https://apis.openapi.sk.com/tmap/routes";
            string query_str = string.Format("{0}?version=1&startX={1}&startY={2}&endX={3}&endY={4}&startName={5}&endName={6}",
                url, start.Lng, start.Lat, end.Lng, end.Lat, start.Pname, end.Pname);
            WebRequest request = WebRequest.Create(query_str);
            request.Headers.Add("appKey", "tZ98CcC1IUapFfsMe7FRn6VgQ0XUKUXfInzRZLSj");
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            dynamic dres = jss.Deserialize<dynamic>(content);

            double totalDistance = dres["features"][0]["properties"]["totalDistance"];
            string distanceUnit = totalDistance <= 1000 ? "m" : "km";
            double totaldistance = totalDistance / 1000;
            tb_total_distance.Text = string.Format("{0}{1}", totaldistance, distanceUnit);

            dynamic dfea = dres["features"];
            foreach (dynamic d in dfea)
            {
                if (d["geometry"]["type"] == "Point")
                {
                    dynamic dd = d["geometry"]["coordinates"];
                    string pname = d["properties"]["description"];
                    double elemLng = (double)dd[0];
                    double elemLat = (double)dd[1];
                    Locale locale = new Locale(pname, elemLng, elemLat);
                    locales.Add(locale);
                    lbox_point.Items.Add(locale);
                }
                if (d["geometry"]["type"] == "LineString")
                {
                    dynamic dd = d["geometry"]["coordinates"];
                    string pname = d["properties"]["name"];
                    bool check = true;
                    foreach (dynamic d3 in dd)
                    {
                        double elemLng = (double)d3[0];
                        double elemLat = (double)d3[1];
                        Locale locale = new Locale(pname, elemLng, elemLat);
                        locales.Add(locale);
                        if (check)
                        {
                            lbox_point.Items.Add(locale);
                            check = false;
                        }
                    }
                }
            }

            object[] objs = new object[locales.Count * 2];
            int i = 0;
            foreach (Locale lo in locales)
            {
                objs[i] = lo.Lat;
                i++;
                objs[i] = lo.Lng;
                i++;
            }
            HtmlDocument hdoc = webbr.Document;
            if (hdoc != null)
            {
                hdoc.InvokeScript("setLine", objs);
                object[] objs2 = new object[] { (start.Lat + end.Lat) / 2, (start.Lng + end.Lng) / 2 };
                hdoc.InvokeScript("setCenter", objs2);
            }
        }

        //출발지에서 목적지 이동 시뮬레이션
        private void simulation()
        {
            HtmlDocument hdoc = webbr.Document;
            object[] objs = new object[] { 4 };
            hdoc.InvokeScript("setLevel", objs);
            timer_simulation.Start();
        }

        // 종료 버튼
        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
