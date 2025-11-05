using SFCSharp.Analyzer;
using SFCSharp.Attributes;
using SFCSharp.Excution;
using SFCSharp.Excution.UnityExec;
using SFCSharp.Runtime;
using System;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== SFCSharp Refactored Test ===\n");

        // 1. Vector3 테스트
        TestVector3();

        // 2. Transform 테스트
        TestTransform();

        // 3. GameObject 테스트
        TestGameObject();

        // 4. CommandParser 테스트
        TestCommandParser();

        // 5. ScriptExecutor 테스트
        TestScriptExecutor();

        Console.WriteLine("\n=== Test Complete ===");
        Console.ReadLine();
    }

    private static void TestVector3()
    {
        Console.WriteLine("--- Vector3 Test ---");

        var vec1 = new SFVector3(1, 2, 3);
        var vec2 = new SFVector3(4, 5, 6);

        Console.WriteLine($"Vector1: {vec1}");
        Console.WriteLine($"Vector2: {vec2}");

        float distance = SFVector3.Distance(vec1, vec2);
        Console.WriteLine($"Distance: {distance:F2}");

        float dot = SFVector3.Dot(vec1, vec2);
        Console.WriteLine($"Dot Product: {dot}");

        var cross = SFVector3.Cross(vec1, vec2);
        Console.WriteLine($"Cross Product: {cross}");

        Console.WriteLine($"Magnitude of Vector1: {vec1.magnitude:F2}");
        Console.WriteLine($"Normalized Vector1: {vec1.normalized}\n");
    }

    private static void TestTransform()
    {
        Console.WriteLine("--- Transform Test ---");

        var transform = new SFTransform("TestObject");
        Console.WriteLine($"Created Transform: {transform.name}");
        Console.WriteLine($"Position: {transform.position}");

        // Position 설정
        transform.position = new SFVector3(10, 20, 30);
        Console.WriteLine($"After position change: {transform.position}");

        // Translate
        transform.Translate(5, 5, 5);
        Console.WriteLine($"After translate(5,5,5): {transform.position}");

        // Scale
        transform.scale = new SFVector3(2, 2, 2);
        Console.WriteLine($"Scale: {transform.scale}\n");
    }

    private static void TestGameObject()
    {
        Console.WriteLine("--- GameObject Test ---");

        var gameObject = new SFGameObject("TestGameObject");
        Console.WriteLine($"Created: {gameObject.name}");
        Console.WriteLine($"Active: {gameObject.activeSelf}");

        // 이름 변경
        gameObject.name = "RenamedObject";
        Console.WriteLine($"After rename: {gameObject.name}");

        // 활성화 상태 변경
        gameObject.SetActive(false);
        Console.WriteLine($"After SetActive(false): {gameObject.activeSelf}");

        gameObject.SetActive(true);
        Console.WriteLine($"After SetActive(true): {gameObject.activeSelf}\n");
    }

    private static void TestCommandParser()
    {
        Console.WriteLine("--- CommandParser Test ---");

        try
        {
            // 테스트 1: 간단한 메서드 호출
            var cmd1 = CommandParser.Parse("GameObject.Create('Player')");
            Console.WriteLine($"✓ Parsed: {cmd1}");
            Console.WriteLine($"  Method: {cmd1.MethodPath}");
            Console.WriteLine($"  Args: {string.Join(", ", cmd1.Arguments)}\n");

            // 테스트 2: Vector3 인자
            var cmd2 = CommandParser.Parse("Transform.Translate(Vector3(1.0, 2.0, 3.0))");
            Console.WriteLine($"✓ Parsed: {cmd2}");
            Console.WriteLine($"  Args: {cmd2.Arguments[0]}\n");

            // 테스트 3: 여러 인자
            var cmd3 = CommandParser.Parse("GameObject.Create('Enemy', 'Boss', true)");
            Console.WriteLine($"✓ Parsed: {cmd3}");
            Console.WriteLine($"  Args: {string.Join(", ", cmd3.Arguments)}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error: {ex.Message}\n");
        }
    }

    private static void TestScriptExecutor()
    {
        Console.WriteLine("--- ScriptExecutor Test ---");

        try
        {
            // 컨텍스트 생성
            var context = new SFCSharp.Context.SFContext("Test.Namespace", "TestScript");
            var executor = new ScriptExecutor(context);

            Console.WriteLine($"✓ ScriptExecutor created for {context}");

            // 변수 저장
            executor.SetVariable("playerName", "Hero");
            Console.WriteLine($"✓ Variable 'playerName' set to 'Hero'");

            // 변수 조회
            var playerName = executor.GetVariable("playerName");
            Console.WriteLine($"✓ Variable 'playerName' retrieved: {playerName}\n");

            // 명령어 실행 테스트 (GameObject.Create)
            Console.WriteLine("Executing commands:");
            var result1 = executor.Execute("UnityEngine.GameObject.Create('Player')");
            Console.WriteLine($"  {result1}");

            if (result1.Success && result1.Result is SFGameObject go)
            {
                Console.WriteLine($"  → GameObject created: {go.name}");
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error: {ex.Message}\n");
        }
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
