using SFCSharp.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SFCSharp.Core
{
    /// <summary>
    /// SFCSharp 스크립트 로더
    /// 컴파일된 DLL을 로드하고 인스턴스를 생성합니다.
    /// </summary>
    public class SFContextLoader : ISFContextLoader
    {
        private readonly SFContextManager _contextManager;
        private readonly Dictionary<string, Assembly> _loadedAssemblies;
        private readonly Dictionary<string, object> _loadedInstances;
        private readonly object _loadLock = new object();

        public SFContextLoader(SFContextManager contextManager)
        {
            _contextManager = contextManager;
            _loadedAssemblies = new Dictionary<string, Assembly>();
            _loadedInstances = new Dictionary<string, object>();
        }

        /// <summary>
        /// DLL 파일을 로드합니다.
        /// </summary>
        /// <param name="dllPath">DLL 파일 경로</param>
        /// <returns>로드된 어셈블리</returns>
        public Assembly LoadAssemblyFromFile(string dllPath)
        {
            lock (_loadLock)
            {
                if (!File.Exists(dllPath))
                    throw new FileNotFoundException($"DLL 파일을 찾을 수 없습니다: {dllPath}");

                try
                {
                    string assemblyName = Path.GetFileNameWithoutExtension(dllPath);

                    // 이미 로드된 경우 캐시된 어셈블리 반환
                    if (_loadedAssemblies.ContainsKey(assemblyName))
                    {
                        return _loadedAssemblies[assemblyName];
                    }

                    byte[] assemblyBytes = File.ReadAllBytes(dllPath);
                    Assembly assembly = Assembly.Load(assemblyBytes);

                    _loadedAssemblies[assemblyName] = assembly;
                    return assembly;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"DLL 로드 실패: {dllPath}\n{ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// 어셈블리에서 지정된 타입의 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="assembly">로드된 어셈블리</param>
        /// <param name="typeName">완전한 타입 이름 (예: "MyNamespace.MyClass")</param>
        /// <returns>생성된 인스턴스</returns>
        public object CreateInstance(Assembly assembly, string typeName)
        {
            lock (_loadLock)
            {
                try
                {
                    Type? type = assembly.GetType(typeName);

                    if (type == null)
                        throw new InvalidOperationException($"타입을 찾을 수 없습니다: {typeName}");

                    object? instance = Activator.CreateInstance(type);

                    if (instance == null)
                        throw new InvalidOperationException($"인스턴스 생성 실패: {typeName}");

                    return instance;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"인스턴스 생성 중 오류: {typeName}\n{ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// 빌드된 컨텍스트를 로드합니다. (이 메서드는 빌더와 상호작용)
        /// </summary>
        /// <param name="context">로드할 SFContext</param>
        public void Load(SFContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Note: 실제 구현은 외부 구성에 따라 달라집니다.
            // 여기서는 컨텍스트 검증만 수행합니다.
        }

        /// <summary>
        /// 스크립트 소스 코드를 로드합니다. (레거시 메서드)
        /// </summary>
        /// <param name="script">C# 스크립트 소스 코드</param>
        public void Load(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
                throw new ArgumentException("스크립트가 비어있습니다.");

            // Note: 이 메서드는 빌더를 통해 컨텍스트가 먼저 생성되어야 합니다.
            // 직접 스크립트 로드는 지원되지 않습니다.
            throw new NotSupportedException("이 메서드는 지원되지 않습니다. SFContextBuilder.Build()를 사용하세요.");
        }

        /// <summary>
        /// 폴더의 모든 DLL을 로드합니다.
        /// </summary>
        /// <param name="folderPath">DLL이 있는 폴더 경로</param>
        /// <returns>로드된 어셈블리 딕셔너리</returns>
        public Dictionary<string, Assembly> LoadFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"폴더를 찾을 수 없습니다: {folderPath}");

            var result = new Dictionary<string, Assembly>();
            string[] dllFiles = Directory.GetFiles(folderPath, "*.dll");

            foreach (string dllFile in dllFiles)
            {
                try
                {
                    Assembly assembly = LoadAssemblyFromFile(dllFile);
                    string assemblyName = Path.GetFileNameWithoutExtension(dllFile);
                    result[assemblyName] = assembly;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"DLL 로드 실패: {dllFile}\n{ex.Message}", ex);
                }
            }

            return result;
        }

        /// <summary>
        /// 로드된 인스턴스를 캐시에 저장합니다.
        /// </summary>
        /// <param name="key">캐시 키</param>
        /// <param name="instance">저장할 인스턴스</param>
        public void CacheInstance(string key, object instance)
        {
            lock (_loadLock)
            {
                _loadedInstances[key] = instance;
            }
        }

        /// <summary>
        /// 캐시된 인스턴스를 반환합니다.
        /// </summary>
        /// <param name="key">캐시 키</param>
        /// <returns>캐시된 인스턴스 또는 null</returns>
        public object? GetCachedInstance(string key)
        {
            lock (_loadLock)
            {
                return _loadedInstances.ContainsKey(key) ? _loadedInstances[key] : null;
            }
        }

        /// <summary>
        /// 모든 로드된 어셈블리를 반환합니다.
        /// </summary>
        public IEnumerable<string> GetLoadedAssemblies => _loadedAssemblies.Keys;

        /// <summary>
        /// 모든 캐시된 인스턴스를 반환합니다.
        /// </summary>
        public IEnumerable<string> GetCachedInstances => _loadedInstances.Keys;

        /// <summary>
        /// 로드 캐시를 초기화합니다.
        /// </summary>
        public void ClearLoadCache()
        {
            lock (_loadLock)
            {
                _loadedAssemblies.Clear();
                _loadedInstances.Clear();
            }
        }

        /// <summary>
        /// 로더의 상태 정보를 반환합니다.
        /// </summary>
        public LoaderInfo GetLoaderInfo()
        {
            return new LoaderInfo
            {
                LoadedAssemblies = _loadedAssemblies.Count,
                CachedInstances = _loadedInstances.Count,
                OutputPath = SFCSharpConfig.ScriptOutputPath
            };
        }
    }

    /// <summary>
    /// 로더 정보 클래스
    /// </summary>
    public class LoaderInfo
    {
        /// <summary>
        /// 로드된 어셈블리 수
        /// </summary>
        public int LoadedAssemblies { get; set; }

        /// <summary>
        /// 캐시된 인스턴스 수
        /// </summary>
        public int CachedInstances { get; set; }

        /// <summary>
        /// 출력 경로
        /// </summary>
        public string OutputPath { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"LoaderInfo(LoadedAssemblies={LoadedAssemblies}, CachedInstances={CachedInstances}, OutputPath={OutputPath})";
        }
    }
}
