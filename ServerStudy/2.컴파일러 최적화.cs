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
        volatile static bool _stop = false; // 전역으로 된 변수는 모든 쓰레드가 공용으로 사용(동시 접근 가능)
        // 휘발성이라는 뜻인데 _stop은 언제 변할지 모르니 최적화 하지 말고 있는 그대로 사용하라는 의미
        // c#에서는 캐시를 무시하고 최신 값을 가져오도록 하는 기능도 있음 (헷갈리므로 쓰지 않도록 권장)

        static void ThreadMain()
        {
            Console.WriteLine("쓰레드 시작");

            while (_stop == false) // 누군가 stop을 true로 바꾸도록 기다린다
            {
                // Debug 모드가 아니라 게임의 실제 버전에선 Release 모드로 실행됨
                // 그러면 멀티쓰레드에서 안되는 경우가 생김 (지금 실습에선 되는데 원래 안됨)
                // ->
                // 같은 코드가 컴파일러에 의해 다음과 같이 최적화되기 때문
                // if(_stop == false)
                //    {
                //         while(true) { }
                //    }
            }

            Console.WriteLine("쓰레드 종료");
        }
        static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000); // 1000ms동안 잠시 대기

            _stop = true; // 쓰레드가 실행되고 1초 뒤에 true로 바뀌어서 while문을 멈추도록

            Console.WriteLine("Stop 호출");
            Console.WriteLine("종료 대기중");
            t.Wait(); // 쓰레드의 Join같은 친구(t가 끝날때까지 대기)
            Console.WriteLine("종료 성공");
        }
    }
}