using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCoreEx
{
    //    경합조건(Race Condition)

    //여러 쓰레드가 존재한다면 각 쓰레드에서 메모리에 있는 내용을 작업하기 위해서 가져왔을 때
    //같은 내용을 여러번 작업하게 된다는 문제점이 있다(한 쓰레드가 했을 때 나머지 쓰레드가 건들지 말아야 했을텐데)

    //number++의 경우 어셈블리에서 3줄의 코드로 작성된 것이다
    //의사코드로 나타내면
    //int temp = number; temp += 1; number = temp; (temp는 이 때 레지스터)
    //왜 이렇게 3단계로 할까? -> 위의 3단계는 절대로 쪼개질 수 없는 작업이기 때문

    //즉 저 3단계의 작업이 멀티쓰레드에서 작업한다면 number++으로 인해 0에서 1이 되어서 number--에서 1이 0이 되어야 하는데
    //중간의 부분에서 1이 아닌 0을 가져오게 되어서 문제가 생기게 된다

    //이러한 문제점때문에 필요한 특성을 atomic(원자성)이라고 한다(반드시 한번에 실행되어야 하는 부분)
    //즉 number++같은 코드는 한번에 실행되기를 원하는 코드이다


    class Program
    {
        static int number = 0;

        static void Thread_1()
        { // 그렇기 때문에 number++의 경우 Interlocked 계열의 함수를 사용하면 원자성을 보장할 수 있다.
            for (int i = 0; i < 1000000; i++)
                Interlocked.Increment(ref number); // 원래 number++;
            // 이에 따른 단점은 성능에 있어서 엄청난 손해를 볼 수 있다는 점이다
        }

        static void Thread_2()
        {
            for (int i = 0; i < 1000000; i++)
                Interlocked.Decrement(ref number); // 원래 number--;
            // 참고로 Interlock 계열은 가시성 문제가 일어나지 않아서 volatile을 적용하지 않아도 된다
            // 순서를 보장해서 최종 승자가 결정 되므로 race condition 문제를 해결해줄 수 있음
        }

        // 참고로 ref로 넣는 이유는 주소값을 넣어주는 것인데 이는 해당 변수의 값은 모르겠지만 그 해당 주소의 값의 1을 늘린다는 뜻임을 생각해야함
        // 추가로 위의 코드를
        // int prev = number;
        // Interlocked.Increment(ref number);
        // int next = number;
        // 이런 식으로 값이 변화하는 것을 확인할 수 있을까?
        // 불가능함(멀티쓰레드에서는 위에처럼 해당 값의 확인이 불가능하기 때문)
        // int afterValue = Interlocked.Increment(ref number); 와 같이 확인하는 수밖에 없음

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