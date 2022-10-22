using NUnit.Framework;
using System.IO.Enumeration;
using static NUnit.Framework.Assert;
using static Task1.Task1;

namespace Task1;

public class Tests
{

    [Test]
    public void ApplyOperationTest()
    {
        That(ApplyOperation('*', 5, 2), Is.EqualTo(10));
        That(ApplyOperation('/', 10, 5), Is.EqualTo(2));
        Catch<DivideByZeroException>(() => { ApplyOperation('/', 1, 0); });
    }

    [Test]
    public void FormatLhsTest()
    {
        That(FormatLhs("**/", new string[] { "1", "2", "3", "4" }), Is.EqualTo("1*2*3/4"));
    }

    [Test]
    public void ProcessStringTest()
    {
        That(ProcessString("**", "1,2,3"), Is.EqualTo("1*2*3=6"));
        That(ProcessString("**/", "1,2,3,4"), Is.EqualTo("1*2*3/4=1"));
        Catch<NotEnoughNumbers>(() => { ProcessString("***", "12"); });
    }

    [Test]
    public void ReadFileTest()
    {
        That(ReadFile("../../../schema_test.txt"), Is.EqualTo(new string[] { "123", "45", "6" }));
        Catch<MissingInputFile>(() => { ReadFile("a.txt"); });
    }

    [Test]
    public void WriteFileTest()
    {
        var fileName = "../../../schema_test.txt";
        var data = new List<string> { "1,2,3", "4,5", "6" };

        WriteFile(fileName, data);
        That(ReadFile(fileName), Is.EqualTo(data));

        Catch<UnableCreateFile>(() => { WriteFile("E/a.txt", new List<string> { "a" }); });
    }

    [Test]
    public void ProcessFilesTest()
    {
        var schemaFile = "../../../data/schema.txt";
        var dataFile1 = "../../../data/data.txt";
        var dataFile2 = "../../../data/data2.txt";
        var dataFile3 = "../../../data/data3.txt";
        var dataFile4 = "../../../data/data4.txt";
        var outputFile = "../../../data/output.txt";

        ProcessFiles(schemaFile, dataFile1, outputFile);
        That(ReadFile(outputFile), Is.EqualTo(new string[] { "2*4/3=2", "5*2*1*2=20", "1/0 *** DIVBYZERO", "5/5*2=2" }));
        Catch<IncorrectFormatOfLines>(() => { ProcessFiles(schemaFile, dataFile2, outputFile); });
        Catch<NotEnoughNumbers>(() => { ProcessFiles(schemaFile, dataFile3, outputFile); });
        Catch<DifferentNumberOfLines>(() => { ProcessFiles(schemaFile, dataFile4, outputFile); });
    }

    [Test]
    public void ParseArgsTest()
    {
        That(ParseArgs(new string[] { "1", "2", "3", "4" }), Is.EqualTo(("1", "2", "3")));
        Catch<NotEnoughArguments>(() => { ParseArgs(new string[] { "a.txt" }); });
    }
}