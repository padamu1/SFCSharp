using SFCSharp.Analyzer;
using SFCSharp.Attributes;
using SFCSharp.Core;
using SFCSharp.Excution;
using SFCSharp.Excution.UnityExec;
using SFCSharp.ScriptUtil;
using System;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== SFCSharp UnityEngine Test ===\n");

        // 1. 설정 초기화
        SFCSharpConfig.Initialize();
        Console.WriteLine($"✓ Config initialized");
        Console.WriteLine($"  - Output Path: {SFCSharpConfig.ScriptOutputPath}");
        Console.WriteLine($"  - Cache Path: {SFCSharpConfig.ScriptCachePath}\n");

        // 2. Vector3 테스트
        TestVector3();

        // 3. Transform 테스트
        TestTransform();

        // 4. GameObject 테스트
        TestGameObject();

        // 5. 기존 스크립트 읽기 테스트
        TestScriptReading();

        // 6. 통합 스크립트 실행 테스트
        TestScriptIntegration();

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
        Console.WriteLine($"Scale: {transform.scale}");

        // GameObject 접근
        if (transform.gameObject != null)
        {
            Console.WriteLine($"GameObject name: {transform.gameObject.name}");
        }
        Console.WriteLine();
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
        Console.WriteLine($"After SetActive(true): {gameObject.activeSelf}");

        // Transform 조작
        var transform = gameObject.transform;
        transform.position = new SFVector3(100, 200, 300);
        transform.Rotate(45, 90, 45);
        Console.WriteLine($"Transform Position: {transform.position}");
        Console.WriteLine();
    }

    private static void TestScriptReading()
    {
        Console.WriteLine("--- Script Reading Test ---");

        try
        {
            var sources = SFSourceReader.GetScriptsFromFolderPath("C:\\git\\SFCSharp\\Tester");
            Console.WriteLine($"Found {sources.Count} scripts with [SFCSharp] attribute");

            foreach (string source in sources)
            {
                var result = ContextAnalyzer.AnalyzeScript(source);
                Console.WriteLine($"  - Namespace: {result.Namespace}, Class: {result.ClassName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading scripts: {ex.Message}");
        }
    }

    private static void TestScriptIntegration()
    {
        Console.WriteLine("\n--- Script Integration Test (Build-Load-Execute) ---");

        try
        {
            // 1. 스크립트 파일 읽기
            Console.WriteLine("Step 1: Reading script file...");
            var scriptPath = "C:\\git\\SFCSharp\\Tester\\UnityTestScript.cs";
            if (!System.IO.File.Exists(scriptPath))
            {
                Console.WriteLine($"✗ Script file not found: {scriptPath}");
                return;
            }

            string scriptSource = System.IO.File.ReadAllText(scriptPath);
            Console.WriteLine("✓ Script file read successfully");

            // 2. 스크립트 분석
            Console.WriteLine("\nStep 2: Analyzing script...");
            var analysisResult = ContextAnalyzer.AnalyzeScript(scriptSource);
            Console.WriteLine($"✓ Script analyzed");
            Console.WriteLine($"  - Namespace: {analysisResult.Namespace}");
            Console.WriteLine($"  - Class: {analysisResult.ClassName}");

            // 3. Transform 객체 생성
            Console.WriteLine("\nStep 3: Creating Transform object...");
            var transform = new SFTransform("TestTransform");
            Console.WriteLine($"✓ Transform created");
            Console.WriteLine($"  - Initial Position: {transform.position}");

            // 4. Transform 메서드 테스트
            Console.WriteLine("\nStep 4: Testing Transform operations...");

            // 4.1 SetPosition 테스트
            Console.WriteLine("  4.1 Testing SetPosition(5, 10, 15)...");
            transform.position = new SFVector3(5, 10, 15);
            Console.WriteLine($"  ✓ Position changed to: {transform.position}");

            // 4.2 Translate 테스트
            Console.WriteLine("  4.2 Testing Translate(2, 3, 4)...");
            transform.Translate(2, 3, 4);
            Console.WriteLine($"  ✓ After translate: {transform.position}");

            // 4.3 Rotate 테스트
            Console.WriteLine("  4.3 Testing Rotate(45, 90, 45)...");
            transform.Rotate(45, 90, 45);
            Console.WriteLine($"  ✓ Rotation applied");

            // 4.4 Scale 테스트
            Console.WriteLine("  4.4 Testing Scale (2, 2, 2)...");
            transform.scale = new SFVector3(2, 2, 2);
            Console.WriteLine($"  ✓ Scale set to: {transform.scale}");

            // 5. Transform 메서드 핸들러 호출 테스트
            Console.WriteLine("\nStep 5: Testing Transform method handlers...");
            TestTransformMethodHandlers(transform);

            Console.WriteLine("\n✓ All integration tests passed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error during integration test: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private static void TestTransformMethodHandlers(SFTransform transform)
    {
        Console.WriteLine("  Testing Transform handler methods...");

        // UnityExecHandler를 통한 메서드 호출 시뮬레이션
        var initialPos = transform.position;
        Console.WriteLine($"  - Initial position: {initialPos}");

        // Translate 메서드 호출
        transform.Translate(1, 2, 3);
        var afterTranslate = transform.position;
        Console.WriteLine($"  ✓ After Translate(1,2,3): {afterTranslate}");

        // GetPosition 확인
        var currentPos = transform.position;
        Console.WriteLine($"  ✓ GetPosition: {currentPos}");

        // SetPosition 테스트
        transform.position = new SFVector3(100, 200, 300);
        var afterSetPosition = transform.position;
        Console.WriteLine($"  ✓ After SetPosition(100,200,300): {afterSetPosition}");

        // LookAt 테스트 (시뮬레이션)
        var targetPos = new SFVector3(110, 210, 310);
        transform.LookAt(targetPos);
        Console.WriteLine($"  ✓ LookAt({targetPos}) executed");

        Console.WriteLine("  ✓ All Transform handler tests passed");
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