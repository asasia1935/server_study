using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCoreEx
{
    class Listener // 리스너 부분 빼서 관리
    {
        Socket _listenSocket;
        Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            // 문지기가 가진 휴대폰
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업 시작
            // backlog : 최대 대기수 -> live에서 조절
            _listenSocket.Listen(10);

            // Accept 부분을 비동기 방식으로 바꾼 버전
            // 등록 (인위적인 등록 후에 밑에서 뺑뺑이를 돌도록 함)
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted); // 나중에 자동으로 밑의 함수를 호출
            ResisterAccept(args); // 낚시대를 던지는 느낌
        }

        void ResisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null; // 이벤트를 재사용할  때 한번 초기화 해주는것

            bool pending = _listenSocket.AcceptAsync(args); // 일단 예약만 해두는것 (바로 될수도 기다렸다 될수도)
            if(pending == false) // 비동기로 호출하였지만, 바로 완료가 됐다면(클라이언트의 접속 요청이 바로)
            {
                OnAcceptCompleted(null, args); // 직접 완료 해줌
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args) // 물고기가 잡혀서 낚시대를 끌어올리는 부분
        {// 결국 어떤 방식으로든 이쪽으로 들어올수밖에 없음
            if(args.SocketError == SocketError.Success) 
            {
                // Todo (유저가 왔을때 무엇을 해야하는지 처리)
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            // 다음을 위해 등록 (다시 낚시대를 던지기)
            ResisterAccept(args);
        }
    }
}
