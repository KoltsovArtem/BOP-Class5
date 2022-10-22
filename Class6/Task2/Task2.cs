using Microsoft.Extensions.Logging;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using System.Diagnostics;

namespace Task2
{
    // Невозможность создать выходной файл
    class UnableCreateFile : Exception
    {
        public UnableCreateFile(string fileName) : base($"Unable to create file {fileName}") { }
    }

    // Отсутствие входного файла
    class MissingInputFile : Exception
    {
        public MissingInputFile(string fileName) : base($"{fileName} not found") { }
    }

    // Некорректный формат строк во входном файлах
    class IncorrectFormatOfLine : Exception
    {
        public IncorrectFormatOfLine(string line, int numberLine) : base($"Incorrect format in line {numberLine}: ${line}") { }
    }

    public class Task2
    {
        private static readonly ILogger<Task2> Logger =
            LoggerFactory.Create(builder => { builder.AddConsole(); }).CreateLogger<Task2>();

        public static int defaultCount = (int)1e5;
        public static string defaultFile = "../../../data/nums.txt";


        private static readonly string logFileName = "../../../data/logFile.txt";

        public static (int, string) ParseArgs(string[] args)
        {
            int count;
            string file;

            try
            {
                count = int.Parse(args[0]);
            }
            catch (Exception)
            {
                Logger.LogError("Wrong count! Default value used");
                count = defaultCount;
            }

            try
            {
                file = args[1];
            }
            catch (Exception)
            {
                file = defaultFile;
            }

            return (count, file);
        }

        public static int gcd(int a, int b)
        {
            if (b == 0)
                return a;

            return gcd(b, a % b);
        }

        internal static void CreateFile(string fileName, int length = 100)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            StreamWriter file;

            try
            {
                file = new StreamWriter(fileName);
            }
            catch (DirectoryNotFoundException)
            {
                throw new UnableCreateFile(fileName);
            }

            for (var i = 0; i < length; i++)
            {
                var num1 = rand.Next((int)1e9);
                var num2 = rand.Next((int)1e9);

                file.WriteLine($"{num1} {num2}");
            }

            file.Close();
        }

        public static void CountGcd(string fileName)
        {
            string[] data;
            var logFile = new StreamWriter(logFileName);

            try
            {
                data = File.ReadAllLines(fileName);
            }
            catch (FileNotFoundException)
            {
                throw new MissingInputFile(fileName);
            }

            for (var i = 0; i < data.Length; i++)
            {
                List<int> nums;

                try
                {
                    nums = data[i].Split(' ').Select(int.Parse).ToList();
                }
                catch (Exception)
                {
                    throw new IncorrectFormatOfLine(data[i], i);
                }

                var startDate = DateTime.Now.Ticks * 100;

                gcd(nums[0], nums[1]);

                var finishDate = DateTime.Now.Ticks * 100;
                logFile.WriteLine(finishDate - startDate);
            }

            logFile.Close();
        }

        public static void AnalizeLogFile()
        {
            var logFile = File.ReadAllLines(logFileName);

            var times = logFile.Select(double.Parse);

            try
            {
                double avgTime = times.Sum() / times.Count();
                Logger.LogInformation($"Average time: {avgTime} nanoseconds");
            }
            catch (DivideByZeroException)
            {
                Logger.LogError("Logfile is empty");
            }
        }

        public static void Main(string[] args)
        {
            Logger.LogInformation("program started");

            var (count, file) = ParseArgs(args);
            CreateFile(file, count);
            CountGcd(file);
            AnalizeLogFile();

            Logger.LogInformation("program completed");
        }
    }
}