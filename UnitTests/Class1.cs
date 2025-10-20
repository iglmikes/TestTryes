using System;
using System.Threading;

class ExampleSemaphores
{
    static void Main()
    {
        // Создаем семафор, допускающий максимум 2 потока одновременно
        Semaphore semaphore = new Semaphore(initialCount: 2, maximumCount: 2);

        for (int i = 0; i < 5; i++)
        {
            Thread thread = new Thread(() =>
            {
                // Запрашиваем разрешение на доступ
                semaphore.WaitOne();
                try
                {
                    Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: начал работу.");
                    Thread.Sleep(1000); // Имитация длительной работы
                    Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: завершил работу.");
                }
                finally
                {
                    // Освобождаем семафор
                    semaphore.Release();
                }
            });
            thread.Start();
        }
    }
}