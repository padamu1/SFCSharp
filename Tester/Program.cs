using SFCSharp.Excution;

public class Program
{
    public static void Main()
    {
        string testScript = @"System.Console.WriteLine(""Hello"");";

        SFExecManager.Exec("System.Console.WriteLine", null, "Hello");

        Console.ReadLine();
    }
}