using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SFCSharp.Tester
{
    /// <summary>
    /// AssetBundle 빌드 과정을 시뮬레이션합니다.
    ///
    /// 실제 프로세스:
    /// 1. 모더가 Unity 프로젝트에서 C# 스크립트 작성 ([SFCSharp] 태그 붙음)
    /// 2. Prefab에 스크립트를 MonoBehaviour로 추가
    /// 3. AssetBundle 빌드 시작
    /// 4. 빌드 과정:
    ///    a) [SFCSharp] 태그된 스크립트 찾기
    ///    b) C# 코드를 SFCSharp 스크립트 문법으로 변환
    ///    c) 변환된 스크립트를 텍스트 파일로 저장 (.script)
    ///    d) Prefab의 MonoBehaviour를 SFCSharpExecutor로 교체
    ///    e) TextAsset 참조 설정
    /// 5. 빌드된 AssetBundle 생성
    /// 6. MOD 배포
    /// 7. 런타임에 MOD 설치 후 실행
    /// </summary>
    public class AssetBundleSimulator
    {
        /// <summary>
        /// 임시 빌드 디렉토리
        /// </summary>
        private readonly string _buildDirectory;

        /// <summary>
        /// 변환된 스크립트 저장 디렉토리
        /// </summary>
        private readonly string _scriptsDirectory;

        /// <summary>
        /// Prefab 정보 저장소
        /// </summary>
        private readonly List<PrefabInfo> _prefabs = new();

        public AssetBundleSimulator(string buildDirectory = "AssetBundleBuild")
        {
            _buildDirectory = buildDirectory;
            _scriptsDirectory = Path.Combine(buildDirectory, "Scripts");

            // 디렉토리 생성
            Directory.CreateDirectory(_buildDirectory);
            Directory.CreateDirectory(_scriptsDirectory);
        }

        /// <summary>
        /// C# 스크립트를 등록합니다 ([SFCSharp] 태그가 있는 스크립트)
        /// </summary>
        public void RegisterScript(string scriptName, string csharpCode)
        {
            // [SFCSharp] 태그 확인
            if (!csharpCode.Contains("[SFCSharp]"))
                throw new InvalidOperationException($"Script '{scriptName}' must have [SFCSharp] attribute");

            // C# 코드를 SFCSharp 스크립트로 변환
            var convertedScript = ConvertCSharpToSFCSharp(csharpCode);

            // 변환된 스크립트를 텍스트 파일로 저장
            string scriptPath = Path.Combine(_scriptsDirectory, $"{scriptName}.script");
            File.WriteAllText(scriptPath, convertedScript);
        }

        /// <summary>
        /// Prefab을 등록합니다 (SFCSharpExecutor가 자동으로 주입됨)
        /// </summary>
        public void RegisterPrefab(string prefabName, string scriptName)
        {
            var scriptPath = Path.Combine(_scriptsDirectory, $"{scriptName}.script");
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"Script not found: {scriptPath}");

            var prefabInfo = new PrefabInfo
            {
                PrefabName = prefabName,
                ScriptName = scriptName,
                ScriptPath = scriptPath,
                HasSFCSharpExecutor = true, // 빌드 시 자동으로 주입됨
                ExecutionState = ExecutionState.Ready
            };

            _prefabs.Add(prefabInfo);
        }

        /// <summary>
        /// AssetBundle 빌드를 시뮬레이션합니다
        /// </summary>
        public BuildResult BuildAssetBundle(string bundleName)
        {
            var result = new BuildResult
            {
                BundleName = bundleName,
                BuildTime = DateTime.Now,
                Status = BuildStatus.Success,
                ProcessedPrefabs = new(),
                ProcessedScripts = new()
            };

            try
            {
                // Step 1: 모든 Prefab 처리
                foreach (var prefab in _prefabs)
                {
                    result.ProcessedPrefabs.Add(new PrefabBuildInfo
                    {
                        PrefabName = prefab.PrefabName,
                        ScriptName = prefab.ScriptName,
                        SFCSharpExecutorInjected = true,
                        TextAssetAssigned = true
                    });
                }

                // Step 2: 모든 스크립트 파일이 올바르게 생성되었는지 확인
                var scriptFiles = Directory.GetFiles(_scriptsDirectory, "*.script");
                foreach (var scriptPath in scriptFiles)
                {
                    var scriptName = Path.GetFileNameWithoutExtension(scriptPath);
                    var scriptContent = File.ReadAllText(scriptPath);

                    result.ProcessedScripts.Add(new ScriptBuildInfo
                    {
                        ScriptName = scriptName,
                        ScriptPath = scriptPath,
                        ScriptSize = new FileInfo(scriptPath).Length,
                        LineCount = scriptContent.Split('\n').Length,
                        IsValid = ValidateScriptContent(scriptContent)
                    });
                }

                // Step 3: AssetBundle 메타정보 생성
                result.BundleInfo = new BundleInfo
                {
                    BundleName = bundleName,
                    TotalPrefabs = _prefabs.Count,
                    TotalScripts = scriptFiles.Length,
                    BundleSize = CalculateBundleSize(),
                    ContainsModData = scriptFiles.Length > 0
                };

                return result;
            }
            catch (Exception ex)
            {
                result.Status = BuildStatus.Failed;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 빌드된 AssetBundle을 런타임에서 로드하고 실행합니다
        /// </summary>
        public RuntimeLoadResult LoadAndRunAssetBundle(string bundleName)
        {
            var result = new RuntimeLoadResult
            {
                BundleName = bundleName,
                LoadTime = DateTime.Now,
                Status = RuntimeStatus.Success,
                ExecutedPrefabs = new()
            };

            try
            {
                foreach (var prefab in _prefabs)
                {
                    var scriptPath = Path.Combine(_scriptsDirectory, $"{prefab.ScriptName}.script");
                    if (!File.Exists(scriptPath))
                    {
                        result.ExecutedPrefabs.Add(new PrefabExecutionInfo
                        {
                            PrefabName = prefab.PrefabName,
                            Status = ExecutionStatus.Failed,
                            ErrorMessage = $"Script file not found: {scriptPath}"
                        });
                        continue;
                    }

                    // 스크립트 파일 읽기
                    var scriptContent = File.ReadAllText(scriptPath);

                    // 스크립트 파싱 및 실행
                    var commandLines = ParseScriptContent(scriptContent);

                    result.ExecutedPrefabs.Add(new PrefabExecutionInfo
                    {
                        PrefabName = prefab.PrefabName,
                        ScriptName = prefab.ScriptName,
                        ExecutedCommands = commandLines.Count,
                        Status = ExecutionStatus.Success
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Status = RuntimeStatus.Failed;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// C# 코드를 SFCSharp 스크립트 문법으로 변환합니다
        /// </summary>
        private string ConvertCSharpToSFCSharp(string csharpCode)
        {
            var result = new System.Text.StringBuilder();

            // 주석 처리
            result.AppendLine("// 이 파일은 AssetBundle 빌드 시 자동으로 생성되었습니다");
            result.AppendLine("// 원본: C# MonoBehaviour 스크립트");
            result.AppendLine();

            // [SFCSharp] 메서드 찾기 및 변환
            var methodMatches = Regex.Matches(csharpCode, @"public\s+void\s+(\w+)\s*\(\s*\)\s*{([^}]*)}", RegexOptions.Multiline);

            foreach (Match methodMatch in methodMatches)
            {
                var methodName = methodMatch.Groups[1].Value;
                var methodBody = methodMatch.Groups[2].Value;

                result.AppendLine($"// 메서드: {methodName}");

                // 메서드 본문의 GameObject.Instantiate 호출을 GameObject.Create로 변환
                var convertedBody = ConvertMethodBody(methodBody);
                result.AppendLine(convertedBody);
                result.AppendLine();
            }

            return result.ToString();
        }

        /// <summary>
        /// C# 메서드 본문을 SFCSharp 문법으로 변환합니다
        /// </summary>
        private string ConvertMethodBody(string body)
        {
            var result = body;

            // Instantiate -> Create 변환
            result = Regex.Replace(result, @"GameObject\.Instantiate\s*\(\s*([^,)]+)\s*\)", m =>
            {
                var param = m.Groups[1].Value.Trim();
                // 문자열 추출
                var match = Regex.Match(param, @"['\x22]([^'\x22]+)['\x22]");
                if (match.Success)
                    return $"UnityEngine.GameObject.Create('{match.Groups[1].Value}')";
                return m.Value;
            });

            // new GameObject -> GameObject.Create 변환
            result = Regex.Replace(result, @"new\s+GameObject\s*\(\s*['\x22]([^'\x22]+)['\x22]\s*\)", m =>
                $"UnityEngine.GameObject.Create('{m.Groups[1].Value}')"
            );

            // 주석 제거 (단일 라인)
            result = Regex.Replace(result, @"//.*$", "", RegexOptions.Multiline);

            // 공백 정리
            result = Regex.Replace(result, @"^\s*$\n", "", RegexOptions.Multiline);
            result = result.Trim();

            return result;
        }

        /// <summary>
        /// 스크립트 콘텐츠를 파싱합니다
        /// </summary>
        private List<string> ParseScriptContent(string content)
        {
            var commands = new List<string>();
            var lines = content.Split('\n');

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedLine) && !trimmedLine.StartsWith("//"))
                {
                    commands.Add(trimmedLine);
                }
            }

            return commands;
        }

        /// <summary>
        /// 스크립트 콘텐츠 유효성 검증
        /// </summary>
        private bool ValidateScriptContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            // 최소 하나의 명령어 확인
            var commandLines = ParseScriptContent(content);
            return commandLines.Count > 0;
        }

        /// <summary>
        /// AssetBundle 크기 계산
        /// </summary>
        private long CalculateBundleSize()
        {
            long totalSize = 0;
            var files = Directory.GetFiles(_scriptsDirectory);
            foreach (var file in files)
            {
                totalSize += new FileInfo(file).Length;
            }
            return totalSize;
        }

        /// <summary>
        /// 빌드 디렉토리 정리
        /// </summary>
        public void CleanUp()
        {
            if (Directory.Exists(_buildDirectory))
            {
                Directory.Delete(_buildDirectory, true);
            }
        }

        #region Inner Classes

        public class PrefabInfo
        {
            public string PrefabName { get; set; }
            public string ScriptName { get; set; }
            public string ScriptPath { get; set; }
            public bool HasSFCSharpExecutor { get; set; }
            public ExecutionState ExecutionState { get; set; }
        }

        public enum ExecutionState
        {
            Ready,
            Running,
            Completed,
            Failed
        }

        public class BuildResult
        {
            public string BundleName { get; set; }
            public DateTime BuildTime { get; set; }
            public BuildStatus Status { get; set; }
            public string ErrorMessage { get; set; }
            public List<PrefabBuildInfo> ProcessedPrefabs { get; set; }
            public List<ScriptBuildInfo> ProcessedScripts { get; set; }
            public BundleInfo BundleInfo { get; set; }

            public override string ToString()
            {
                return $"Bundle: {BundleName} | Status: {Status} | Prefabs: {ProcessedPrefabs?.Count ?? 0} | Scripts: {ProcessedScripts?.Count ?? 0}";
            }
        }

        public enum BuildStatus
        {
            Success,
            Failed
        }

        public class PrefabBuildInfo
        {
            public string PrefabName { get; set; }
            public string ScriptName { get; set; }
            public bool SFCSharpExecutorInjected { get; set; }
            public bool TextAssetAssigned { get; set; }
        }

        public class ScriptBuildInfo
        {
            public string ScriptName { get; set; }
            public string ScriptPath { get; set; }
            public long ScriptSize { get; set; }
            public int LineCount { get; set; }
            public bool IsValid { get; set; }
        }

        public class BundleInfo
        {
            public string BundleName { get; set; }
            public int TotalPrefabs { get; set; }
            public int TotalScripts { get; set; }
            public long BundleSize { get; set; }
            public bool ContainsModData { get; set; }
        }

        public class RuntimeLoadResult
        {
            public string BundleName { get; set; }
            public DateTime LoadTime { get; set; }
            public RuntimeStatus Status { get; set; }
            public string ErrorMessage { get; set; }
            public List<PrefabExecutionInfo> ExecutedPrefabs { get; set; }

            public override string ToString()
            {
                return $"Bundle: {BundleName} | Status: {Status} | Executed: {ExecutedPrefabs?.Count ?? 0}";
            }
        }

        public enum RuntimeStatus
        {
            Success,
            Failed
        }

        public class PrefabExecutionInfo
        {
            public string PrefabName { get; set; }
            public string ScriptName { get; set; }
            public int ExecutedCommands { get; set; }
            public ExecutionStatus Status { get; set; }
            public string ErrorMessage { get; set; }
        }

        public enum ExecutionStatus
        {
            Success,
            Failed
        }

        #endregion
    }
}
