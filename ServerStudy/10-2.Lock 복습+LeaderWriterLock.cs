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
        static object _lock = new object();
        static SpinLock _lock2 = new SpinLock();

        // 아래의 특수한 경우에만 lock을 작동시키는 것이 RWLock ReaderWriteLock
        static ReaderWriterLockSlim _lock3 = new ReaderWriterLockSlim();

        // [] [] [] [] ([] []) -> 아이템 보상이  추가적으로 더 뿌린다 치면
        class Reward
        {
            // 어쩌구
        }


        static Reward GetRewardById(int id)
        {
            _lock3.EnterReadLock();

            _lock3.ExitReadLock();

            // 여러 쓰레드가 위의 락을 사용할 수 있다

            /*lock(_lock) // 누가 보상을 더 추가적으로 넣는다면 문제가 생길 수 있으므로 락을 걸어야함
            {

            } // 대부분의 경우 변함이 없지만 진짜 가끔 바뀔때를 대비해서 lock을 거는게 아쉽다
            // -> 바꿀때만 막을 수 있으면 효율적일듯 (일반적일땐 lock이 없는 느낌)*/
            return null;
        }

        static void AddReward(Reward reward)
        {
            _lock3.EnterWriteLock();

            _lock3.ExitWriteLock();
            // Write를 하는 경우에만 여기서 락을 걸음 (위는 접근 불가)


            /* lock (_lock)
            {
            // 원래 락을 거는 방식
            }*/
        }

        static void Main(string[] args)
        {
            // 첫번째 버전을 사용하면
            lock (_lock)
            {
                // 이렇게 편하게 사용할 수 있다
            }
        }
    }
}