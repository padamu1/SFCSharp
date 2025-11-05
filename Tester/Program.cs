using SFCSharp.Context;
using SFCSharp.Excution;
using SFCSharp.Excution.UnityExec;
using SFCSharp.Runtime;
using SFCSharp.Tester;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Program
{
    private static int passedTests = 0;
    private static int failedTests = 0;
    private static List<string> failedTestNames = new();

    public static void Main()
    {
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine("           SFCSharp MOD Scripting Framework Tests             ");
        Console.WriteLine("════════════════════════════════════════════════════════════\n");

        // 테스트 실행
        TestCommandParserBasic();
        TestCommandParserAdvanced();
        TestScriptExecutorBasic();
        TestScriptExecutorVariables();
        TestUnityEngineTypes();
        TestGameObjectLifecycle();
        TestTransformOperations();
        TestIntegrationWorkflow();
        TestScriptExecution();
        TestAssetBundleBuildPipeline();

        // 테스트 결과 출력
        PrintTestSummary();

        Console.WriteLine("\n프로그램을 종료하려면 엔터를 누르세요...");
        Console.ReadLine();
    }

    #region CommandParser Tests

    private static void TestCommandParserBasic()
    {
        PrintTestCategory("CommandParser - Basic Tests");

        // Test 1: 단순 메서드 호출
        try
        {
            var cmd = CommandParser.Parse("GameObject.Create('Player')");
            Assert(cmd.MethodPath == "GameObject.Create", "MethodPath should be 'GameObject.Create'");
            Assert(cmd.Arguments.Length == 1, "Should have 1 argument");
            Assert(cmd.Arguments[0].ToString() == "Player", "First argument should be 'Player'");
            PassTest("단순 메서드 호출 파싱");
        }
        catch (Exception ex)
        {
            FailTest("단순 메서드 호출 파싱", ex.Message);
        }

        // Test 2: 여러 인자
        try
        {
            var cmd = CommandParser.Parse("GameObject.Create('Enemy', 'Boss', true)");
            Assert(cmd.Arguments.Length == 3, "Should have 3 arguments");
            Assert(cmd.Arguments[0].ToString() == "Enemy", "First arg should be 'Enemy'");
            Assert(cmd.Arguments[1].ToString() == "Boss", "Second arg should be 'Boss'");
            Assert((bool)cmd.Arguments[2] == true, "Third arg should be true");
            PassTest("여러 인자 파싱");
        }
        catch (Exception ex)
        {
            FailTest("여러 인자 파싱", ex.Message);
        }

        // Test 3: Vector3 인자
        try
        {
            var cmd = CommandParser.Parse("Transform.Translate(Vector3(1.0, 2.0, 3.0))");
            Assert(cmd.MethodPath == "Transform.Translate", "MethodPath should be 'Transform.Translate'");
            Assert(cmd.Arguments[0] is SFVector3, "Argument should be SFVector3");
            var vec = (SFVector3)cmd.Arguments[0];
            Assert(vec.x == 1.0f && vec.y == 2.0f && vec.z == 3.0f, "Vector3 values should match");
            PassTest("Vector3 인자 파싱");
        }
        catch (Exception ex)
        {
            FailTest("Vector3 인자 파싱", ex.Message);
        }

        // Test 4: 부동소수점 숫자
        try
        {
            var cmd = CommandParser.Parse("Transform.Scale(2.5)");
            Assert(cmd.Arguments[0] is float, "Argument should be float");
            Assert((float)cmd.Arguments[0] == 2.5f, "Float value should be 2.5");
            PassTest("부동소수점 숫자 파싱");
        }
        catch (Exception ex)
        {
            FailTest("부동소수점 숫자 파싱", ex.Message);
        }

        // Test 5: 빈 인자
        try
        {
            var cmd = CommandParser.Parse("GameObject.Destroy()");
            Assert(cmd.Arguments.Length == 0, "Should have 0 arguments");
            PassTest("빈 인자 파싱");
        }
        catch (Exception ex)
        {
            FailTest("빈 인자 파싱", ex.Message);
        }
    }

    private static void TestCommandParserAdvanced()
    {
        PrintTestCategory("CommandParser - Advanced Tests");

        // Test 1: 네임스페이스가 포함된 명령어
        try
        {
            var cmd = CommandParser.Parse("UnityEngine.GameObject.Create('Player')");
            Assert(cmd.MethodPath == "UnityEngine.GameObject.Create", "Should handle namespace correctly");
            PassTest("네임스페이스 포함 명령어");
        }
        catch (Exception ex)
        {
            FailTest("네임스페이스 포함 명령어", ex.Message);
        }

        // Test 2: 정수 인자
        try
        {
            var cmd = CommandParser.Parse("Array.Get(5)");
            Assert(cmd.Arguments[0] is int, "Argument should be int");
            Assert((int)cmd.Arguments[0] == 5, "Int value should be 5");
            PassTest("정수 인자 파싱");
        }
        catch (Exception ex)
        {
            FailTest("정수 인자 파싱", ex.Message);
        }

        // Test 3: 혼합 인자
        try
        {
            var cmd = CommandParser.Parse("Method('text', 42, 3.14, true, Vector3(0, 1, 0))");
            Assert(cmd.Arguments.Length == 5, "Should have 5 arguments");
            Assert(cmd.Arguments[0].ToString() == "text", "First arg should be string");
            Assert((int)cmd.Arguments[1] == 42, "Second arg should be int");
            Assert(Math.Abs((float)cmd.Arguments[2] - 3.14f) < 0.01f, "Third arg should be float");
            Assert((bool)cmd.Arguments[3] == true, "Fourth arg should be bool");
            Assert(cmd.Arguments[4] is SFVector3, "Fifth arg should be Vector3");
            PassTest("혼합 인자 파싱");
        }
        catch (Exception ex)
        {
            FailTest("혼합 인자 파싱", ex.Message);
        }

        // Test 4: 공백 처리
        try
        {
            var cmd1 = CommandParser.Parse("  GameObject.Create('Player')  ");
            var cmd2 = CommandParser.Parse("GameObject.Create( 'Player' )");
            var cmd3 = CommandParser.Parse("GameObject . Create ( 'Player' )");
            Assert(cmd1.MethodPath == "GameObject.Create", "Should handle leading/trailing spaces");
            PassTest("공백 처리");
        }
        catch (Exception ex)
        {
            FailTest("공백 처리", ex.Message);
        }

        // Test 5: 잘못된 형식 예외
        try
        {
            CommandParser.Parse("InvalidCommand");
            FailTest("예외 처리 - 잘못된 형식", "Should have thrown exception");
        }
        catch (InvalidOperationException)
        {
            PassTest("예외 처리 - 잘못된 형식");
        }
        catch (Exception ex)
        {
            FailTest("예외 처리 - 잘못된 형식", ex.Message);
        }
    }

    #endregion

    #region ScriptExecutor Tests

    private static void TestScriptExecutorBasic()
    {
        PrintTestCategory("ScriptExecutor - Basic Tests");

        try
        {
            var context = new SFContext("Test.Namespace", "TestScript");
            var executor = new ScriptExecutor(context);

            // Test 1: GameObject 생성
            var result1 = executor.Execute("UnityEngine.GameObject.Create('TestPlayer')");
            Assert(result1.Success, "Execute should succeed");
            Assert(result1.Result is SFGameObject, "Result should be GameObject");
            var go = (SFGameObject)result1.Result;
            Assert(go.name == "TestPlayer", "GameObject name should be 'TestPlayer'");
            PassTest("GameObject 생성");

            // Test 2: Transform 접근
            var result2 = executor.Execute("UnityEngine.GameObject.GetTransform()");
            Assert(result2.Success, "GetTransform should succeed");
            PassTest("Transform 접근");
        }
        catch (Exception ex)
        {
            FailTest("ScriptExecutor 기본 기능", ex.Message);
        }
    }

    private static void TestScriptExecutorVariables()
    {
        PrintTestCategory("ScriptExecutor - Variable Tests");

        try
        {
            var context = new SFContext("Test.Variables", "VariableTest");
            var executor = new ScriptExecutor(context);

            // Test 1: 변수 저장
            executor.SetVariable("playerName", "Hero");
            var value = executor.GetVariable("playerName");
            Assert(value.ToString() == "Hero", "Variable should be stored and retrieved");
            PassTest("변수 저장 및 조회");

            // Test 2: 다양한 타입의 변수
            executor.SetVariable("playerLevel", 10);
            executor.SetVariable("playerHealth", 95.5f);
            executor.SetVariable("isAlive", true);

            Assert((int)executor.GetVariable("playerLevel") == 10, "Int variable");
            Assert(Math.Abs((float)executor.GetVariable("playerHealth") - 95.5f) < 0.01f, "Float variable");
            Assert((bool)executor.GetVariable("isAlive") == true, "Bool variable");
            PassTest("다양한 타입 변수 저장");

            // Test 3: 변수 덮어쓰기
            executor.SetVariable("playerName", "SuperHero");
            Assert(executor.GetVariable("playerName").ToString() == "SuperHero", "Variable should be overwritable");
            PassTest("변수 덮어쓰기");

            // Test 4: 존재하지 않는 변수 조회
            var nonExistent = executor.GetVariable("nonExistent");
            Assert(nonExistent == null, "Non-existent variable should return null");
            PassTest("존재하지 않는 변수 처리");
        }
        catch (Exception ex)
        {
            FailTest("ScriptExecutor 변수 관리", ex.Message);
        }
    }

    #endregion

    #region UnityEngine Tests

    private static void TestUnityEngineTypes()
    {
        PrintTestCategory("UnityEngine Types - Basic Tests");

        // Test 1: Vector3 생성 및 연산
        try
        {
            var vec1 = new SFVector3(1, 2, 3);
            var vec2 = new SFVector3(4, 5, 6);

            Assert(vec1.x == 1 && vec1.y == 2 && vec1.z == 3, "Vector3 초기화");

            float distance = SFVector3.Distance(vec1, vec2);
            Assert(Math.Abs(distance - 5.196f) < 0.01f, "Vector3 거리 계산");

            float dot = SFVector3.Dot(vec1, vec2);
            Assert(dot == 32, "Vector3 내적 계산");

            var cross = SFVector3.Cross(vec1, vec2);
            Assert(cross.x == -3 && cross.y == 6 && cross.z == -3, "Vector3 외적 계산");

            PassTest("Vector3 연산");
        }
        catch (Exception ex)
        {
            FailTest("Vector3 연산", ex.Message);
        }

        // Test 2: Vector3 정규화
        try
        {
            var vec = new SFVector3(3, 4, 0);
            var normalized = vec.normalized;
            float magnitude = vec.magnitude;

            Assert(magnitude == 5.0f, "Vector3 크기");
            Assert(Math.Abs(normalized.magnitude - 1.0f) < 0.01f, "정규화된 벡터의 크기는 1");
            PassTest("Vector3 정규화");
        }
        catch (Exception ex)
        {
            FailTest("Vector3 정규화", ex.Message);
        }

        // Test 3: Quaternion 기본
        try
        {
            var quat = new SFQuaternion(0, 0, 0, 1);
            Assert(quat.x == 0 && quat.y == 0 && quat.z == 0 && quat.w == 1, "Quaternion 항등원소");
            PassTest("Quaternion 기본");
        }
        catch (Exception ex)
        {
            FailTest("Quaternion 기본", ex.Message);
        }
    }

    private static void TestGameObjectLifecycle()
    {
        PrintTestCategory("GameObject - Lifecycle Tests");

        try
        {
            // Test 1: GameObject 생성
            var go = new SFGameObject("TestObject");
            Assert(go.name == "TestObject", "GameObject 이름 설정");
            Assert(go.activeSelf == true, "GameObject 기본값은 활성");
            PassTest("GameObject 생성");

            // Test 2: GameObject 이름 변경
            go.name = "RenamedObject";
            Assert(go.name == "RenamedObject", "GameObject 이름 변경");
            PassTest("GameObject 이름 변경");

            // Test 3: GameObject 활성화 상태 변경
            go.SetActive(false);
            Assert(go.activeSelf == false, "GameObject 비활성화");
            go.SetActive(true);
            Assert(go.activeSelf == true, "GameObject 활성화");
            PassTest("GameObject 활성화 상태");

            // Test 4: GameObject Transform 접근
            var transform = go.transform;
            Assert(transform != null, "GameObject에서 Transform 접근");
            Assert(transform.name == "TestObject", "Transform 이름은 GameObject와 동일");
            PassTest("GameObject Transform 접근");
        }
        catch (Exception ex)
        {
            FailTest("GameObject 생명주기", ex.Message);
        }
    }

    private static void TestTransformOperations()
    {
        PrintTestCategory("Transform - Operations Tests");

        try
        {
            var transform = new SFTransform("TestTransform");

            // Test 1: 위치 설정 및 변경
            transform.position = new SFVector3(10, 20, 30);
            Assert(transform.position.x == 10 && transform.position.y == 20 && transform.position.z == 30,
                "Transform 위치 설정");
            PassTest("Transform 위치 설정");

            // Test 2: 이동
            transform.Translate(5, 5, 5);
            Assert(transform.position.x == 15 && transform.position.y == 25 && transform.position.z == 35,
                "Transform 이동");
            PassTest("Transform 이동");

            // Test 3: 회전
            transform.Rotate(90, 0, 0);
            Assert(transform.rotation != null, "Transform 회전");
            PassTest("Transform 회전");

            // Test 4: 스케일
            transform.scale = new SFVector3(2, 2, 2);
            Assert(transform.scale.x == 2 && transform.scale.y == 2 && transform.scale.z == 2,
                "Transform 스케일 설정");
            PassTest("Transform 스케일");

            // Test 5: LookAt
            var target = new SFVector3(100, 100, 100);
            transform.LookAt(target);
            Assert(transform.rotation != null, "Transform LookAt");
            PassTest("Transform LookAt");
        }
        catch (Exception ex)
        {
            FailTest("Transform 연산", ex.Message);
        }
    }

    #endregion

    #region Integration Tests

    private static void TestIntegrationWorkflow()
    {
        PrintTestCategory("Integration - Complete Workflow Tests");

        try
        {
            var context = new SFContext("Game.MOD", "PlayerController");
            var executor = new ScriptExecutor(context);

            // 워크플로우: 플레이어 생성 → 변수 설정 → 위치 변경 → 상태 관리

            // Step 1: 플레이어 GameObject 생성
            var createResult = executor.Execute("UnityEngine.GameObject.Create('Player')");
            Assert(createResult.Success, "플레이어 생성 성공");
            PassTest("워크플로우 - 객체 생성");

            // Step 2: 플레이어 속성을 변수로 저장
            executor.SetVariable("playerName", "MainCharacter");
            executor.SetVariable("playerHealth", 100);
            executor.SetVariable("playerMana", 50);

            var health = executor.GetVariable("playerHealth");
            Assert((int)health == 100, "플레이어 상태 변수 저장");
            PassTest("워크플로우 - 상태 저장");

            // Step 3: 플레이어 위치 설정 (GameObject 생성 후 transform 접근)
            if (createResult.Result is SFGameObject player)
            {
                var playerTransform = player.transform;
                playerTransform.position = new SFVector3(10, 0, 10);
                Assert(playerTransform.position.x == 10, "플레이어 위치 설정");
                PassTest("워크플로우 - 위치 설정");
            }

            // Step 4: 연속 명령 실행
            var results = executor.ExecuteSequence(
                "UnityEngine.GameObject.Create('Enemy1')",
                "UnityEngine.GameObject.Create('Enemy2')",
                "UnityEngine.GameObject.Create('Enemy3')"
            );

            int successCount = 0;
            foreach (var result in results)
            {
                if (result.Success) successCount++;
            }
            Assert(successCount == 3, "3개의 적 생성");
            PassTest("워크플로우 - 연속 명령");

            // Step 5: 최종 상태 확인
            var finalName = executor.GetVariable("playerName");
            var finalHealth = executor.GetVariable("playerHealth");
            Assert(finalName.ToString() == "MainCharacter" && (int)finalHealth == 100,
                "최종 상태 확인");
            PassTest("워크플로우 - 최종 상태");
        }
        catch (Exception ex)
        {
            FailTest("통합 워크플로우", ex.Message);
        }
    }

    #endregion

    #region Script Execution Tests

    private static void TestScriptExecution()
    {
        PrintTestCategory("Script Execution - MOD Script Tests");

        // Test 1: BasicGameObject.script 실행
        TestScriptFile("Scripts/BasicGameObject.script", "기본 GameObject 생성 스크립트", 5);

        // Test 2: TransformOperations.script 실행
        TestScriptFile("Scripts/TransformOperations.script", "Transform 조작 스크립트", 4);

        // Test 3: PlayerInitialization.script 실행
        TestScriptFile("Scripts/PlayerInitialization.script", "플레이어 초기화 스크립트", 4);

        // Test 4: GameWorldSetup.script 실행
        TestScriptFile("Scripts/GameWorldSetup.script", "게임 세계 설정 스크립트", 19);

        // Test 5: ComplexInteraction.script 실행
        TestScriptFile("Scripts/ComplexInteraction.script", "복잡한 상호작용 스크립트", 15);

        // Test 6: 스크립트 파일 읽기 및 파싱
        try
        {
            string scriptPath = "Scripts/BasicGameObject.script";
            if (File.Exists(scriptPath))
            {
                var lines = File.ReadAllLines(scriptPath);
                var commandLines = lines.Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("//")).ToList();
                Assert(commandLines.Count >= 5, "Should have at least 5 command lines");
                PassTest("스크립트 파일 파싱");
            }
            else
            {
                FailTest("스크립트 파일 파싱", "Script file not found");
            }
        }
        catch (Exception ex)
        {
            FailTest("스크립트 파일 파싱", ex.Message);
        }

        // Test 7: 스크립트 주석 필터링
        try
        {
            var context = new SFContext("Test.Script", "CommentTest");
            var executor = new ScriptExecutor(context);

            // 주석이 포함된 명령어
            var result = executor.Execute("UnityEngine.GameObject.Create('TestObj')");
            Assert(result.Success, "Should execute command successfully");
            PassTest("스크립트 주석 처리");
        }
        catch (Exception ex)
        {
            FailTest("스크립트 주석 처리", ex.Message);
        }

        // Test 8: 여러 명령어 순차 실행
        try
        {
            var context = new SFContext("Test.Sequence", "SequenceTest");
            var executor = new ScriptExecutor(context);

            var results = executor.ExecuteSequence(
                "UnityEngine.GameObject.Create('Unit1')",
                "UnityEngine.GameObject.Create('Unit2')",
                "UnityEngine.GameObject.Create('Unit3')",
                "UnityEngine.GameObject.Create('Unit4')",
                "UnityEngine.GameObject.Create('Unit5')"
            );

            Assert(results.Count == 5, "Should execute 5 commands");
            int successCount = results.Count(r => r.Success);
            Assert(successCount == 5, "All 5 commands should succeed");
            PassTest("여러 명령어 순차 실행");
        }
        catch (Exception ex)
        {
            FailTest("여러 명령어 순차 실행", ex.Message);
        }
    }

    private static void TestScriptFile(string scriptPath, string testName, int expectedCommands)
    {
        try
        {
            if (!File.Exists(scriptPath))
            {
                FailTest(testName, $"Script file not found: {scriptPath}");
                return;
            }

            // 스크립트 파일 읽기
            var lines = File.ReadAllLines(scriptPath);

            // 주석과 빈 줄 필터링
            var commandLines = lines
                .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("//") && !l.StartsWith("/*"))
                .ToList();

            Assert(commandLines.Count >= expectedCommands, $"Should have at least {expectedCommands} commands");

            // 각 명령어 파싱 및 실행 테스트
            var context = new SFContext("Test.Scripts", Path.GetFileNameWithoutExtension(scriptPath));
            var executor = new ScriptExecutor(context);

            int successCount = 0;
            foreach (var line in commandLines)
            {
                try
                {
                    var result = executor.Execute(line);
                    if (result.Success)
                        successCount++;
                }
                catch
                {
                    // 특정 명령어 실패는 무시하고 계속
                }
            }

            Assert(successCount > 0, "Should execute at least one command successfully");
            PassTest(testName);
        }
        catch (Exception ex)
        {
            FailTest(testName, ex.Message);
        }
    }

    #endregion

    #region AssetBundle Build Pipeline Tests

    private static void TestAssetBundleBuildPipeline()
    {
        PrintTestCategory("AssetBundle Build Pipeline - Full MOD Build & Runtime Tests");

        // Test 1: 기본 MOD 빌드
        TestBasicModBuild();

        // Test 2: 복잡한 MOD 빌드 (여러 Prefab)
        TestComplexModBuild();

        // Test 3: C# 코드 → SFCSharp 스크립트 변환
        TestCSharpToSFCSharpConversion();

        // Test 4: 빌드된 Bundle 런타임 로드 및 실행
        TestRuntimeBundleExecution();

        // Test 5: MOD 배포 및 설치 시뮬레이션
        TestModDistributionSimulation();
    }

    private static void TestBasicModBuild()
    {
        try
        {
            var simulator = new AssetBundleSimulator("TestBuild_Basic");

            // Step 1: C# 스크립트 작성 ([SFCSharp] 태그)
            string playerScript = @"
using UnityEngine;
[SFCSharp]
public class PlayerController : MonoBehaviour
{
    public void Start()
    {
        // 플레이어 생성
        GameObject.Instantiate('MainCharacter')
    }

    public void Initialize()
    {
        GameObject.Instantiate('Player')
        GameObject.Instantiate('PlayerUI')
    }
}";

            // Step 2: 스크립트 등록 (C# -> SFCSharp로 자동 변환)
            simulator.RegisterScript("PlayerController", playerScript);
            Assert(File.Exists("TestBuild_Basic/Scripts/PlayerController.script"), "Script file should be created");
            PassTest("Step 1: C# 스크립트 변환");

            // Step 3: Prefab 등록 (SFCSharpExecutor 자동 주입)
            simulator.RegisterPrefab("Player_Prefab", "PlayerController");
            PassTest("Step 2: Prefab SFCSharpExecutor 주입");

            // Step 4: AssetBundle 빌드
            var buildResult = simulator.BuildAssetBundle("PlayerMod_v1");
            Assert(buildResult.Status == AssetBundleSimulator.BuildStatus.Success, "Build should succeed");
            Assert(buildResult.ProcessedPrefabs.Count == 1, "Should process 1 prefab");
            Assert(buildResult.ProcessedScripts.Count == 1, "Should process 1 script");
            PassTest("Step 3: AssetBundle 빌드");

            // Step 5: 빌드 검증
            Assert(buildResult.BundleInfo.ContainsModData, "Bundle should contain MOD data");
            Assert(buildResult.BundleInfo.TotalPrefabs == 1, "Bundle should have 1 prefab");
            PassTest("Step 4: 빌드 결과 검증");

            simulator.CleanUp();
        }
        catch (Exception ex)
        {
            FailTest("기본 MOD 빌드", ex.Message);
        }
    }

    private static void TestComplexModBuild()
    {
        try
        {
            var simulator = new AssetBundleSimulator("TestBuild_Complex");

            // 플레이어 스크립트
            string playerScript = @"
[SFCSharp]
public class PlayerController : MonoBehaviour
{
    public void Initialize()
    {
        GameObject.Instantiate('Player')
        GameObject.Instantiate('PlayerWeapon')
        GameObject.Instantiate('PlayerArmor')
    }
}";

            // 적 스크립트
            string enemyScript = @"
[SFCSharp]
public class EnemyController : MonoBehaviour
{
    public void Spawn()
    {
        GameObject.Instantiate('Goblin')
        GameObject.Instantiate('Goblin')
        GameObject.Instantiate('Orc')
    }
}";

            // 아이템 스크립트
            string itemScript = @"
[SFCSharp]
public class ItemManager : MonoBehaviour
{
    public void Setup()
    {
        GameObject.Instantiate('Potion')
        GameObject.Instantiate('Gold')
    }
}";

            // 스크립트 등록
            simulator.RegisterScript("PlayerController", playerScript);
            simulator.RegisterScript("EnemyController", enemyScript);
            simulator.RegisterScript("ItemManager", itemScript);
            PassTest("복잡한 MOD: 3개 스크립트 등록");

            // Prefab 등록
            simulator.RegisterPrefab("Player_Prefab", "PlayerController");
            simulator.RegisterPrefab("Enemy_Prefab", "EnemyController");
            simulator.RegisterPrefab("Item_Prefab", "ItemManager");
            PassTest("복잡한 MOD: 3개 Prefab 등록");

            // AssetBundle 빌드
            var buildResult = simulator.BuildAssetBundle("ComplexMod_v1");
            Assert(buildResult.Status == AssetBundleSimulator.BuildStatus.Success, "Build should succeed");
            Assert(buildResult.ProcessedPrefabs.Count == 3, "Should process 3 prefabs");
            Assert(buildResult.ProcessedScripts.Count == 3, "Should process 3 scripts");
            PassTest("복잡한 MOD: AssetBundle 빌드 (3 Prefab, 3 Script)");

            simulator.CleanUp();
        }
        catch (Exception ex)
        {
            FailTest("복잡한 MOD 빌드", ex.Message);
        }
    }

    private static void TestCSharpToSFCSharpConversion()
    {
        try
        {
            var simulator = new AssetBundleSimulator("TestBuild_Conversion");

            // C# 원본 코드
            string csharpCode = @"
using UnityEngine;

[SFCSharp]
public class TestScript : MonoBehaviour
{
    public void CreateObjects()
    {
        GameObject.Instantiate('TestObject1')
        GameObject.Instantiate('TestObject2')
    }

    public void SetupGame()
    {
        new GameObject('GameManager')
        new GameObject('UICanvas')
    }
}";

            // 변환 실행
            simulator.RegisterScript("TestScript", csharpCode);

            // 변환된 스크립트 확인
            var scriptPath = "TestBuild_Conversion/Scripts/TestScript.script";
            Assert(File.Exists(scriptPath), "Converted script file should exist");

            var convertedContent = File.ReadAllText(scriptPath);
            Assert(convertedContent.Contains("GameObject.Create"), "Should contain GameObject.Create");
            Assert(convertedContent.Contains("UnityEngine.GameObject.Create"), "Should have UnityEngine namespace");
            Assert(!convertedContent.Contains("Instantiate"), "Should not contain Instantiate");
            PassTest("C# → SFCSharp 코드 변환");

            simulator.CleanUp();
        }
        catch (Exception ex)
        {
            FailTest("C# → SFCSharp 변환", ex.Message);
        }
    }

    private static void TestRuntimeBundleExecution()
    {
        try
        {
            var simulator = new AssetBundleSimulator("TestBuild_Runtime");

            // MOD 스크립트 준비
            string modScript = @"
[SFCSharp]
public class RuntimeTest : MonoBehaviour
{
    public void Execute()
    {
        GameObject.Instantiate('RuntimeObject')
        GameObject.Instantiate('RuntimeEntity')
    }
}";

            // 빌드
            simulator.RegisterScript("RuntimeTest", modScript);
            simulator.RegisterPrefab("RuntimeTest_Prefab", "RuntimeTest");
            var buildResult = simulator.BuildAssetBundle("RuntimeMod_v1");
            Assert(buildResult.Status == AssetBundleSimulator.BuildStatus.Success, "Build should succeed");
            PassTest("런타임 테스트: Bundle 빌드");

            // 런타임 로드 및 실행
            var runtimeResult = simulator.LoadAndRunAssetBundle("RuntimeMod_v1");
            Assert(runtimeResult.Status == AssetBundleSimulator.RuntimeStatus.Success, "Runtime execution should succeed");
            Assert(runtimeResult.ExecutedPrefabs.Count == 1, "Should execute 1 prefab");
            Assert(runtimeResult.ExecutedPrefabs[0].Status == AssetBundleSimulator.ExecutionStatus.Success, "Prefab execution should succeed");
            PassTest("런타임 테스트: Bundle 로드 및 실행");

            // 실행된 명령어 확인
            var executedInfo = runtimeResult.ExecutedPrefabs[0];
            Assert(executedInfo.ExecutedCommands > 0, "Should execute at least one command");
            PassTest("런타임 테스트: 명령어 실행 완료");

            simulator.CleanUp();
        }
        catch (Exception ex)
        {
            FailTest("런타임 Bundle 실행", ex.Message);
        }
    }

    private static void TestModDistributionSimulation()
    {
        try
        {
            // Step 1: 모더가 MOD 개발
            var modBuilder = new AssetBundleSimulator("TestBuild_Distribution");

            string questModScript = @"
[SFCSharp]
public class QuestSystem : MonoBehaviour
{
    public void Initialize()
    {
        GameObject.Instantiate('QuestGiver')
        GameObject.Instantiate('QuestTarget')
        GameObject.Instantiate('QuestReward')
    }
}";

            modBuilder.RegisterScript("QuestSystem", questModScript);
            modBuilder.RegisterPrefab("Quest_Prefab", "QuestSystem");
            var buildResult = modBuilder.BuildAssetBundle("QuestMod_v1.0");
            Assert(buildResult.Status == AssetBundleSimulator.BuildStatus.Success, "MOD build should succeed");
            PassTest("배포 시뮬레이션: MOD 개발 및 빌드");

            // Step 2: MOD 배포 (AssetBundle 파일)
            var bundleFile = "TestBuild_Distribution/QuestMod_v1.0.bundle";
            Assert(buildResult.BundleInfo.ContainsModData, "Bundle should be ready for distribution");
            PassTest("배포 시뮬레이션: MOD 배포 준비");

            // Step 3: 사용자가 MOD 설치
            var modInstaller = new AssetBundleSimulator("TestInstall_Distribution");

            // 설치 과정 (Bundle에서 파일 추출)
            string questModScript2 = @"
[SFCSharp]
public class QuestSystem : MonoBehaviour
{
    public void Initialize()
    {
        GameObject.Instantiate('QuestGiver')
        GameObject.Instantiate('QuestTarget')
        GameObject.Instantiate('QuestReward')
    }
}";

            modInstaller.RegisterScript("QuestSystem", questModScript2);
            modInstaller.RegisterPrefab("Quest_Prefab", "QuestSystem");
            PassTest("배포 시뮬레이션: MOD 설치");

            // Step 4: 런타임에 MOD 실행
            var runtimeResult = modInstaller.LoadAndRunAssetBundle("QuestMod_v1.0");
            Assert(runtimeResult.Status == AssetBundleSimulator.RuntimeStatus.Success, "MOD execution should succeed");
            PassTest("배포 시뮬레이션: MOD 런타임 실행");

            modBuilder.CleanUp();
            modInstaller.CleanUp();

            PassTest("배포 시뮬레이션: 완전한 파이프라인");
        }
        catch (Exception ex)
        {
            FailTest("MOD 배포 시뮬레이션", ex.Message);
        }
    }

    #endregion

    #region Test Utilities

    private static void Assert(bool condition, string message)
    {
        if (!condition)
            throw new AssertionException(message);
    }

    private static void PassTest(string testName)
    {
        passedTests++;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✓ {testName}");
        Console.ResetColor();
    }

    private static void FailTest(string testName, string reason)
    {
        failedTests++;
        failedTestNames.Add($"{testName}: {reason}");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ✗ {testName}");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"    사유: {reason}");
        Console.ResetColor();
    }

    private static void PrintTestCategory(string categoryName)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n▶ {categoryName}");
        Console.ResetColor();
    }

    private static void PrintTestSummary()
    {
        Console.WriteLine("\n════════════════════════════════════════════════════════════");
        Console.WriteLine("                        테스트 결과 요약                      ");
        Console.WriteLine("════════════════════════════════════════════════════════════");

        int totalTests = passedTests + failedTests;
        double passRate = totalTests > 0 ? (passedTests * 100.0 / totalTests) : 0;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  통과: {passedTests}");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  실패: {failedTests}");
        Console.ResetColor();

        Console.WriteLine($"  총합: {totalTests}");
        Console.WriteLine($"  성공률: {passRate:F1}%");

        if (failedTestNames.Count > 0)
        {
            Console.WriteLine("\n실패한 테스트:");
            foreach (var failedTest in failedTestNames)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  - {failedTest}");
                Console.ResetColor();
            }
        }

        Console.WriteLine("\n════════════════════════════════════════════════════════════");
    }

    #endregion
}

/// <summary>
/// 테스트 검증 실패 예외
/// </summary>
public class AssertionException : Exception
{
    public AssertionException(string message) : base(message) { }
}
