using NUnit.Framework;
using static NUnit.Framework.Assert;
using static Task2.Task2;

namespace Task2;

public class Tests
{
    [Test]
    public static void ParseArgsTest()
    {
        var data = new string[] { "1000", "../../../data/y.txt"};
        var errorData = new string[] { "a" };

        That(ParseArgs(data), Is.EqualTo((int.Parse(data[0]), data[1])));
        That(ParseArgs(errorData), Is.EqualTo((defaultCount, defaultFile)));
    }

    [Test]
    public static void CreateFileTest()
    {
        CreateFile(defaultFile, 567);
        That(File.ReadAllLines(defaultFile).Length, Is.EqualTo(567));
        Catch<UnableCreateFile>(() => { CreateFile("y/y.txt", 100); });
    }

    [Test]
    public static void gcdTest()
    {
        That(gcd(120, 180), Is.EqualTo(60));
        Catch<MissingInputFile>(() => { CountGcd("file.txt"); });
    }
}
