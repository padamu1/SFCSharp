using System;
using System.Collections.Generic;
using System.IO;

namespace SFCSharp.Build
{
    /// <summary>
    /// SFCSharp 빌드 커맨드라인 도구
    ///
    /// 사용법:
    /// dotnet build-sfcsharp [옵션]
    ///
    /// 옵션:
    ///   --source          C# 스크립트 소스 디렉토리 (기본값: Assets/Scripts)
    ///   --prefabs         Prefab 소스 디렉토리 (기본값: Assets/Prefabs)
    ///   --output          빌드 출력 디렉토리 (기본값: Build/SFCSharp)
    ///   --bundle-name     AssetBundle 이름 (기본값: MOD)
    ///   --version         Bundle 버전 (기본값: 1.0.0)
    /// </summary>
    public class BuildCommand
    {
        public static int Main(string[] args)
        {
            try
            {
                var config = ParseArguments(args);
                return ExecuteBuild(config);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"빌드 오류: {ex.Message}");
                Console.ResetColor();
                return 1;
            }
        }

        private static SFCSharpBuildConfig ParseArguments(string[] args)
        {
            var config = new SFCSharpBuildConfig();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--source":
                        if (i + 1 < args.Length)
                            config.SourceScriptDirectory = args[++i];
                        break;

                    case "--prefabs":
                        if (i + 1 < args.Length)
                            config.SourcePrefabDirectory = args[++i];
                        break;

                    case "--output":
                        if (i + 1 < args.Length)
                            config.OutputDirectory = args[++i];
                        break;

                    case "--bundle-name":
                        if (i + 1 < args.Length)
                            config.BundleName = args[++i];
                        break;

                    case "--version":
                        if (i + 1 < args.Length)
                            config.BundleVersion = args[++i];
                        break;

                    case "--help":
                        PrintHelp();
                        return null;
                }
            }

            return config;
        }

        private static int ExecuteBuild(SFCSharpBuildConfig config)
        {
            if (config == null)
                return 0;

            Console.WriteLine("════════════════════════════════════════════════════════════");
            Console.WriteLine("           SFCSharp Build Processor                          ");
            Console.WriteLine("════════════════════════════════════════════════════════════\n");

            PrintConfig(config);

            Console.WriteLine("\n빌드 시작...\n");

            var processor = new SFCSharpBuildProcessor(config);
            var result = processor.Build();

            PrintBuildResult(result);

            return result.Status == BuildStatus.Success ? 0 : 1;
        }

        private static void PrintConfig(SFCSharpBuildConfig config)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("빌드 설정:");
            Console.ResetColor();
            Console.WriteLine($"  Source Scripts:    {config.SourceScriptDirectory}");
            Console.WriteLine($"  Source Prefabs:    {config.SourcePrefabDirectory}");
            Console.WriteLine($"  Output Directory:  {config.OutputDirectory}");
            Console.WriteLine($"  Bundle Name:       {config.BundleName}");
            Console.WriteLine($"  Bundle Version:    {config.BundleVersion}");
        }

        private static void PrintBuildResult(BuildResult result)
        {
            Console.WriteLine("\n════════════════════════════════════════════════════════════");
            Console.WriteLine("                      빌드 결과                             ");
            Console.WriteLine("════════════════════════════════════════════════════════════\n");

            Console.Write("상태: ");
            if (result.Status == BuildStatus.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("성공 ✓");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"실패 ✗");
            }
            Console.ResetColor();

            Console.WriteLine($"빌드 시간: {result.BuildTime.TotalSeconds:F2}초");
            Console.WriteLine($"처리된 스크립트: {result.ProcessedScripts.Count}개");
            Console.WriteLine($"처리된 Prefab: {result.ProcessedPrefabs.Count}개");

            if (result.Status == BuildStatus.Failed && !string.IsNullOrEmpty(result.ErrorMessage))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n오류: {result.ErrorMessage}");
                Console.ResetColor();
            }

            // 로그 출력
            if (result.Logs.Count > 0)
            {
                Console.WriteLine("\n빌드 로그:");
                foreach (var log in result.Logs)
                {
                    Console.WriteLine($"  {log}");
                }
            }

            // 처리된 스크립트
            if (result.ProcessedScripts.Count > 0)
            {
                Console.WriteLine("\n처리된 스크립트:");
                foreach (var script in result.ProcessedScripts)
                {
                    var statusColor = script.Status == ProcessStatus.Success ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.ForegroundColor = statusColor;
                    Console.Write($"  {(script.Status == ProcessStatus.Success ? "✓" : "✗")} ");
                    Console.ResetColor();
                    Console.WriteLine($"{script.ScriptName} ({script.LineCount} lines)");

                    if (!string.IsNullOrEmpty(script.ErrorMessage))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"    오류: {script.ErrorMessage}");
                        Console.ResetColor();
                    }
                }
            }

            // 처리된 Prefab
            if (result.ProcessedPrefabs.Count > 0)
            {
                Console.WriteLine("\n처리된 Prefab:");
                foreach (var prefab in result.ProcessedPrefabs)
                {
                    var statusColor = prefab.Status == ProcessStatus.Success ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.ForegroundColor = statusColor;
                    Console.Write($"  {(prefab.Status == ProcessStatus.Success ? "✓" : "✗")} ");
                    Console.ResetColor();
                    Console.WriteLine($"{prefab.PrefabName}");

                    if (prefab.Status == ProcessStatus.Success)
                    {
                        Console.WriteLine($"    연결 스크립트: {prefab.AssociatedScriptName}");
                        Console.WriteLine($"    SFCSharpExecutor 주입: {(prefab.SFCSharpExecutorInjected ? "O" : "X")}");
                        Console.WriteLine($"    TextAsset 설정: {(prefab.TextAssetAssigned ? "O" : "X")}");
                    }
                    else if (!string.IsNullOrEmpty(prefab.ErrorMessage))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"    오류: {prefab.ErrorMessage}");
                        Console.ResetColor();
                    }
                }
            }

            Console.WriteLine("\n════════════════════════════════════════════════════════════");
        }

        private static void PrintHelp()
        {
            Console.WriteLine(@"
SFCSharp 빌드 커맨드라인 도구

사용법:
  SFCSharp.Build.exe [옵션]

옵션:
  --source <경로>           C# 스크립트 소스 디렉토리
                            기본값: Assets/Scripts

  --prefabs <경로>          Prefab 소스 디렉토리
                            기본값: Assets/Prefabs

  --output <경로>           빌드 출력 디렉토리
                            기본값: Build/SFCSharp

  --bundle-name <이름>      생성할 AssetBundle 이름
                            기본값: MOD

  --version <버전>          Bundle 버전 (semantic versioning)
                            기본값: 1.0.0

  --help                    이 도움말 표시

예시:
  SFCSharp.Build.exe --source Assets/MyScripts --bundle-name MyMod

설명:
  1. --source 디렉토리에서 [SFCSharp] 속성이 있는 C# 파일 검색
  2. C# 코드를 SFCSharp 스크립트 형식으로 자동 변환
  3. --prefabs 디렉토리에서 해당 스크립트를 포함한 Prefab 찾기
  4. Prefab에서 MonoBehaviour를 SFCSharpExecutor로 교체
  5. 변환된 스크립트를 TextAsset으로 참조 설정
  6. --output 디렉토리에 빌드 결과 생성
");
        }
    }
}
