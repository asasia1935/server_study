using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCoreEx
{
    class Program
    {
        static void MainThread(object state) // 쓰레드로 실행할 메인 함수
        { // object state는 당장 사용해야하는것은 아니지만 쓰레드풀로 사용할때 필요하므로 작성
            for (int i = 0; i < 5; i++)
                Console.WriteLine("Hello Thread!");
        }

        static void Main(string[] args)
        {
            //// 뉴 쓰레드를 하는 것은 새 직원을 뽑는 것과 같으므로 부하가 좀 심한 편이다
            //Thread t = new Thread(MainThread); // 쓰레드 생성후 메인쓰레드와 연결
            //t.Name = "Test Thread"; // 쓰레드 이름 정의
            //t.IsBackground = true; // 쓰레드를 백그라운드로 설정 (메인쓰레드가 종료되면 종료되도록)
            //t.Start(); // 쓰레드 시작
            //t.Join(); // t가 끝날때까지 기다렸다가 이후 내용을 실행하겠다는 뜻

            //// 그렇기에 쓰레드를 직접 만들어서 관리하는게 아니라 단기알바처럼 쓸 수 있는데 그것이 쓰레드풀이다
            //ThreadPool.QueueUserWorkItem(MainThread);
            //// 이미 쓰레드가 있는 상태에서 할 일을 넘겨주면 알아서 하는 느낌 (쓰레드 풀링)

            //// 쓰레드를 사용하는 것과 쓰레드풀을 사용하는 것의 차이
            //// 쓰레드가 아무리 많더라도 CPU코어가 그 수만큼보다 적으면 일에 있어서 손해가 생김
            //// 쓰레드가 막 1000개 생기면 일을 시키는것보다 쓰레드에게 명령을 내리는 시간이 더 오래걸리기 때문
            //// 반면 쓰레드풀링을 이용하는 것은 알아서 일을 끝낸 쓰레드에게 작업을 내리기 때문에 빠를 수 있음
            //// 물론 작업이 막 무한대로 걸리는 것인 경우는 쓰레드가 돌아올 수 없으므로 쓰레드풀은 짧은 작업을 쓰는 것이 필요함

            ThreadPool.SetMinThreads(1, 1); // 최소 쓰레드 1개
            ThreadPool.SetMinThreads(5, 5); // 최대 쓰레드 5개

            for (int i = 0; i < 5; i++) // 5번을 돌려서 5개의 쓰레드를 전부 추출해서 사용하는 경우
            {
                Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning); // Task는 작업 단위를 관리하는것
                // 뒤의 인자에 저것을 넣어서 오래 걸리는 작업이라는 것을 명시하는것 -> 쓰레드풀을 효율적으로 이용
                t.Start(); // task역시 쓰레드풀에서 관리
            }

            ThreadPool.QueueUserWorkItem(MainThread);
            // C#에서는 쓰레드를 직접 만들어서 관리하는 일이 없고 보통 간단한 작업은 ThreadPool의 기능을 이용하거나 복잡한 작업은 Task를 만들어서 넣는 식으로 사용

        }
    }
}