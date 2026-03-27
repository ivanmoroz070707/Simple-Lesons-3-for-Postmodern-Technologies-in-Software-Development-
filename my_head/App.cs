using System;
using System.IO;

namespace my_head
{
    public class App
    {
        public static int Run(string[] args, TextReader inReader, TextWriter outWriter, TextWriter errWriter)
        {
            int linesToRead = 10; // Значення за замовчуванням
            string filePath = null;

            // Парсинг аргументів
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-n")
                {
                    // Перевіряємо, чи є наступний аргумент і чи це число
                    if (i + 1 < args.Length && int.TryParse(args[i + 1], out int parsedN) && parsedN >= 0)
                    {
                        linesToRead = parsedN;
                        i++; // Пропускаємо число, щоб не сприйняти його за файл
                    }
                    else
                    {
                        errWriter.WriteLine("Error: Invalid or missing value for -n option.");
                        return 2; // Неправильні аргументи
                    }
                }
                else if (args[i].StartsWith("-"))
                {
                    errWriter.WriteLine($"Error: Unknown option {args[i]}");
                    return 2; // Неправильні аргументи
                }
                else
                {
                    if (filePath == null)
                    {
                        filePath = args[i];
                    }
                    else
                    {
                        errWriter.WriteLine("Error: Too many arguments. Only one file is supported.");
                        return 2; // Неправильні аргументи
                    }
                }
            }

            // Читання даних
            try
            {
                if (filePath != null)
                {
                    if (!File.Exists(filePath))
                    {
                        errWriter.WriteLine($"Error: File '{filePath}' not found.");
                        return 1; // Часткова помилка (проблема виконання)
                    }

                    using (var reader = new StreamReader(filePath))
                    {
                        ProcessStream(reader, outWriter, linesToRead);
                    }
                }
                else
                {
                    // Якщо файл не вказано, читаємо зі стандартного вводу
                    ProcessStream(inReader, outWriter, linesToRead);
                }

                return 0; // Успішне виконання
            }
            catch (Exception ex)
            {
                errWriter.WriteLine($"Error: {ex.Message}");
                return 1; // Часткова помилка
            }
        }

        private static void ProcessStream(TextReader reader, TextWriter writer, int linesCount)
        {
            string line;
            int count = 0;
            while (count < linesCount && (line = reader.ReadLine()) != null)
            {
                writer.WriteLine(line);
                count++;
            }
        }
    }
}