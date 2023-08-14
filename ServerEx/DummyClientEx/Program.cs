using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DummyClientEx
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName(); // 로컬 컴퓨터의 호스트 이름
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0]; // 아이피 호스트들의 배열 (식당 주소)
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 최종 주소 설정 (포트 번호는 해당 식당의 여러 문 중 하나)

            // 휴대폰 설정
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // 문지기에게 입장 문의
                socket.Connect(endPoint); // 연결을 하겠다 -> 상대방의 주소로 전달
                Console.WriteLine($"Connected to {socket.RemoteEndPoint.ToString()}"); // 연결한 반대쪽 대상을 출력

                // 보낸다
                byte[] sendBuffer = Encoding.UTF8.GetBytes("Hello World!");
                int sendBytes = socket.Send(sendBuffer);

                // 받는다
                byte[] recvBuffer = new byte[1024];
                int recvBytes = socket.Receive(recvBuffer);
                string recvData = Encoding.UTF8.GetString(recvBuffer, 0, recvBytes);
                Console.WriteLine($"[From Server] {recvData}");

                // 나간다
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
