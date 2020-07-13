using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadExample
{
    class Program
    {
        static void MainThread(object state)
        {
            for (int i = 0; i < 5; i++)
                Console.WriteLine("test");
        }

        static void Main(string[] args)
        {
            // 쓰레드 풀이 알아서 관리되는 Task를 사용하면 된다.

            // Task도 Thread Pool에서 관리가 된다.
            //Task t = new Task(() => { while(true){ } }, TaskCreationOptions.LongRunning); // 엄청 오래걸릴거다. //
            //t.Start();

            //백그라운드 스레드
            ThreadPool.SetMinThreads(1, 1);
            // 쓰레드 풀에 설정
            // WorkerThreads -> 
            // completionThreads -> 네트워크 이벤트를 기다리는 쓰레드 설정

            ThreadPool.SetMaxThreads(5, 5);
            for (int i = 0; i < 4; i++) {
                Task t = new Task(() => { while (true) { } }); // 엄청 오래걸릴거다. //
                t.Start();
            }

            ThreadPool.QueueUserWorkItem(MainThread);
            //ThreadPool.SetMaxThreads(5, 5);
            //for (int i = 0; i < 5; i++) //일케하면 먹통
            //    ThreadPool.QueueUserWorkItem((obj) => { while(true) { } });            
            //ThreadPool.QueueUserWorkItem(MainThread);
            // 쓰레드 풀을 사용하면, 공유 자원을 알아서 처리해주는건가?

            //Thread t = new Thread(MainThread);
            ////c# 쓰레드는 기본적으로 forTread로 시작됨
            //// 백그라운드에서 시작하려면
            //// t.IsBackground = true;
            //t.Name = "Test Thread";
            //t.Start();
            //Console.WriteLine("Wait for Thread");
            //t.Join();
            //Console.WriteLine("Hello World");

            while (true) { }
        }
    }
}
