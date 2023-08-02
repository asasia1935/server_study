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
        static void Main(string[] args)
        {
            int[,] arr = new int[10000, 10000];

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++)
                        arr[y, x] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(y, x) 순서 걸린 시간 {end - now}");
            }

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++)
                        arr[x, y] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(y, x) 순서 걸린 시간 {end - now}");
            }

            // 두 저장하는 시간은
            // (y, x) 순서 걸린 시간 4177814
            // (y, x) 순서 걸린 시간 4978915
            // 둘의 시간 차이가 날 수 밖에 없다 -> 공간적인 문제떄문

            // [][][][][][] [][][][][][] [][][][][][] [][][][][][] [][][][][][]
            // 5x5 배열이 있다 치면 x를 먼저 증가 시켰을때 순차적으로 넣는 것이고
            // y를 먼저 증가 시켰을때 따로따로 넣는 것이므로 시간의 차이가 있을수밖에 없다
        }
    }
}