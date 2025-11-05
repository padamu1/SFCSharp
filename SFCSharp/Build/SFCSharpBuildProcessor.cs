using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SFCSharp.Build
{
    /// <summary>
    /// SFCSharp MOD 빌드 프로세서
    ///
    /// Unity 프로젝트의 C# 스크립트를 자동으로 감지하고,
    /// [SFCSharp] 속성이 있는 경우 다음 과정을 수행합니다:
    ///
    /// 1. C# 코드 분석 ([SFCSharp] 속성 확인)
    /// 2. C# 코드를 SFCSharp 스크립트 문법으로 변환
    /// 3. 변환된 스크립트를 .script 파일로 저장
    /// 4. Prefab 수정:
    ///    a) MonoBehaviour 컴포넌트를 SFCSharpExecutor로 교체
    ///    b) TextAsset 참조 설정
    /// 5. AssetBundle 빌드
    /// </summary>
    public class SFCSharpBuildProcessor
    {
        /// <summary>
        /// 빌드 설정
        /// </summary>
        private readonly SFCSharpBuildConfig _config;

        /// <summary>
        /// 처리된 스크립트 목록
        /// </summary>
        private List<ScriptProcessResult> _processedScripts = new List<ScriptProcessResult>();

        /// <summary>
        /// 처리된 Prefab 목록
        /// </summary>
        private List<PrefabProcessResult> _processedPrefabs = new List<PrefabProcessResult>();

        public SFCSharpBuildProcessor(SFCSharpBuildConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// MOD 빌드를 시작합니다
        /// </summary>
        public BuildResult Build()
        {
            var result = new BuildResult
            {
                StartTime = DateTime.Now,
                Status = BuildStatus.InProgress
            };

            try
            {
                // Step 1: 빌드 디렉토리 준비
                PrepareBuildDirectory();
                result.AppendLog("Step 1: 빌드 디렉토리 준비 완료");

                // Step 2: [SFCSharp] 스크립트 찾기
                var scripts = FindSFCSharpScripts();
                result.AppendLog($"Step 2: {scripts.Count}개의 SFCSharp 스크립트 발견");

                // Step 3: 각 스크립트를 SFCSharp 형식으로 변환
                ConvertScripts(scripts);
                result.AppendLog($"Step 3: {_processedScripts.Count}개 스크립트 변환 완료");

                // Step 4: Prefab 수정
                var prefabs = FindPrefabsWithSFCSharpScripts();
                ModifyPrefabs(prefabs);
                result.AppendLog($"Step 4: {_processedPrefabs.Count}개 Prefab 수정 완료");

                // Step 5: 빌드 결과 패킹
                PackBuildOutput();
                result.AppendLog("Step 5: 빌드 결과 패킹 완료");

                result.Status = BuildStatus.Success;
                result.ProcessedScripts = _processedScripts;
                result.ProcessedPrefabs = _processedPrefabs;
            }
            catch (Exception ex)
            {
                result.Status = BuildStatus.Failed;
                result.ErrorMessage = ex.Message;
                result.AppendLog($"Error: {ex.Message}");
            }

            result.EndTime = DateTime.Now;
            return result;
        }

        /// <summary>
        /// 빌드 디렉토리를 준비합니다
        /// </summary>
        private void PrepareBuildDirectory()
        {
            if (Directory.Exists(_config.OutputDirectory))
                Directory.Delete(_config.OutputDirectory, true);

            Directory.CreateDirectory(_config.OutputDirectory);
            Directory.CreateDirectory(Path.Combine(_config.OutputDirectory, "Scripts"));
            Directory.CreateDirectory(Path.Combine(_config.OutputDirectory, "Prefabs"));
            Directory.CreateDirectory(Path.Combine(_config.OutputDirectory, "Bundles"));
        }

        /// <summary>
        /// [SFCSharp] 속성이 있는 C# 스크립트를 찾습니다
        /// </summary>
        private List<string> FindSFCSharpScripts()
        {
            var scripts = new List<string>();

            if (!Directory.Exists(_config.SourceScriptDirectory))
                return scripts;

            var csFiles = Directory.GetFiles(_config.SourceScriptDirectory, "*.cs", SearchOption.AllDirectories);

            foreach (var file in csFiles)
            {
                var content = File.ReadAllText(file);
                if (content.Contains("[SFCSharp]"))
                {
                    scripts.Add(file);
                }
            }

            return scripts;
        }

        /// <summary>
        /// C# 스크립트를 SFCSharp 형식으로 변환합니다
        /// </summary>
        private void ConvertScripts(List<string> scripts)
        {
            foreach (var scriptPath in scripts)
            {
                try
                {
                    var scriptContent = File.ReadAllText(scriptPath);
                    var scriptName = Path.GetFileNameWithoutExtension(scriptPath);

                    // C# 코드를 SFCSharp 스크립트로 변환
                    var convertedContent = ConvertCSharpToSFCSharp(scriptContent);

                    // 변환된 스크립트 저장
                    var outputPath = Path.Combine(_config.OutputDirectory, "Scripts", $"{scriptName}.script");
                    File.WriteAllText(outputPath, convertedContent);

                    _processedScripts.Add(new ScriptProcessResult
                    {
                        SourcePath = scriptPath,
                        OutputPath = outputPath,
                        ScriptName = scriptName,
                        Status = ProcessStatus.Success,
                        LineCount = convertedContent.Split('\n').Length
                    });
                }
                catch (Exception ex)
                {
                    _processedScripts.Add(new ScriptProcessResult
                    {
                        SourcePath = scriptPath,
                        ScriptName = Path.GetFileNameWithoutExtension(scriptPath),
                        Status = ProcessStatus.Failed,
                        ErrorMessage = ex.Message
                    });
                }
            }
        }

        /// <summary>
        /// [SFCSharp] 속성이 있는 스크립트를 포함한 Prefab을 찾습니다
        /// </summary>
        private List<string> FindPrefabsWithSFCSharpScripts()
        {
            var prefabs = new List<string>();

            if (!Directory.Exists(_config.SourcePrefabDirectory))
                return prefabs;

            // 실제로는 Unity Editor에서 Prefab의 MonoBehaviour를 검사해야 하지만,
            // 시뮬레이션이므로 규칙 기반으로 찾습니다
            var prefabFiles = Directory.GetFiles(_config.SourcePrefabDirectory, "*.prefab", SearchOption.AllDirectories);

            foreach (var prefab in prefabFiles)
            {
                // Prefab 파일에 [SFCSharp] 스크립트 참조가 있는지 확인
                var content = File.ReadAllText(prefab);
                foreach (var processedScript in _processedScripts)
                {
                    if (content.Contains(processedScript.ScriptName))
                    {
                        prefabs.Add(prefab);
                        break;
                    }
                }
            }

            return prefabs;
        }

        /// <summary>
        /// Prefab을 수정합니다
        /// </summary>
        private void ModifyPrefabs(List<string> prefabs)
        {
            foreach (var prefabPath in prefabs)
            {
                try
                {
                    var prefabName = Path.GetFileNameWithoutExtension(prefabPath);

                    // Prefab 수정 (실제로는 Unity Editor API 필요)
                    // 1. MonoBehaviour 컴포넌트를 찾아서 스크립트 이름 추출
                    var scriptName = ExtractScriptNameFromPrefab(prefabPath);

                    // 2. SFCSharpExecutor로 교체
                    var modifiedContent = ModifyPrefabContent(File.ReadAllText(prefabPath), scriptName);

                    // 3. 수정된 Prefab 저장
                    var outputPath = Path.Combine(_config.OutputDirectory, "Prefabs", $"{prefabName}.prefab");
                    File.WriteAllText(outputPath, modifiedContent);

                    _processedPrefabs.Add(new PrefabProcessResult
                    {
                        SourcePath = prefabPath,
                        OutputPath = outputPath,
                        PrefabName = prefabName,
                        AssociatedScriptName = scriptName,
                        Status = ProcessStatus.Success,
                        SFCSharpExecutorInjected = true,
                        TextAssetAssigned = true
                    });
                }
                catch (Exception ex)
                {
                    _processedPrefabs.Add(new PrefabProcessResult
                    {
                        SourcePath = prefabPath,
                        PrefabName = Path.GetFileNameWithoutExtension(prefabPath),
                        Status = ProcessStatus.Failed,
                        ErrorMessage = ex.Message
                    });
                }
            }
        }

        /// <summary>
        /// Prefab에서 스크립트 이름을 추출합니다
        /// </summary>
        private string ExtractScriptNameFromPrefab(string prefabPath)
        {
            var content = File.ReadAllText(prefabPath);

            // m_Script 필드에서 스크립트 이름 추출
            var match = Regex.Match(content, @"m_Script:\s*{fileID:\s*\d+,\s*guid:\s*(\w+)");
            if (match.Success)
            {
                // 처리된 스크립트 중에서 찾기
                foreach (var script in _processedScripts)
                {
                    if (content.Contains(Path.GetFileNameWithoutExtension(script.SourcePath)))
                    {
                        return Path.GetFileNameWithoutExtension(script.SourcePath);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Prefab 콘텐츠를 수정합니다 (MonoBehaviour → SFCSharpExecutor)
        /// </summary>
        private string ModifyPrefabContent(string prefabContent, string scriptName)
        {
            // 실제 구현에서는:
            // 1. 원본 MonoBehaviour 컴포넌트 제거
            // 2. SFCSharpExecutor 컴포넌트 추가
            // 3. scriptTextAsset 필드에 변환된 .script 파일 참조 설정

            var modified = prefabContent;

            // 마커 추가 (실제 구현에서는 복잡한 YAML 파싱 필요)
            modified = Regex.Replace(modified, @"m_Script:\s*\{[^}]*\}",
                "m_Script: {fileID: 11500000, guid: SFCSharpExecutor}");

            return modified;
        }

        /// <summary>
        /// 빌드 결과를 패킹합니다
        /// </summary>
        private void PackBuildOutput()
        {
            // AssetBundle 시뮬레이션
            var bundleDir = Path.Combine(_config.OutputDirectory, "Bundles");
            var bundleFile = Path.Combine(bundleDir, $"{_config.BundleName}.bundle");

            // 메타데이터 파일 생성
            var metadataJson = $@"{{
  ""bundleName"": ""{_config.BundleName}"",
  ""version"": ""{_config.BundleVersion}"",
  ""buildTime"": ""{DateTime.Now:yyyy-MM-dd HH:mm:ss}"",
  ""scripts"": {_processedScripts.Count},
  ""prefabs"": {_processedPrefabs.Count},
  ""contentHash"": ""{ComputeContentHash()}""
}}";

            File.WriteAllText(
                Path.Combine(_config.OutputDirectory, "build.manifest"),
                metadataJson
            );
        }

        /// <summary>
        /// 콘텐츠 해시를 계산합니다
        /// </summary>
        private string ComputeContentHash()
        {
            // 간단한 해시 계산
            var scriptDir = Path.Combine(_config.OutputDirectory, "Scripts");
            long totalBytes = 0;

            if (Directory.Exists(scriptDir))
            {
                foreach (var file in Directory.GetFiles(scriptDir))
                {
                    totalBytes += new FileInfo(file).Length;
                }
            }

            return totalBytes.ToString("X");
        }

        /// <summary>
        /// C# 코드를 SFCSharp 스크립트로 변환합니다
        /// </summary>
        private string ConvertCSharpToSFCSharp(string csharpCode)
        {
            var result = new System.Text.StringBuilder();

            result.AppendLine("// Auto-generated SFCSharp script from C# MonoBehaviour");
            result.AppendLine($"// Generated at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            result.AppendLine();

            // [SFCSharp] 메서드 찾기
            var methodMatches = Regex.Matches(csharpCode,
                @"public\s+void\s+(\w+)\s*\(\s*\)\s*\{([^}]*)\}",
                RegexOptions.Multiline);

            foreach (Match methodMatch in methodMatches)
            {
                var methodName = methodMatch.Groups[1].Value;
                var methodBody = methodMatch.Groups[2].Value;

                result.AppendLine($"// Method: {methodName}");

                // GameObject.Instantiate -> UnityEngine.GameObject.Create 변환
                var convertedBody = ConvertMethodBody(methodBody);
                result.Append(convertedBody);
                result.AppendLine();
            }

            return result.ToString();
        }

        /// <summary>
        /// 메서드 본문을 변환합니다
        /// </summary>
        private string ConvertMethodBody(string body)
        {
            var result = body;

            // GameObject.Instantiate -> UnityEngine.GameObject.Create
            result = Regex.Replace(result,
                @"GameObject\.Instantiate\s*\(\s*['\x22]([^'\x22]+)['\x22]\s*\)",
                m => $"UnityEngine.GameObject.Create('{m.Groups[1].Value}')"
            );

            // new GameObject -> UnityEngine.GameObject.Create
            result = Regex.Replace(result,
                @"new\s+GameObject\s*\(\s*['\x22]([^'\x22]+)['\x22]\s*\)",
                m => $"UnityEngine.GameObject.Create('{m.Groups[1].Value}')"
            );

            // 주석 제거
            result = Regex.Replace(result, @"//.*$", "", RegexOptions.Multiline);

            // 빈 줄 정리
            result = Regex.Replace(result, @"^\s*\n", "", RegexOptions.Multiline);

            return result.Trim();
        }
    }

    /// <summary>
    /// SFCSharp 빌드 설정
    /// </summary>
    public class SFCSharpBuildConfig
    {
        /// <summary>
        /// 소스 C# 스크립트 디렉토리
        /// </summary>
        public string SourceScriptDirectory { get; set; } = "Assets/Scripts";

        /// <summary>
        /// 소스 Prefab 디렉토리
        /// </summary>
        public string SourcePrefabDirectory { get; set; } = "Assets/Prefabs";

        /// <summary>
        /// 빌드 출력 디렉토리
        /// </summary>
        public string OutputDirectory { get; set; } = "Build/SFCSharp";

        /// <summary>
        /// AssetBundle 이름
        /// </summary>
        public string BundleName { get; set; } = "MOD";

        /// <summary>
        /// Bundle 버전
        /// </summary>
        public string BundleVersion { get; set; } = "1.0.0";

        /// <summary>
        /// 빌드 후 자동으로 AssetBundle 만들 것인지
        /// </summary>
        public bool AutoBuildAssetBundle { get; set; } = true;
    }

    /// <summary>
    /// 빌드 결과
    /// </summary>
    public class BuildResult
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public BuildStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public List<ScriptProcessResult> ProcessedScripts { get; set; } = new List<ScriptProcessResult>();
        public List<PrefabProcessResult> ProcessedPrefabs { get; set; } = new List<PrefabProcessResult>();
        private List<string> _logs = new List<string>();

        public IReadOnlyList<string> Logs => _logs.AsReadOnly();

        public void AppendLog(string message)
        {
            _logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        public TimeSpan BuildTime => EndTime - StartTime;

        public override string ToString()
        {
            return $"Status: {Status} | Duration: {BuildTime.TotalSeconds:F2}s | Scripts: {ProcessedScripts.Count} | Prefabs: {ProcessedPrefabs.Count}";
        }
    }

    public enum BuildStatus
    {
        InProgress,
        Success,
        Failed
    }

    /// <summary>
    /// 스크립트 처리 결과
    /// </summary>
    public class ScriptProcessResult
    {
        public string SourcePath { get; set; }
        public string OutputPath { get; set; }
        public string ScriptName { get; set; }
        public ProcessStatus Status { get; set; }
        public int LineCount { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Prefab 처리 결과
    /// </summary>
    public class PrefabProcessResult
    {
        public string SourcePath { get; set; }
        public string OutputPath { get; set; }
        public string PrefabName { get; set; }
        public string AssociatedScriptName { get; set; }
        public ProcessStatus Status { get; set; }
        public bool SFCSharpExecutorInjected { get; set; }
        public bool TextAssetAssigned { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum ProcessStatus
    {
        Success,
        Failed
    }
}
