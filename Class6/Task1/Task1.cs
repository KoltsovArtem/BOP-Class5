using Microsoft.Extensions.Logging;

namespace Task1
{
    class UnsupportedOperation : Exception
    {
        UnsupportedOperation(char op) : base($"Operation '{op}' is unsupported")
        {
        }
    }

    class NotEnoughNumbers : Exception
    {
        public NotEnoughNumbers(int available, string line) : base($"More than {available} numbers required in ${line}")
        {
        }
    }

    public class Task1
    {
        private static readonly ILogger<Task1> Logger =
            LoggerFactory.Create(builder => { builder.AddSimpleConsole(); }).CreateLogger<Task1>();

        internal static int ApplyOperation(char op, int arg1, int arg2)
        {
            return TODO<int>();
        }

        private static Func<List<int>, int> ApplySchema(string schema)
        {
            var ops = schema.ToCharArray();
            return args =>
            {
                var res = 0;
                TODO();
                return res;
            };
        }

        private static string FormatLhs(string schema, string[] numbers)
        {
            return TODO<string>();
        }

        internal static string ProcessString(string schema, string input)
        {
            var transformation = ApplySchema(schema);
            var numbers = input.Split(",");
            var result = transformation(numbers.Select(int.Parse).ToList());
            return FormatLhs(schema, numbers) + $"={result}";
        }

        internal static void ProcessFiles(string schemasFile, string dataFile, string outputFile)
        {
            TODO();
        }

        public static void Main(string[] args)
        {
            Logger.LogInformation("program started");
            TODO();
            Logger.LogInformation("program completed");
        }

        private static void TODO()
        {
            throw new NotImplementedException();
        }

        private static T TODO<T>()
        {
            throw new NotImplementedException();
        }
    }
}