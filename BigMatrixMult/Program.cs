using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BigMatrixMult
{
    class Program
    {
        static Random rnd = new Random();

        static void Main()
        {
            int n1 = 1000;
            int m1, n2;
            m1 = n2 = 2000;
            int m2 = 3000;
            //Создаем две матрицы
            int[,] firstMatrix = CreateMatrix(n1, m1);
            int[,] secondMatrix = CreateMatrix(n2, m2);

            //Очищаем консоль
            ClearConsole();

            Console.WriteLine("Результат без многопоточности:");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            //int[,] multiMatrix = MatrixMultiplication(firstMatrix, secondMatrix);

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";

            Console.WriteLine($"\tЗатраченно времени : {elapsedTime}");

            Console.WriteLine();
            //PrintMatrix(multiMatrix);
            Console.WriteLine();

            stopWatch.Reset();
            Console.WriteLine("Результат c многопоточностью:");
            stopWatch.Start();

            int[,] multiMatrix2 = MatrixMultiplicationTask(8000, firstMatrix, secondMatrix);

            stopWatch.Stop();
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";

            Console.WriteLine($"\tЗатраченно времени : {elapsedTime}");

            Console.WriteLine();
            //PrintMatrix(multiMatrix2);

            Console.ReadKey();
        }

        /// <summary>
        /// Метод умножения матрицы на матрицу
        /// </summary>
        /// <param name="a">матрица множитель</param>
        /// <param name="b">матрица сомножитель</param>
        /// <returns></returns>
        private static int[,] MatrixMultiplication(int[,] a, int[,] b)
        {
            int aRows = a.GetUpperBound(0) + 1;  // Определяем кол-во строк первой матрицы (индекс последнего элемента + 1)
            int aCols = a.GetUpperBound(1) + 1;  // Определяем кол-во столбцов первой матрицы
            int bRows = b.GetUpperBound(0) + 1;  // Определяем кол-во строк второй матрицы
            int bCols = b.GetUpperBound(1) + 1;  // Определяем кол-во столбцов второй матрицы

            var multiMatrix = new int[aRows, bCols];

            //Умножение матриц
            for (int i = 0; i < aRows; i++)
            {
                for (int j = 0; j < bCols; j++)
                {
                    for (int k = 0; k < bRows; k++)
                    {
                        multiMatrix[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return multiMatrix;
        }

        private static int[,] MatrixMultiplicationTask(int countTasks, int[,] a, int[,] b)
        {
            List<Task> tasks = new List<Task>();

            int aRows = a.GetUpperBound(0) + 1;
            int aCols = a.GetUpperBound(1) + 1;
            int bRows = b.GetUpperBound(0) + 1;
            int bCols = b.GetUpperBound(1) + 1;

            int[,] result = new int[aRows, bCols];

            int min = 0;
            int max = aRows;
            
            while (max / countTasks == 0)
            {
                countTasks--;
            }

            int interval = max / countTasks == 0 ? max : max / countTasks;


            int min1 = min;
            do
            {
                int max1 = min1 + interval;
                if (max1 > max) max1 = max;

                int min2 = min1;
                int max2 = max1;

                tasks.Add(new Task(() => Mult(min2, max2, a, b, result)));
                min1 = max2;

            } while (min1 < max);


            foreach (var t in tasks)
            {
                t.Start();
            }

            Task.WaitAll(tasks.ToArray());

            return result;
        }

        private static void Mult(int min, int max, int[,] a, int[,] b, int[,] result)
        {
            int aRows = a.GetUpperBound(0) + 1;
            int aCols = a.GetUpperBound(1) + 1;
            int bRows = b.GetUpperBound(0) + 1;
            int bCols = b.GetUpperBound(1) + 1;

            for (int i = min; i < max; i++)
            {
                for (int j = 0; j < bCols; j++)
                {
                    for (int k = 0; k < bRows; k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
        }

        /// <summary>
        /// Метод очистки консоли
        /// </summary>
        private static void ClearConsole()
        {
            Console.Clear();
        }

        /// <summary>
        /// Метод создания матрицы
        /// </summary>
        /// <returns></returns>
        private static int[,] CreateMatrix(int n = 0, int m = 0)
        {
            //int n, m;
            if (n == 0)
            {
                while (true)
                {
                    Console.Write("Число строк матрицы: ");
                    if (int.TryParse(Console.ReadLine(), out n) && n < 500)
                    {
                        break;
                    }
                    Console.WriteLine("Введите число больше 500!");
                }
            }

            if (m == 0)
            {
                while (true)
                {
                    Console.Write("Число столбцов матрицы: ");
                    if (int.TryParse(Console.ReadLine(), out m) && m < 500)
                    {
                        break;
                    }
                    Console.WriteLine("Введите число больше 500!");
                }
            }


            //Создаем матрицу
            int[,] matrix = new int[n, m];

            //Заполняем матрицу случайными числами
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    matrix[i, j] = rnd.Next(0, 2);
                }
            }
            return matrix;
        }

        private static void PrintMatrix(int[,] matrix)
        {
            int n = matrix.GetUpperBound(0) + 1;
            int m = matrix.GetUpperBound(1) + 1;

            //если выводимый результат шире окна консоли, добавляем скрол
            if ((2 + m * 2) * 4 > Console.WindowWidth)
            {
                Console.BufferWidth = (2 + m * 2) * 4;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write($"{matrix[i, j],5}");
                }
                Console.WriteLine();
            }
        }
    }
}
