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
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => { return $"My Name is {Thread.CurrentThread.ManagedThreadId}"; }); // 모든 쓰레드들은 얘를 공유해서 사용 -> ThreadLocal을 씌워서 TLS 영역으로 넣음
        // 쓰레드마다 접근해도 자신만의 공간에 저장이 되서 다른 애들한테 영향 x -> Lock을 걸지 않아도 됨
        // 안에 람다를 넣었는데 ThreadName이 세팅이 안되있으면 안이 실행이 되고 그게 아니라면 원래 밸류를 사용하면 됨

        // static string ThreadName; // 이렇게 하면 모든 쓰레드 네임의 값이 같아져버리게 됨

        // JobQueue에서 일감을 빼올 때 TLS에 최대한 많이 들고와서 작업하면 좋음 

        static void WhoAmI()
        {
            // ThreadName.Value = $"My Name is {Thread.CurrentThread.ManagedThreadId}";

            bool repeat = ThreadName.IsValueCreated; // 이미 만들어져 있었으면 true
            // 위에서 넣은 람다의 체크
            if (repeat)
                Console.WriteLine(ThreadName.Value + " (repeat)");
            else
                Console.WriteLine(ThreadName.Value);

            Thread.Sleep(1000);
        }

        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);
            // Invoke는 넣어준 액션 만큼 Task로 만들어서 실행

            ThreadName.Dispose(); // 쓰레드 네임 날려주기
        }
    }
}