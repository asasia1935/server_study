using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCoreEx
{
    class Lock
    {
        // bool값 <- 커널
        // AutoResetEvent _available = new AutoResetEvent(true); // true -> available한 상태 (열린 상태로 시작)
        // Auto -> 문을 닫는 것을 자동으로 해줌

        ManualResetEvent _available = new ManualResetEvent(false);
        // Manual -> 문을 자동으로 닫지 않는다

        public void Acquire()
        {
            _available.WaitOne(); // 입장 시도 -> 문이 열려있으니 바로 입장 가능
            // _available.Reset(); // bool을 false로 바꾼다 (문을 닫는 행위)
            // 하지만 생략해야 하는 이유는 WaitOne에서 입장시 바로 문을 닫기 때문            
        }

        public void Release()
        {
            _available.Set(); // true로 바꿔준다 (다시 문을 열어주는 행위)
        }
    }

    // 실제로 해당 코드를 돌려보면 아까 방식에 비해서 확연히 느려진 것을 알 수 있다.
    // 이는 커널모드까지 가야하는 부하가 더 심해졌기 때문이다.

    class Program
    {
        static int _num = 0;
        // static Lock _lock = new Lock();
        static Mutex _lock = new Mutex();
        // Mutex 또한 커널을 이용해서 순서를 맞추는 방법
        // 역시 커널 동기화 객체라서 느릴 수밖에 없다

        // 아까 Event는 bool로만 확인을 했는데
        // Mutex는 여러 정보를 갖고 있다(int) -> 예를 들어 여러번 잠그고 싶을 때 (But 추가 비용이 들어감)
        // Auto로만 충분하긴 함

        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                /* _lock.Acquire();
                   _num--;
                   _lock.Release();
                   // Event를 이용할 때 코드
                */

                _lock.WaitOne();
                _num++;
                _lock.ReleaseMutex();
                // Mutex를 이용하는 코드
            }
        }
        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                /* _lock.Acquire();
                 _num--;
                 _lock.Release();
                 // Event를 이용할 때 코드
                */

                _lock.WaitOne();
                _num--;
                _lock.ReleaseMutex();
                // Mutex를 이용하는 코드
            }
        }
        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }
    }
}