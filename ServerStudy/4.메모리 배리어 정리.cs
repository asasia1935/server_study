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
        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;

        // 해당 두 쓰레드를 어떻게 조합해도 코드 순서대로 실행이 된다면 x, y가 0이 되는 경우가 존재할 수 없다.
        // 이때 문제는 하드웨어도 최적화를 하기 때문에 두 코드가 의존성이 없으면 순서를 뒤바꿀 수 있기 떄문임.
        // ex) y = 1과 r1 = x는 둘이 관련이 없어서 바뀌어서 실행이 될 수 있음
        // r1 = x와 r2 = y를 먼저 실행하면 둘다 0이 되어서 while문을 빠져나오는 경우가 생김

        // 싱글쓰레드에서는 문제가 되지 않지만 멀티쓰레드에서는 문제가 생길 수 있음

        // 메모리배리어
        // A) 코드 재배치 억제
        // B) 가시성 : 직원(쓰레드)이 자기 수첩(캐시)에 있는 정보(데이터)를 주문현황(메인메모리)에 갱신을 하면 실제 상황에 맞아 떨어지도록 업데이트가 될 것임
        // 만약 코드에서 메모리 배리어를 실행할 경우 각 값들이 실제 값으로 볼 수 있게 됨
        // volatile도 같은 가시성을 보이는 문법임(내부적으로 배리어 구현)

        // 1) Full Memory Barrier : Store/Load 둘다 막는다
        // 2) Store Memory Barrier : Store만 막는다
        // 3) Load Memory Barrier : Load만 막는다

        static void Thread_1()
        {
            y = 1; // y 값 저장

            Thread.MemoryBarrier(); // 해당 위치에 벽을 세우는 것과 같음
            // 그러면 하드웨어가 멋대로 위치를 바꾸지 않을 것임

            r1 = x; // x 값 읽기
        }

        static void Thread_2()
        {
            x = 1; // x 값 저장

            Thread.MemoryBarrier(); // 반대 쓰레드에서도 같은 작업을 해야함

            r2 = y; // y 값 읽기
        }

        static void Main(string[] args)
        {
            int count = 0;
            while (true)
            {
                count++;
                x = y = r1 = r2 = 0;

                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);

                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2); // 끝날때까지 메인스레드는 대기

                if (r1 == 0 && r2 == 0)
                    break;
            }

            Console.WriteLine($"{count}번만에 빠져나옴");
        }
    }
}

// 메모리 배리어 추가적인 예제

//int _answer;
//bool _complete;

//void A()
//{
//    _answer = 123;
//    Thread.MemoryBarrier(); 1
//    _complete = true;
//    Thread.MemoryBarrier(); 2
//}

//void B()
//{
//    Thread.MemoryBarrier(); 3
//    if (_complete)
//    {
//        Thread.MemoryBarrier(); 4
//        Console.WriteLine(_answer);
//    }
//}

// 2번과 3번 배리어에 대해서
// store의 경우 배리어는 위의 변수의 가시성을 챙겨주는 것임 그렇기 때문에 2번은 위의 _complete의 가시성을 챙기는 것임
// load의 경우 배리어는 아래 변수의 가시성을 챙겨주는 것임 그렇기 때문에 3번은 아래의 _complete의 가시성을 챙기는 것임