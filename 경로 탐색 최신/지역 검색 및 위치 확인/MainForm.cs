using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace 지역_검색_및_위치_확인
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            webbr.ScriptErrorsSuppressed = true; // 스크립트 오류 억제
            webbr.Navigate("https://kwonminsugithub.github.io/ms2/index.html"); // GitHub Pages URL로 변경
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            string query = tbox_query.Text;
            string url = "https://dapi.kakao.com/v2/local/search/keyword.json";
            string query_str = string.Format("{0}?query={1}", url, query);
            WebRequest request = WebRequest.Create(query_str);
            request.Headers.Add("Authorization", "KakaoAK e386b052ddc547078c27f1de6ccd3db6");
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string content = sr.ReadToEnd();
            Console.WriteLine(content);

            JavaScriptSerializer jss = new JavaScriptSerializer();
            dynamic d = jss.Deserialize<dynamic>(content);
            dynamic[] ddoc = d["documents"];
            lbox_locale.Items.Clear();
            foreach (dynamic elem in ddoc)
            {
                string pname = elem["place_name"];
                double lng = double.Parse(elem["x"]);
                double lat = double.Parse(elem["y"]);
                Locale locale = new Locale(pname, lng, lat);
                lbox_locale.Items.Add(locale);
            }
        }

        private void lbox_locale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbox_locale.SelectedItem is Locale selectedLocale)
            {
                HtmlDocument hdoc = webbr.Document;
                object[] objs = new object[2];
                objs[0] = selectedLocale.Lat;
                objs[1] = selectedLocale.Lng;
                hdoc.InvokeScript("setCenter", objs);
            }
        }
    }
}