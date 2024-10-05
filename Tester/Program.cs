using SFCSharp.Analyzer;
using SFCSharp.Excution;

public class Program
{
    public static void Main()
    {
        string testScript = @"System.Console.Error.WriteLine(""Hello"");";

        ContextMethodAnalyzer.Parse(testScript, out string method, out List<object> param);
        SFExecManager.ExecWrapper(method, null, param.ToArray());

        Console.ReadLine();
    }
}