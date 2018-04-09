using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace nival_testing
{
    class Program
    {
        static void Main(string[] args)
        {
            /**
             * Если программе передан 1 параметр и 
             * соответствующая директория существует,
             * начинаем выполнение программы.
             */
            if (args.Length == 1 && Directory.Exists(args[0]))
            {
                /**
                 * запуск таймера работы программы.
                 * получение путей до всех *.xml файлов
                 * создание списка потоков для обработки
                 * файлов.
                 */
                DateTime programStartTime = DateTime.Now;
                string[] filepaths = Directory.GetFiles(args[0], "*.xml");
                var tasks = new List<Task<TaskOutput>>();

                /**
                 * Запуск создание и запуск потоков
                 */
                foreach (var filepath in filepaths)
                {
                    tasks.Add(Task.Factory.StartNew(() => TaskFunc(filepath)));
                }
                Task.WaitAll(tasks.ToArray());

                /**
                 * Когда все потоки закончили выполняться,
                 * разбираем результаты их работы,
                 * находим файл с наибольшим количеством
                 * успешных десериализаций.
                 * Выводим всю информацию записанную
                 * классом Logger.
                 * Завершаем отсчет времени выполнения
                 * программы.
                 */
                int mostCompletedCalculations = 0; 
                string mostCompletedFile = "null"; 
                foreach (var task in tasks)
                {
                    if (mostCompletedCalculations < task.Result.completed)
                    {
                        mostCompletedCalculations = task.Result.completed;
                        mostCompletedFile = task.Result.filepath;
                    }

                    if (task.Result.logs.Count > 0)
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkGreen;

                    Console.WriteLine("[{0}]", task.Result.filepath);
                    Console.WriteLine(" >> результат вычислений {0}.", task.Result.result);

                    if (task.Result.logs.Count > 0)
                    {
                        foreach (var log in task.Result.logs)
                        {
                            Console.WriteLine(" >> {0}", log);
                        }
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[Работа программы завершена]");
                Console.WriteLine(" >> Наибольшее количество успешно десериализованных элементов в файле:\n   \"{0}\".", mostCompletedFile);
                Console.WriteLine(" >> Время выполнения программы {0} миллисекунд.", (DateTime.Now - programStartTime).TotalMilliseconds.ToString());
            } 
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Неверный параметр запуска!");
            }
            Console.WriteLine("\nНажмите любую клавишу для выхода.");
            Console.ReadKey();
        }
        
        static TaskOutput TaskFunc(string filepath)
        {
            /**
             * Создаем объект CalculationReader
             * и запускаем чтение документа
             */
            CalculationReader reader = new CalculationReader(filepath);
            reader.ParseCalculations();

            /**
             * После завершения чтения файла
             * имеем список удачно десериализованных
             * объектов вычислительных операций,
             * производим подсчет результатов.
             */
            int result = 0;
            foreach (var calculation in reader.calculations)
            {
                switch (calculation.operand)
                {
                    case Operand.add:
                        result += calculation.mod;
                        break;
                    case Operand.subtract:
                        result -= calculation.mod;
                        break;
                    case Operand.multiply:
                        result *= calculation.mod;
                        break;
                    case Operand.divide:
                        if (calculation.mod != 0)
                            result /= calculation.mod;
                        break;
                }
            }

            return new TaskOutput
            {
                filepath = filepath,
                result = result,
                completed = reader.calculations.Count,
                logs = reader.logger.GetMessages()
            };
        }
    }
}
