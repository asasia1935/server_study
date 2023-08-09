using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCoreEx
{
    /*// 재귀적 락 허용 x (재귀적 lock에서 필요한 것이 WriteThreadId임)
    // 스핀락 정책 (5000번 후에 Yield)
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000; // 앞에 15비트만 추출하는 마스크
        const int READ_MASK = 0x0000FFFF; // 뒤에 16비트만 추출하는 마스크
        const int MAX_SPIN_COUNT = 5000;

        // 구조 -> [Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
        int _flag = EMPTY_FLAG; // 시작 값을 0으로 맞춤

        public void WriteLock()
        {
            // 아무도 WriteLock or ReadLock을 획득하고 있지 않을 때, 경합해서 소유권을 얻는다.
            // Thread.CurrentThread.ManagedThreadId는 1부터 증가하는 숫자(쓰레드 구분에 편함)
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK; // 가운데 부분만 남고 0으로 밀어서 15비트 부분에 밀어넣음
            while(true)
            {
                for(int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG);
                        return;
                    // 시도를 해서 성공하면 return
                    //if (_flag == EMPTY_FLAG) // 아무도 획득하지 않는 상태
                    //    _flag = desired;
                    // 위의 코드는 원자성이 보장되어있지 않음 (멀티쓰레드에서 안됨)
                    // 그래서 더 위처럼 인터락을 사용하여 동시에 두 쓰레드가 들어올 수 없게 함
                }

                Thread.Yield();
            }
        }

        public void WriteUnlock()
        {
            // WriteLock을 한 쓰레드만 가능할 수 있음
            Interlocked.Exchange(ref _flag, EMPTY_FLAG); // flag를 EMPTY_FLAG로 대체
        }

        public void ReadLock()
        {
            // 아무도 WriteLock을 획득하고 있지 않으면, ReadCount를 1 늘린다
            // 15 비트 부분이 0이면 16비트 부분에 +1;
            while(true)
            {
                for(int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    *//*if((_flag & WRITE_MASK) == 0)
                    {
                        _flag = _flag + 1;
                        return;
                        // 이 코드 역시 원자성이 보장되지 않음
                    }*//*
                    int expected = (_flag & READ_MASK); // 왜 위에서는 WRITE_MASK를 씌웠는데 여기서는 READ_MASK인가
                    // expected는 _flag가 될 값을 지정해야함 이 때 15비트 부분이 다 이 되어 있어야 함
                    // 그렇기에 READ_MASK를 씌워서 그 값을 만든 후에 그 값이 같으면 +1을 시켜줘야함
                    // 참고로 반환값은 바뀌기 이전 값이므로 expected가 들어가야함
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return; // A(0 -> 1) B(0 -> 1)을 실행한다면 A가 먼저 실행되었을때 flag는 1이 되고 A는 성공 후에 B의 요청은 실패를 하게 됨 (flag가 0이 아니므로)
                    *//* int result = Interlocked.CompareExchange(ref A, int B, int C)
 
                        인자 1 : 인자3과 비교해서 같으면 인자2로 바뀜
                        인자 2 : 바뀔 값
                        인자 3 : 인자 1과 비교되는 값
                        반환 결과 : 바뀌기 이전 값
                    *//*
                }

                Thread.Yield();
            }
        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }
    }*/

    // 재귀적 락 허용 o (재귀적 lock에서 필요한 것이 WriteThreadId임)
    // WriteLock->WriteLock OK, WriteLock->ReadLock OK, ReadLock->WriteLock NO
    // 스핀락 정책 (5000번 후에 Yield)
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000; // 앞에 15비트만 추출하는 마스크
        const int READ_MASK = 0x0000FFFF; // 뒤에 16비트만 추출하는 마스크
        const int MAX_SPIN_COUNT = 5000;

        // 구조 -> [Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
        int _flag = EMPTY_FLAG; // 시작 값을 0으로 맞춤
        int _writeCount = 0;

        public void WriteLock()
        {
            // 동일 쓰레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16; // 가운데 15비트만 추출
            if(Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                _writeCount++;
                return;
            }

            // 아무도 WriteLock or ReadLock을 획득하고 있지 않을 때, 경합해서 소유권을 얻는다.
            // Thread.CurrentThread.ManagedThreadId는 1부터 증가하는 숫자(쓰레드 구분에 편함)
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK; // 가운데 부분만 남고 0으로 밀어서 15비트 부분에 밀어넣음
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                    // 시도를 해서 성공하면 return
                    //if (_flag == EMPTY_FLAG) // 아무도 획득하지 않는 상태
                    //    _flag = desired;
                    // 위의 코드는 원자성이 보장되어있지 않음 (멀티쓰레드에서 안됨)
                    // 그래서 더 위처럼 인터락을 사용하여 동시에 두 쓰레드가 들어올 수 없게 함
                }

                Thread.Yield();
            }
        }

        public void WriteUnlock()
        {
            int lockCount = --_writeCount;
            if(lockCount == 0) // 지금까지 걸은 락 만큼 풀어줘야함
                // WriteLock을 한 쓰레드만 가능할 수 있음
                Interlocked.Exchange(ref _flag, EMPTY_FLAG); // flag를 EMPTY_FLAG로 대체
        }

        public void ReadLock()
        {
            // 동일 쓰레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16; // 가운데 15비트만 추출
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Increment(ref _flag); // 1을 늘리면 카운트가 증가하게 된다 (구조 확인)
                return;
            }

            // 아무도 WriteLock을 획득하고 있지 않으면, ReadCount를 1 늘린다
            // 15 비트 부분이 0이면 16비트 부분에 +1;
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    /*if((_flag & WRITE_MASK) == 0)
                    {
                        _flag = _flag + 1;
                        return;
                        // 이 코드 역시 원자성이 보장되지 않음
                    }*/
                    int expected = (_flag & READ_MASK); // 왜 위에서는 WRITE_MASK를 씌웠는데 여기서는 READ_MASK인가
                    // expected는 _flag가 될 값을 지정해야함 이 때 15비트 부분이 다 이 되어 있어야 함
                    // 그렇기에 READ_MASK를 씌워서 그 값을 만든 후에 그 값이 같으면 +1을 시켜줘야함
                    // 참고로 반환값은 바뀌기 이전 값이므로 expected가 들어가야함
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return; // A(0 -> 1) B(0 -> 1)을 실행한다면 A가 먼저 실행되었을때 flag는 1이 되고 A는 성공 후에 B의 요청은 실패를 하게 됨 (flag가 0이 아니므로)
                    /* int result = Interlocked.CompareExchange(ref A, int B, int C)
 
                        인자 1 : 인자3과 비교해서 같으면 인자2로 바뀜
                        인자 2 : 바뀔 값
                        인자 3 : 인자 1과 비교되는 값
                        반환 결과 : 바뀌기 이전 값
                    */
                }

                Thread.Yield();
            }
        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }

        // 라이트락을 한 후에 리드락을 했다면 리드언락을 한 후에 라이트 언락을 해야한다(순서를 지켜야함)
    }
}
