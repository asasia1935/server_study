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
        static int number = 0;
        // 이제 한 부분을 한 쓰레드가 작업하고 있으면 다른 쓰레드에서 건들 수 없도록 하는
        // critical section이라는 임계영역을 만들 것임
        // c++에선 std::mutex로 구현되어있음
        static object _obj = new object();
        static void Thread_1()
        {
            for (int i = 0; i < 1000000; i++)
            {
                // 상호 배제 Mutual Exclusive (나만 쓰겠다)
                Monitor.Enter(_obj); // 문을 잠그는 행위
                {
                    number++; // 잠긴 곳은 싱글 쓰레드처럼 생각해도 됨
                    // 여기에서 return; 같은 코드를 작성한 경우 잠금을 풀지 않았으니 영영 풀리지 않음
                }
                Monitor.Exit(_obj); // 잠금을 푸는 행위
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
            {
                // 위에서 리턴이 된 경우 여기서 먹통이 일어날 것임(DeadLock)
                Monitor.Enter(_obj);
                number--;
                Monitor.Exit(_obj);
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}

// 위에서 잠금을 풀지 않고 넘어가는 경우를 방지하기 위해서 try catch문을 이용할 수 있다
// c#의 경우
//try
//{
//    Monitor.Enter(_obj);
//    number++;
//    return;
//}
//finally
//{
//    Monitor.Exit(_obj);
//}
// 다음과 같이 하여 반드시 exit를 하도록 하면 된다

// 사실 이 마저도 코드가 더러워지므로 lock을 이용하면 더 편하다
// lock(_obj)
// {
//     number++;
// }
// _obj가 자물쇠의 개념이 되어 잠그고 들어간다는 의미로 볼 수 있다.