using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Speech.Synthesis;

namespace 경로_탐색
{
    internal class Program2
    {
        static void Main(string[] args)
        {        
            string result = "현재 ";
            SpeechSynthesizer synth = new SpeechSynthesizer
            {
                Volume = 100,  // 음량 (0 ~ 100)
                Rate = 2       // 속도 (-10 ~ 10)
            };

            try
            {
                // CSV 파일에서 목적지 정보 읽어오기
                List<Locale2> destinations = ReadDestinationsFromCSV("C:\\Users\\user\\Desktop\\최종프로젝트\\API\\경로 탐색\\finaltest.csv");

                Locale2 start = new Locale2("천안역", 127.14682487, 36.80898535, 0);

                List<(Locale2, double)> distances = new List<(Locale2, double)>();

                // 모든 경로의 거리 계산
                foreach (var end in destinations)
                {
                    Console.WriteLine($"\n경로 탐색 : {start.Pname} -> {end.Pname}");
                    double distance = SearchRoute(start, end);
                    if (distance > 0)
                    {
                        distances.Add((end, distance));
                    }
                }

                // 가장 가까운 1개의 경로 선택
                var closestOne = distances.OrderBy(d => d.Item2).FirstOrDefault();

                // 가장 가까운 경로가 존재하는지 확인 후 작업 수행
                if (closestOne != default)
                {
                    var closestDestination = closestOne.Item1;
                    var closestDistance = closestOne.Item2;

                    string distanceUnit = closestDistance <= 1000 ? "미터" : "킬로미터";
                    double result_distance = closestDistance <= 1000 ? closestDistance : closestDistance / 1000;

                    string output = closestDestination.Pname + "까지 " + result_distance.ToString() + distanceUnit + " 남았습니다.\n";
                    Console.WriteLine(output);
                    result += output;

                    // txt 파일로 저장
                    SaveToFile(start, closestDestination, result_distance, distanceUnit);
                }

                synth.Speak(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 발생: {ex.Message}");
            }
        }

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
                            else
                            {
                                Console.WriteLine($"위도/경도 값 변환 실패: {data[1]}, {data[2]}");
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

        // txt 파일로 저장
        private static void SaveToFile(Locale2 start, Locale2 destination, double distance, string distanceUnit)
        {
            try
            {
                string filePath = "C:\\Users\\user\\Desktop\\최종프로젝트\\API\\경로 탐색\\closest_destination.txt";
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine(start.Lat); // 천안역 위도
                    sw.WriteLine(start.Lng); // 천안역 경도
                    sw.WriteLine(destination.Lat); // 목적지 위도
                    sw.WriteLine(destination.Lng); // 목적지 경도
                }
                Console.WriteLine("가장 가까운 졸음쉼터 정보를 파일로 저장했습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 저장 중 오류 발생: {ex.Message}");
            }
        }

        // 경로 탐색 TMap API
        private static double SearchRoute(Locale2 start, Locale2 end)
        {
            try
            {
                string url = "https://apis.openapi.sk.com/tmap/routes";
                string query_str = string.Format("{0}?version=1&startX={1}&startY={2}&endX={3}&endY={4}&startName={5}&endName={6}",
                    url, start.Lng, start.Lat, end.Lng, end.Lat, start.Pname, end.Pname);
                WebRequest request = WebRequest.Create(query_str);
                request.Headers.Add("appKey", "E3gnlpqKiv1H36aFTfsn4a40DHb7Byik1nK8ATY6");
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                dynamic dres = jss.Deserialize<dynamic>(content);
                double totalDistance = dres["features"][0]["properties"]["totalDistance"]; // totalDistance 추출
                start.TotalDistance = totalDistance; // 출발지의 totalDistance 설정

                return totalDistance; // 거리를 반환
            }
            catch (Exception ex)
            {
                Console.WriteLine($"경로 탐색 중 오류 발생: {ex.Message}");
                return -1; // 오류 발생 시 -1 반환
            }
        }
    }
}