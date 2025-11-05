using SFCSharp.Analyzer;
using SFCSharp.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SFCSharp.Core
{
    /// <summary>
    /// SFCSharp 스크립트 빌드 클래스
    /// 스크립트 소스 코드를 분석하고 컴파일하여 SFContext를 생성합니다.
    /// </summary>
    public class SFContextBuilder : ISFContextBuilder
    {
        private readonly SFContextManager _contextManager;
        private readonly SFScriptCompiler _compiler;
        private readonly Dictionary<string, SFContext> _builtContexts;

        public SFContextBuilder(SFContextManager contextManager)
        {
            _contextManager = contextManager;
            _compiler = new SFScriptCompiler();
            _builtContexts = new Dictionary<string, SFContext>();
        }

        /// <summary>
        /// 스크립트 소스 코드를 빌드합니다.
        /// </summary>
        /// <param name="script">C# 스크립트 소스 코드</param>
        /// <returns>빌드된 SFContext</returns>
        public SFContext Build(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
                throw new ArgumentException("스크립트가 비어있습니다.");

            try
            {
                // 1. 스크립트 분석
                var contextInfo = AnalyzeScript(script);

                // 2. 컴파일 (메타데이터 추출)
                Assembly assembly = _compiler.CompileSource(script, contextInfo.ClassName);

                // 3. SFContext 생성
                SFContext context = CreateContext(assembly, contextInfo);

                // 4. 캐시에 저장
                _builtContexts[contextInfo.ClassName] = context;

                return context;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"스크립트 빌드 실패: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 파일 경로로부터 스크립트를 빌드합니다.
        /// </summary>
        /// <param name="filePath">C# 파일 경로</param>
        /// <returns>빌드된 SFContext</returns>
        public SFContext BuildFromFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException($"파일을 찾을 수 없습니다: {filePath}");

            string script = System.IO.File.ReadAllText(filePath);
            return Build(script);
        }

        /// <summary>
        /// 여러 스크립트를 한 번에 빌드합니다.
        /// </summary>
        /// <param name="scripts">스크립트 소스 코드 배열</param>
        /// <returns>빌드된 SFContext 배열</returns>
        public SFContext[] BuildMultiple(params string[] scripts)
        {
            return scripts.Select(Build).ToArray();
        }

        /// <summary>
        /// 빌드된 컨텍스트를 반환합니다.
        /// </summary>
        /// <param name="contextName">컨텍스트 이름</param>
        /// <returns>SFContext 또는 null</returns>
        public SFContext? GetBuiltContext(string contextName)
        {
            return _builtContexts.ContainsKey(contextName) ? _builtContexts[contextName] : null;
        }

        /// <summary>
        /// 모든 빌드된 컨텍스트 목록을 반환합니다.
        /// </summary>
        public IEnumerable<string> GetBuiltContextNames => _builtContexts.Keys;

        /// <summary>
        /// 빌드 캐시를 초기화합니다.
        /// </summary>
        public void ClearBuildCache()
        {
            _builtContexts.Clear();
            _compiler.ClearCache();
        }

        /// <summary>
        /// 스크립트를 분석하여 메타데이터를 추출합니다.
        /// </summary>
        private ScriptAnalysisResult AnalyzeScript(string script)
        {
            return ContextAnalyzer.AnalyzeScript(script);
        }

        /// <summary>
        /// 어셈블리와 분석 결과로부터 SFContext를 생성합니다.
        /// </summary>
        private SFContext CreateContext(Assembly assembly, ScriptAnalysisResult analysisResult)
        {
            SFContext context = new SFContext(analysisResult.Namespace, analysisResult.ClassName);

            // 어셈블리에서 클래스를 찾아 메서드/프로퍼티 추출
            Type? scriptType = assembly.GetType($"{analysisResult.Namespace}.{analysisResult.ClassName}");

            if (scriptType == null)
            {
                throw new InvalidOperationException($"클래스 타입을 찾을 수 없습니다: {analysisResult.Namespace}.{analysisResult.ClassName}");
            }

            // 메서드 메타데이터 추출
            ExtractMethodMetadata(context, scriptType);

            // 프로퍼티/필드 메타데이터 추출
            ExtractVariableMetadata(context, scriptType);

            return context;
        }

        /// <summary>
        /// 클래스의 메서드 메타데이터를 추출합니다.
        /// </summary>
        private void ExtractMethodMetadata(SFContext context, Type scriptType)
        {
            MethodInfo[] methods = scriptType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (MethodInfo method in methods)
            {
                // 기본 메서드는 제외
                if (method.DeclaringType == typeof(object))
                    continue;

                // TODO: 메서드 메타데이터를 SFContext의 메서드 딕셔너리에 추가
                // SFMethod sfMethod = new SFMethod(context, method.Name);
            }
        }

        /// <summary>
        /// 클래스의 프로퍼티/필드 메타데이터를 추출합니다.
        /// </summary>
        private void ExtractVariableMetadata(SFContext context, Type scriptType)
        {
            PropertyInfo[] properties = scriptType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (PropertyInfo property in properties)
            {
                // TODO: 프로퍼티 메타데이터를 SFContext의 변수 딕셔너리에 추가
            }

            FieldInfo[] fields = scriptType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (FieldInfo field in fields)
            {
                // TODO: 필드 메타데이터를 SFContext의 변수 딕셔너리에 추가
            }
        }
    }
}
