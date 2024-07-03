// Program.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace 경로_찾기
{
    internal static class Program
    {
        private const int Port = 12345;
        private static TcpListener listener;
        private static bool isRunning = true;

        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()       
        {
            //listener = new TcpListener(IPAddress.Parse("192.168.0.81"), Port);
            //listener.Start();
            //Console.WriteLine("Server started...");

            //// 클라이언트 연결을 비동기로 처리
            //Thread acceptThread = new Thread(AcceptClients);
            //acceptThread.Start();

            //// 콘솔 애플리케이션이 종료될 때까지 대기
            //while (isRunning)
            //{
            //    Thread.Sleep(100); // CPU 사용을 줄이기 위해 약간의 대기 시간을 둠
            //}

            ////서버가 종료되면 폼을 실행
            //Application.Run(new Form1());


            // Program2의 Execute 메서드 호출
            경로_찾기.Program2.Execute();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void AcceptClients()
        {
            while (isRunning)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Client connected...");
                    Thread clientThread = new Thread(HandleClient);
                    clientThread.SetApartmentState(ApartmentState.STA); // STA 모드 설정
                    clientThread.Start(client);
                }
                catch (SocketException)
                {
                    // listener.Stop() 호출 시 예외 발생, 이를 무시하고 종료 루프 탈출
                    if (!isRunning)
                        break;
                }
            }
        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1];

            try
            {
                while (isRunning)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    char key = (char)buffer[0];
                    Console.WriteLine("Received from client: " + key);

                    if (key == 'q')
                    {
                        Console.WriteLine("Received 'q'. Server is shutting down...");
                        isRunning = false;
                        listener.Stop();
                        client.Close();

                        // 폼을 실행시키는 로직
                        Application.Run(new MainForm());
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

    }
}
