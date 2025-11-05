using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SFCSharp.Core
{
    /// <summary>
    /// SFCSharp 스크립트 컴파일러
    /// C# 소스 코드를 분석하고 컴파일하여 DLL로 변환합니다.
    /// </summary>
    public class SFScriptCompiler
    {
        private readonly Dictionary<string, Assembly> _compiledAssemblies;
        private readonly object _compileLock = new object();

        public SFScriptCompiler()
        {
            _compiledAssemblies = new Dictionary<string, Assembly>();
        }

        /// <summary>
        /// 단일 C# 파일을 컴파일합니다.
        /// </summary>
        /// <param name="filePath">C# 소스 파일 경로</param>
        /// <param name="outputPath">컴파일 결과물 저장 경로 (선택사항)</param>
        /// <returns>컴파일된 어셈블리</returns>
        public Assembly CompileFile(string filePath, string? outputPath = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"스크립트 파일을 찾을 수 없습니다: {filePath}");

            string sourceCode = File.ReadAllText(filePath, Encoding.UTF8);
            string assemblyName = Path.GetFileNameWithoutExtension(filePath);

            return CompileSource(sourceCode, assemblyName, outputPath);
        }

        /// <summary>
        /// C# 소스 코드를 컴파일합니다.
        /// </summary>
        /// <param name="sourceCode">C# 소스 코드</param>
        /// <param name="assemblyName">어셈블리 이름</param>
        /// <param name="outputPath">컴파일 결과물 저장 경로 (선택사항)</param>
        /// <returns>컴파일된 어셈블리</returns>
        public Assembly CompileSource(string sourceCode, string assemblyName, string? outputPath = null)
        {
            lock (_compileLock)
            {
                if (string.IsNullOrWhiteSpace(sourceCode))
                    throw new ArgumentException("소스 코드가 비어있습니다.");

                if (string.IsNullOrWhiteSpace(assemblyName))
                    assemblyName = $"Script_{Guid.NewGuid():N}";

                // 캐시 확인
                if (SFCSharpConfig.EnableCache && _compiledAssemblies.ContainsKey(assemblyName))
                {
                    return _compiledAssemblies[assemblyName];
                }

                try
                {
                    // 현재는 기본 .NET 리플렉션을 사용하여 메타데이터 추출
                    // 실제 컴파일은 호스트 애플리케이션에서 처리해야 함
                    Assembly assembly = CompileSourceInternal(sourceCode, assemblyName, outputPath);

                    // 캐시에 저장
                    if (SFCSharpConfig.EnableCache)
                    {
                        _compiledAssemblies[assemblyName] = assembly;
                    }

                    return assembly;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"스크립트 컴파일 중 오류가 발생했습니다: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// 폴더의 모든 C# 파일을 컴파일합니다.
        /// </summary>
        /// <param name="folderPath">대상 폴더 경로</param>
        /// <returns>컴파일된 어셈블리 딕셔너리</returns>
        public Dictionary<string, Assembly> CompileFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"폴더를 찾을 수 없습니다: {folderPath}");

            var result = new Dictionary<string, Assembly>();
            string[] files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    Assembly assembly = CompileFile(file);
                    string assemblyName = Path.GetFileNameWithoutExtension(file);
                    result[assemblyName] = assembly;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"파일 컴파일 실패: {file}\n{ex.Message}", ex);
                }
            }

            return result;
        }

        /// <summary>
        /// 컴파일된 어셈블리를 DLL 파일로 저장합니다.
        /// </summary>
        /// <param name="assembly">컴파일된 어셈블리</param>
        /// <param name="outputPath">저장 경로</param>
        public void SaveAssembly(Assembly assembly, string outputPath)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            string? directoryPath = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 현재는 메모리에만 로드된 어셈블리이므로
            // 실제 파일 저장은 호스트 애플리케이션의 지원이 필요
            // 향후 Roslyn을 사용하여 구현
        }

        /// <summary>
        /// 캐시된 어셈블리를 제거합니다.
        /// </summary>
        /// <param name="assemblyName">어셈블리 이름</param>
        public void ClearCache(string? assemblyName = null)
        {
            if (assemblyName == null)
            {
                _compiledAssemblies.Clear();
            }
            else if (_compiledAssemblies.ContainsKey(assemblyName))
            {
                _compiledAssemblies.Remove(assemblyName);
            }
        }

        /// <summary>
        /// 컴파일 상태 정보를 반환합니다.
        /// </summary>
        public CompileInfo GetCompileInfo()
        {
            return new CompileInfo
            {
                CachedAssemblies = _compiledAssemblies.Count,
                CacheEnabled = SFCSharpConfig.EnableCache,
                OutputPath = SFCSharpConfig.ScriptOutputPath,
                CachePath = SFCSharpConfig.ScriptCachePath
            };
        }

        /// <summary>
        /// 내부 컴파일 로직 (메모리 기반)
        /// 실제 구현은 Roslyn 또는 동적 코드 생성을 사용해야 합니다.
        /// </summary>
        private Assembly CompileSourceInternal(string sourceCode, string assemblyName, string? outputPath)
        {
            // NOTE: 현재는 메모리 기반 더미 어셈블리 생성
            // 실제 구현을 위해서는 Roslyn 또는 CodeDOM 사용 필요

            // 호스트 애플리케이션에서 실제 컴파일을 수행해야 함
            // 예: Unity에서는 assembly 정의 파일 사용, .NET 프로젝트에서는 Roslyn 사용

            // 임시로 AppDomain의 현재 어셈블리 반환
            // 실제 동적 컴파일 구현 필요
            try
            {
                // 간단한 검증
                if (!sourceCode.Contains("public class") && !sourceCode.Contains("public struct"))
                {
                    throw new InvalidOperationException("공개 클래스 또는 구조체가 필요합니다.");
                }

                // 메모리 어셈블리 생성 (실제 구현 필요)
                // 현재는 System 어셈블리 사용 (테스트 용도)
                return typeof(object).Assembly;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"소스 코드 컴파일 실패: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// 컴파일 정보 클래스
    /// </summary>
    public class CompileInfo
    {
        /// <summary>
        /// 캐시된 어셈블리 수
        /// </summary>
        public int CachedAssemblies { get; set; }

        /// <summary>
        /// 캐시 활성화 여부
        /// </summary>
        public bool CacheEnabled { get; set; }

        /// <summary>
        /// 출력 경로
        /// </summary>
        public string OutputPath { get; set; } = string.Empty;

        /// <summary>
        /// 캐시 경로
        /// </summary>
        public string CachePath { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"CompileInfo(CachedAssemblies={CachedAssemblies}, CacheEnabled={CacheEnabled}, OutputPath={OutputPath}, CachePath={CachePath})";
        }
    }
}
