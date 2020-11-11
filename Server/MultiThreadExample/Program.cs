using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreadExample
{
    class Program
    {
        public static int x = 0;
        public static int y = 0;
        public static int r1 = 0;
        public static int r2 = 0;

        static void Main(string[] args)
        {
            //var mb = new MemoryBarrier();
            //mb.StartThread();

            //var t = new InterLockedTest();
            //t.StartThread();

            //var t = new ThreeTypeLock();
            //t.StartThread();
        }

        static void ChashTest()
        {
            int xx = 10000;
            int yy = 10000;

            int[,] arr = new int[xx, yy];

            {
                long now = DateTime.Now.Ticks;
                for (int x = 0; x < 10000; x++) {
                    for (int y = 0; y < 10000; y++) {
                        arr[y, x] = 1;
                    }
                }

                for (int x = 0; x < 10000; x++) {
                    for (int y = 0; y < 10000; y++) {
                        arr[x, y] = 1;
                    }
                }

                // 속도가 다르다.
                // 캐쉬가 인접한 공간에 위치 해야 빠름, Spaacial Locality에 의해서 
            }
        }

        public class MemoryBarrier
        {
            // 하드웨어도 맘대로 최적화를한다.
            // 그걸 막는다
            // 코드 재배치를 막도록한다.
            // 가시성을 도와준다.
            // 메모리 베리어는 무었인가?


            // Full Memory Barrier (ASM MFENCE, C# Thread.MemoryBarrier) : Store/Load 둘 다 막는다.
            // Store Memory Barrier (ASM SFENCE) Store만 막는다
            // Load Memory Barrier (ASM FEMCE) Load만 막는다.

            public void Thread1()
            {
                y = 1; // Store

                Thread.MemoryBarrier();

                r1 = x; // Load
            }

            public void Thread2()
            {
                x = 1; // Store

                Thread.MemoryBarrier();

                r2 = y; // Load
            }

            public void StartThread()
            {
                int count = 0;
                while (true) {

                    count++;
                    x = y = r1 = r2 = 0;

                    Task t1 = new Task(Thread1);
                    Task t2 = new Task(Thread1);

                    t1.Start();
                    t2.Start();

                    Task.WaitAll(t1, t2);

                    if (r1 == 0 && r2 == 0) {
                        break;
                    }
                }

                Console.WriteLine($"{count} 번만에 빠져나옴");
            }

            int _answer;
            bool _complete;

            public void A()
            {
                _answer = 123;
                Thread.MemoryBarrier();
                _complete = true;
                Thread.MemoryBarrier();
            }

            public void B()
            {
                Thread.MemoryBarrier();
                if (_complete) {
                    Thread.MemoryBarrier();
                }
            }
        }


        // 인터락
        public class InterLockedTest
        {
            //레이스 컨디션
            static int number = 0;

            static void Thread1()
            {
                for (int i = 0; i < 100; i++) {

                    // All or Nothing
                    Interlocked.Increment(ref number); // 원자 성을 보호해줌
                                                       // 단점 : 성능에서 어마어마하게 손해를 본다.
                                                       // 메모리 배리어를 하고 있다.
                                                       // number++;
                                                       // -> sudo code Assemble 어셈블리에선 3줄임
                                                       // int temp = number;
                                                       // temp += 1;
                                                       // number = tmep;
                }

                // actomic == 원자성
            }

            static void Thread2()
            {
                for (int i = 0; i < 100; i++) {
                    Interlocked.Decrement(ref number);

                    // number--;
                    // -> sudo code Assemble 어셈블리에선 3줄임
                    // int temp = number;
                    // temp -= 1;
                    // number = tmep;
                }
            }

            public void StartThread()
            {
                var t1 = new Task(Thread1);
                var t2 = new Task(Thread2);

                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                Console.WriteLine(number);
            }
        }

        // Lock 기초
        public class LockBase
        {
            static int number = 0;

            static object _obj = new object();

            static void Thread1()
            {
                for (int i = 0; i < 10000; i++) {

                    // 상호배제 Mutual Exclusive
                    Monitor.Enter(_obj); // 문을 잠구는 행위 // -> c++ CriticalSection, std::mutex
                    {
                        number++;
                    }
                    Monitor.Exit(_obj); // 여기서 만약 해제하는 걸 잊는다면? 다른 쓰레드가 무한 대기상태가된다. -> Dead Lock
                }

                //try {} finally {}를 사용하는 게 좋다. -> 파이널리는 리턴을 해도 무조건 불린다.
                // 보단 Lock을 사용하자.


                //락 내부도 모니터로 구현되어있다.
                lock (_obj) {
                    number++;
                }
            }

            static void Thread2()
            {
                for (int i = 0; i < 10000; i++) {
                    Monitor.Enter(_obj); // 문을 잠구는 행위
                    number--;
                    Monitor.Exit(_obj);
                }
            }

            public void StartThread()
            {
                var t1 = new Task(Thread1);
                var t2 = new Task(Thread2);

                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                Console.WriteLine(number);
            }
        }

        public class ThreeTypeLock
        {
            public class SpinLock
            {
                volatile bool _locked = false;
                readonly object _lock = new object();

                public void Aquire()
                {
                    lock (_lock) {
                        while (_locked) {
                            // 잠김이 풀리기를 기다린다.
                        }

                        _locked = true;
                    }
                }

                public void Rlease()
                {
                    _locked = false;
                }
            }

            public class SpinLock1
            {
                volatile int _locked;

                public void Aquire()
                {
                    // Interlocked는 변경되기 이전 값을 리턴해준다.
                    // 결국 이 while문의 의미는, 값이 1이 리턴된 것은 다른 곳에서 1로 바꾸었다는 것
                    //while(Interlocked.Exchange(ref _locked, 1) == 1) { 

                    //}
                    // 매커니즘 차이
                    // 

                    // CAS Compare-And-Swap
                    int expected = 0;
                    int desired = 1;

                    while (Interlocked.CompareExchange(ref _locked, desired, expected) == 1) {

                    }
                }

                public void Rlease()
                {
                    _locked = 0;
                }
            }

            public class EventLock
            {
                // 커널용 bool
                // 근데 이거 커널용이라 매우 오래걸린다.
                AutoResetEvent _available = new AutoResetEvent(true);

                public void Acquire()
                {
                    _available.WaitOne();
                }

                public void Release()
                {
                    _available.Reset(); // flag = true
                }
            }

            // Mutex는 AutoResetEvent랑 비슷한데, 몇 번잠궜는지 카운팅되고, 쓰레드의 인덱스 값을 가지고 있다. 
            public class MutexLock
            {
                Mutex mu = new Mutex();

                public void Acquire()
                {
                    mu.WaitOne();
                }

                public void Release()
                {
                    mu.ReleaseMutex(); // flag = true
                }
            }


            int testCount = 0;
            readonly SpinLock1 sl = new SpinLock1();

            void Thread0()
            {
                for (int i = 0; i < 100000; i++) {
                    sl.Aquire();
                    testCount++;
                    sl.Rlease();
                }
            }

            void Thread1()
            {
                for (int i = 0; i < 100000; i++) {
                    sl.Aquire();
                    testCount--;
                    sl.Rlease();
                }
            }

            public void StartThread()
            {
                var t1 = new Task(Thread0);
                var t2 = new Task(Thread1);

                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                Console.WriteLine($"Count : {testCount}");
            }
        }

        public class LockLibary
        {

            static object _lock = new object();
            //내부에선 Sleep도 하고 있다.
            static SpinLock _spinLock = new SpinLock();

            // 장점은 프로그램 사이에서 동기화를 기다릴 수 있다.
            static Mutex _mutexLock = new Mutex();

            public static void Test()
            {
                lock (_lock) {

                }

                bool lockTaken = false;
                try {
                    _spinLock.Enter(ref lockTaken);
                } finally {
                    if (lockTaken)
                        _spinLock.Exit();
                }
            }
        }

        // 재귀적 락 허용 x 재귀적 허용이란? WriteLock -> WriteLock 가능 WriteLock -> ReadLock 가능 ReadLock -> Write 불가능
        // 스핀락 정책 (5000번 -> Yield)
        // 락프리 기초

        // Write는 한번에 한 쓰레드만 가능 
        // Read는 맘대로 가능 ReadCount는 그냥 접근한 횟수를 저장함
        public class ReadWriteLock
        {

            const int EMPTY_FLAG = 0x00000000;
            const int WRITE_FLAG = 0x7FFF0000;
            const int READ_FLAG = 0x0000FFFF;
            const int MAX_SPINE_COUNT = 5000;

            //[Unused(1)] [WriteTreadId(15)] [ReadCount(16)]
            int _flag = EMPTY_FLAG;
            int writeCount = 0;
            public void WirteLock()
            {
                // 동일 쓰레드가 WriteLock을 이미 획득하고 있는지 확인
                int LockThreadId = (_flag & WRITE_FLAG) >> 16;
                if (LockThreadId == Thread.CurrentThread.ManagedThreadId) {
                    writeCount++;
                    return;
                }

                var desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_FLAG;
                while (true) {
                    for (int i = 0; i < MAX_SPINE_COUNT; i++) {
                        //_flag 값을 desired 값으로 수정한 한다. _flag의 예상하는 값은EMPTY_FLAG다. 그리고 이전 값이 EMPTY_FLAG면 return;
                        if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG) {
                            writeCount = 1;
                            return;
                        }
                    }

                    Thread.Yield();
                }
            }

            public void WriteUnLock()
            {
                if (--writeCount == 0)
                    Interlocked.Exchange(ref _flag, EMPTY_FLAG);
            }

            public void ReadLock()
            {
                // 동일 쓰레드가 WriteLock을 이미 획득하고 있는지 확인
                int LockThreadId = (_flag & WRITE_FLAG) >> 16;
                if (LockThreadId == Thread.CurrentThread.ManagedThreadId) {
                    Interlocked.Increment(ref _flag);
                    return;
                }

                while (true) {
                    int expected = (_flag & READ_FLAG);
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected) {
                        return;
                    }
                    Thread.Yield();
                }
            }

            public void Rock()
            {
                Interlocked.Decrement(ref _flag);
            }
        }
    }
}
