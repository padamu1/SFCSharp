using SFCSharp.Analyzer;
using SFCSharp.Attributes;
using SFCSharp.Excution;
using SFCSharp.ScriptUtil;

public class Program
{
    public static void Main()
    {
        //string testScript2 = @"";
        //string testScript = @"System.Console.Error.WriteLine(""Hello"");";
        //
        //ContextMethodAnalyzer.Parse(testScript, out string method, out List<object> param);
        //SFExecManager.ExecWrapper(method, null, param.ToArray());


        var sources = SFSourceReader.GetScriptsFromFolderPath("C:\\git\\SFCSharp\\Tester");
        
        foreach (string source in sources)
        {
            SFCSharpParser.ExtractClassesWithSFCSharp(source);
            //Console.WriteLine(source);
        }

        Console.ReadLine();
    }
}


namespace Test.Sample
{
    [SFCSharp]
    public class TestClass
    {
        public void Run()
        {
        }

        public void Test()
        {
            Console.WriteLine("Hello");
        }
    }
}