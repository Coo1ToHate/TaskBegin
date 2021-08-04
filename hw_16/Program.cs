using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace hw_16
{
    class Program
    {
        static void Main()
        {
            int min = 1_000_000_000;
            int max = 2_000_000_000;

            Console.WriteLine("Результаты без многопоточности:");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Console.WriteLine($"\tКоличество чисел : {Calc(min, max)}");

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";

            Console.WriteLine($"\tЗатраченно времени : {elapsedTime}");

            stopWatch.Reset();
            stopWatch.Start();

            var tasks = TasksCreate(Environment.ProcessorCount*2000, min, max);

            foreach (var t in tasks)
            {
                t.Start();
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Результаты без многопоточности:");

            int count = 0;
            foreach (var t in tasks)
            {
                count += t.Result;
            }

            Console.WriteLine($"\tКоличество чисел : {count}");

            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine($"\tЗатраченно времени : {elapsedTime}");

            Console.ReadKey();
        }

        private static Task<int>[] TasksCreate(int countTasks, int min, int max)
        {
            Task<int>[] tasks = new Task<int>[countTasks];
            int interval = (max - min) / countTasks;


            for (int i = 0; i < countTasks; i++)
            {
                max = min + interval;

                int min1 = min;
                int max1 = max;

                tasks[i] = new Task<int>(() => Calc(min1, max1));
                min = max1;
            }

            return tasks;
        }

        private static int Calc(int min, int max)
        {
            int count = 0;
            int num;
            for (int i = min; i < max; i++)
            {
                num = SumDigit(i);
                int lastNum = i % 10;
                //Console.Write($"num = {num} last = {lastNum} ");

                if (lastNum == 0 || lastNum == 1)
                {
                    count++;
                    continue;
                }
                if (num % lastNum == 0)
                {
                    //Console.Write(i);
                    count++;
                }

                //try
                //{
                //    if (num % lastNum == 0)
                //    {
                //        //Console.Write(i);
                //        count++;
                //    }
                //}
                //catch (DivideByZeroException)
                //{
                //    //Console.Write(i);
                //    count++;
                //}

                //Console.WriteLine();
            }
            return count;
        }

        private static int SumDigit(int num)
        {
            int result = 0;
            while (num != 0)
            {
                result += num % 10;
                num /= 10;
            }
            return result;
        }

        static void GetThreadPoolInformation()
        {
            int workerThreadsAvailable, completionPortThreadsAvailable;
            ThreadPool.GetAvailableThreads(out workerThreadsAvailable, out completionPortThreadsAvailable);

            int workerThreadsMax, completionPortThreadsMax;
            ThreadPool.GetMaxThreads(out workerThreadsMax, out completionPortThreadsMax);

            Console.WriteLine($"Доступно потоков в ThreadPool: {workerThreadsAvailable} из {workerThreadsMax}");
            Console.WriteLine($"Доступно потоков для ввода-вывода: {completionPortThreadsAvailable} из {completionPortThreadsMax}");
            Console.WriteLine();
        }
    }
}
