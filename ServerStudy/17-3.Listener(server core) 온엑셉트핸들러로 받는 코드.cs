using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCoreEx
{
    class Program
    {
        static Listener _listener = new Listener();
        
        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                // 받는다
                byte[] recvBuffer = new byte[1024]; // 받을 내용을 저장
                int recvBytes = clientSocket.Receive(recvBuffer);
                string recvData = Encoding.UTF8.GetString(recvBuffer, 0 /* 시작 인덱스 */, recvBytes /* 문자열 크기 */); // 방금 받은 애를 string으로 변환
                Console.WriteLine($"[From Client] {recvData}");

                // 보낸다
                byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !"); // 보낼 버퍼는 몇개짜리를 보낼걸 알고 있으니 바로 만들어주면 됨
                clientSocket.Send(sendBuffer);

                // 쫒아낸다
                clientSocket.Shutdown(SocketShutdown.Both); // 더이상 소통하지 않는다는 것을 전달
                clientSocket.Close(); // 연결 종료
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            // www.어쩌구저쩌구.com -> 123.123.123.12
            string host = Dns.GetHostName(); // 로컬 컴퓨터의 호스트 이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0]; // 아이피 호스트들의 배열 (식당 주소)
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 최종 주소 설정 (포트 번호는 해당 식당의 여러 문 중 하나)
            
            _listener.Init(endPoint, OnAcceptHandler); // 온엑셉트핸들러로 알려줘 라는 뜻
            Console.WriteLine("Listening...");

            while (true)
            {
                ;
            }

        }
    }
}