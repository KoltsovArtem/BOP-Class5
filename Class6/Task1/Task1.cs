using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Task1
{
    // Передано недостаточное число аргументов
    class NotEnoughArguments : Exception
    {
        public NotEnoughArguments(int available) : base($"More than {available} arguments required") { }
    }

    // Некорректность схемы преобразования
    class UnsupportedOperation : Exception
    {
        public UnsupportedOperation(char op) : base($"Operation '{op}' is unsupported") { }
    }

    // Отсутствие входного файла
    class MissingInputFile : Exception
    {
        public MissingInputFile(string fileName) : base($"{fileName} not found") { }
    }

    // Разное количество строк во входных файлах
    class DifferentNumberOfLines : Exception
    {
        public DifferentNumberOfLines(int available, string fileName) : base($"More than {available} lines required in {fileName}") { }
    }

    // Некорректный формат строк во входных файлах
    class IncorrectFormatOfLine : Exception
    {
        public IncorrectFormatOfLine(string line, int numberLine) : base($"Incorrect format in line {numberLine}: ${line}") { }
    }

    /* Несоответствие количества чисел в соответствующей 
     * строке второго файла выполняемому преобразованию */
    class NotEnoughNumbers : Exception
    {
        public NotEnoughNumbers(int available, string line, int numberLine) : base($"More than {available} numbers required in line {numberLine}: ${line}") { }
    }

    // Невозможность создать выходной файл
    class UnableCreateFile : Exception
    {
        public UnableCreateFile(string fileName) : base($"Unable to create file {fileName}") { }
    }

    public class Task1
    {
        private static readonly ILogger<Task1> Logger =
            LoggerFactory.Create(builder => { builder.AddSimpleConsole(); }).CreateLogger<Task1>();

        //Выполнить операцию+
        internal static int ApplyOperation(char op, int arg1, int arg2)
        {
            var functions = new Dictionary<char, Func<int, int, int>>
            {
                {'*', (x, y) => x * y},
                {'/', (x, y) => x / y},
            };

            try
            {
                return functions[op](arg1, arg2);
            }

            catch (KeyNotFoundException)
            {
                throw new UnsupportedOperation(op);
            }
        }

        //Преобразование схемы+
        private static Func<List<int>, int> ApplySchema(string schema)
        {
            var ops = schema.ToCharArray();

            return args =>
            {
                int res = args[0];

                for (var i = 0; i < schema.Length; i++)
                {
                    res = ApplyOperation(ops[i], res, args[i + 1]);
                }

                return res;
            };
        }

        //Форматирование операций+
        public static string FormatLhs(string schema, string[] numbers)
        {
            var result = new StringBuilder();
            result.Append(numbers[0]);

            for (var i = 0; i < schema.Length; i++)
            {
                result.Append(schema[i]);
                result.Append(numbers[i + 1]);
            }

            return result.ToString();
        }

        // Обработка строки
        internal static string ProcessString(string schema, string input, int line = -1)
        {
            var transformation = ApplySchema(schema);
            var numbers = input.Split(",");
            var result = "";

            try
            {
                result = $"={transformation(numbers.Select(int.Parse).ToList())}";
            }

            catch (DivideByZeroException)
            {
                result = " *** DIVBYZERO";
            }

            catch(ArgumentOutOfRangeException)
            {
                throw new NotEnoughNumbers(numbers.Length, input, line);
            }

            catch (FormatException)
            {
                throw new IncorrectFormatOfLine(input, line);
            }


            return FormatLhs(schema, numbers) + $"{result}";
        }

        // Чтение файла
        public static string[] ReadFile(string fileName)
        {
            try
            {
                var data = File.ReadAllLines(fileName);
                return data;
            }

            catch (FileNotFoundException)
            {
                throw new MissingInputFile(fileName);
            }
        }

        // Запись в файл
        public static void WriteFile(string fileName, List<string> data)
        {
            try
            {
                var file = new StreamWriter(fileName);

                foreach (var line in data)
                {
                    file.WriteLine(line);
                }

                file.Close();
            }

            catch (DirectoryNotFoundException)
            {
                throw new UnableCreateFile(fileName);
            }
        }

        public static void CheckLengths(string[] schema, string schemaFile, string[] data, string dataFile)
        {
            if (schema.Length < data.Length)
            {
                throw new DifferentNumberOfLines(schema.Length, schemaFile);
            }

            if (data.Length < schema.Length)
            {
                throw new DifferentNumberOfLines(data.Length, dataFile);
            }
        }

        // Обработка файлов
        internal static void ProcessFiles(string schemaFile, string dataFile, string outputFile)
        {
            var schema = ReadFile(schemaFile);
            var data = ReadFile(dataFile);
            var result = new List<string>();

            CheckLengths(schema, schemaFile, data, dataFile); 

            for (int i = 0; i < schema.Length; i++)
            {
                result.Add(ProcessString(schema[i], data[i], i));
            }

            WriteFile(outputFile, result);
        }

        // Обработка аргументов
        public static (string, string, string) ParseArgs(string[] args)
        {
            try
            {
                var schemaFile = args[0];
                var dataFile = args[1];
                var outputFile = args[2];

                return (schemaFile, dataFile, outputFile);
            }

            catch (IndexOutOfRangeException)
            {
                throw new NotEnoughArguments(args.Length);
            }
        }

        public static void Main(string[] args)
        {
            Logger.LogInformation("program started");

            try
            {
                var (schemaFile, dataFile, outputFile) = ParseArgs(args);
                ProcessFiles(schemaFile, dataFile, outputFile);
            }

            catch (Exception exception)
            {
                Logger.LogError(exception.Message);
            }

            finally
            {
                Logger.LogInformation("program completed");
            }
        }
    }
}