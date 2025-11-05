using System;
using System.IO;

namespace SFCSharp.Core
{
    /// <summary>
    /// SFCSharp 시스템 전역 설정 클래스
    /// 빌드 경로, 캐시 경로, 네임스페이스 등을 관리합니다.
    /// </summary>
    public static class SFCSharpConfig
    {
        private static string? _scriptOutputPath;
        private static string? _scriptCachePath;
        private static string? _scriptSourcePath;
        private static string? _defaultNamespace = "SFCSharp.Scripts";
        private static bool _enableCache = true;
        private static bool _isInitialized = false;

        /// <summary>
        /// 컴파일된 스크립트 DLL이 저장될 경로
        /// 기본값: {ApplicationBase}/Scripts/Output
        /// </summary>
        public static string ScriptOutputPath
        {
            get
            {
                if (string.IsNullOrEmpty(_scriptOutputPath))
                {
                    _scriptOutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "Output");
                }
                return _scriptOutputPath;
            }
            set => _scriptOutputPath = value;
        }

        /// <summary>
        /// 빌드 캐시가 저장될 경로
        /// 기본값: {ApplicationBase}/Scripts/Cache
        /// </summary>
        public static string ScriptCachePath
        {
            get
            {
                if (string.IsNullOrEmpty(_scriptCachePath))
                {
                    _scriptCachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "Cache");
                }
                return _scriptCachePath;
            }
            set => _scriptCachePath = value;
        }

        /// <summary>
        /// 원본 스크립트 파일의 기본 경로
        /// 기본값: {ApplicationBase}/Scripts/Source
        /// </summary>
        public static string ScriptSourcePath
        {
            get
            {
                if (string.IsNullOrEmpty(_scriptSourcePath))
                {
                    _scriptSourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "Source");
                }
                return _scriptSourcePath;
            }
            set => _scriptSourcePath = value;
        }

        /// <summary>
        /// 컴파일된 스크립트의 기본 네임스페이스
        /// </summary>
        public static string DefaultNamespace
        {
            get => _defaultNamespace ?? "SFCSharp.Scripts";
            set => _defaultNamespace = value;
        }

        /// <summary>
        /// 빌드 캐시 사용 여부
        /// </summary>
        public static bool EnableCache
        {
            get => _enableCache;
            set => _enableCache = value;
        }

        /// <summary>
        /// SFCSharp 설정 초기화
        /// 필요한 디렉토리를 자동으로 생성합니다.
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
                return;

            try
            {
                // 필요한 디렉토리 생성
                CreateDirectoryIfNotExists(ScriptOutputPath);
                CreateDirectoryIfNotExists(ScriptCachePath);
                CreateDirectoryIfNotExists(ScriptSourcePath);

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("SFCSharp 설정 초기화 중 오류가 발생했습니다.", ex);
            }
        }

        /// <summary>
        /// 사용자 정의 설정으로 SFCSharp 초기화
        /// </summary>
        public static void Initialize(string scriptOutputPath, string scriptCachePath, string scriptSourcePath)
        {
            _scriptOutputPath = scriptOutputPath;
            _scriptCachePath = scriptCachePath;
            _scriptSourcePath = scriptSourcePath;

            Initialize();
        }

        /// <summary>
        /// 설정 리셋 (주로 테스트용)
        /// </summary>
        public static void Reset()
        {
            _scriptOutputPath = null;
            _scriptCachePath = null;
            _scriptSourcePath = null;
            _defaultNamespace = "SFCSharp.Scripts";
            _enableCache = true;
            _isInitialized = false;
        }

        /// <summary>
        /// 초기화 상태 확인
        /// </summary>
        public static bool IsInitialized => _isInitialized;

        private static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
