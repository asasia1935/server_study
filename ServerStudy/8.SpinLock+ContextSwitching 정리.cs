using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCoreEx
{
    class SpinLock
    {
        volatile int _locked = 0;
        /*SpinLock Lock구현이론에서 첫번째 방법

자물쇠가 1개인데 두 쓰레드가 정말 드물게 서로 동시에 접근하는 경우 공동으로 들어가게 된다면
이럴때 자물쇠를 잠그고 들어가는 행위가 한번에(원자적으로) 일어나야 둘이 공동으로 들어가는 경우를 차단할 수 있음*/
        public void Acquire()
        {
            /* while(_locked)
             {
                 // 잠김이 풀리기를 기다리는 행위
             }

             // 내꺼다
             _locked = true;
             해당 코드는 문제가 생기는 코드임 */

            while (true)
            {
                /*int original = Interlocked.Exchange(ref _locked, 1); // _locked에 1을 넣고 넣기 이전 값이 original로 튀어나오게 됨
                if (original == 0) // 바꾸기 전 이전 값이 0이었으면 아무도 없었던 것임 (1이었으면 원래 있는데 접근하는 경우이므로 기다려야함)
                    break; // 즉 이전에 아무도 없었으면 접근이 가능하게 하는 것
                           // 이전에 _locked는 경합되는 변수이므로 사용하면 안된다고 했지만
                           // original은 원래의 값을 받고 stack에 저장된 값이므로 위처럼 사용해도 상관이 없다*/

                // CAS Compare-And-Swap
                int original = Interlocked.CompareExchange(ref _locked, 1, 0); // _locked와 세번쨰 인자를 비교해서 같으면 1을 넣어주는 함수
                /* if(_locked == 0)
                 *    _locked = 1;
                 *    이것과 같은 코드임 */
                if (original == 0)
                    break;

                // 위의 CompareExchange가 어떻게 작동하는지 헷갈리는 경우 다음과 같이 작성하여 가독성을 높일 수 있다.
                /*int expected = 0;
                int desired = 1;
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;*/

                // 쉬다 올게 -> 2번 방법
                // Thread.Sleep(1); // 무조건 휴식 -> 1ms 정도 쉬고 싶어요
                // Thread.Sleep(0); // 조건부 양보(갑질하는 양보) -> 나보다 우선순위가 낮은 애들한테는 양보 불가(우선 순위가 나보다 높은 쓰레드가 없으면 나에게 옴)
                Thread.Yield(); // 관대한 양보 -> 지금 실행이 가능한 쓰레드가 있으면 실행하세요(yield == 양보하다) 실행 가능한 애가 없으면 남은 시간 소진
                                // 이렇게 적절하게 쉬다 오는 것이 무거운 프로그램을 돌리고 있다면 나을수도 있음

                // 하지만 다음과 같은 쓰레드를 넘기는 작업은 해당 과정을 거치기에 상당한 부하에 걸리는 것이 일반적이다.
                /* 1. 커널모드로 들어간다.
                 * 2. 레지스터에 있던 정보를 복원 시킨다
                 * 3. 다른 쓰레드에서 실행한다. */
                // 이렇게 어마어마한 부담이 들게 되므로 SpinLock이 더 효율적일 수도 있다는 점이다.
            }
        }

        public void Release()
        {
            _locked = 0;
            // 여기는 별도의 처리를 할 필요도 없는게 Acquire를 진행한 경우 나만 물고 있는 것이므로
            // 그대로 0으로 바꿔주면 알아서 다른 쓰레드에서 처리를 할 수 있다.
        }
    }

    class Program
    {
        static int _num = 0;
        static SpinLock _lock = new SpinLock();

        static void Thread_1()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }
        static void Thread_2()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                _num--;
                _lock.Release();
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