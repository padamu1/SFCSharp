using System;
using System.Text.RegularExpressions;

namespace SFCSharp.Analyzer
{
    /// <summary>
    /// SFCSharp 스크립트 분석기
    /// C# 소스 코드를 분석하여 네임스페이스, 클래스명 등의 메타데이터를 추출합니다.
    /// </summary>
    public static class ContextAnalyzer
    {
        /// <summary>
        /// 스크립트를 분석합니다.
        /// </summary>
        /// <param name="script">C# 스크립트 소스 코드</param>
        /// <returns>분석 결과</returns>
        public static ScriptAnalysisResult AnalyzeScript(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
                throw new ArgumentException("스크립트가 비어있습니다.");

            var result = new ScriptAnalysisResult();

            // 네임스페이스 추출
            result.Namespace = ExtractNamespace(script) ?? "SFCSharp.Scripts";

            // 클래스명 추출
            result.ClassName = ExtractClassName(script);

            if (string.IsNullOrWhiteSpace(result.ClassName))
                throw new InvalidOperationException("공개 클래스를 찾을 수 없습니다.");

            return result;
        }

        /// <summary>
        /// 네임스페이스를 추출합니다.
        /// </summary>
        private static string? ExtractNamespace(string script)
        {
            // namespace keyword 검색
            Regex namespaceRegex = new Regex(@"namespace\s+([a-zA-Z_][a-zA-Z0-9._]*)", RegexOptions.Multiline);
            Match match = namespaceRegex.Match(script);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        /// <summary>
        /// 공개 클래스명을 추출합니다.
        /// </summary>
        private static string? ExtractClassName(string script)
        {
            // public class keyword 검색
            Regex classRegex = new Regex(@"public\s+(?:partial\s+)?class\s+([a-zA-Z_][a-zA-Z0-9_]*)", RegexOptions.Multiline);
            Match match = classRegex.Match(script);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            // public struct도 지원
            Regex structRegex = new Regex(@"public\s+(?:partial\s+)?struct\s+([a-zA-Z_][a-zA-Z0-9_]*)", RegexOptions.Multiline);
            match = structRegex.Match(script);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        /// <summary>
        /// (레거시) 전체 스크립트를 분석합니다.
        /// </summary>
        public static void Analyze(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
                throw new ArgumentException("스크립트가 비어있습니다.");

            Parse(script);
        }

        /// <summary>
        /// (레거시) 스크립트를 파싱합니다.
        /// </summary>
        private static void Parse(string script)
        {
            // 현재는 AnalyzeScript 메서드로 통합됨
            AnalyzeScript(script);
        }
    }

    /// <summary>
    /// 스크립트 분석 결과를 담는 클래스
    /// </summary>
    public class ScriptAnalysisResult
    {
        /// <summary>
        /// 스크립트의 네임스페이스
        /// </summary>
        public string Namespace { get; set; } = "SFCSharp.Scripts";

        /// <summary>
        /// 스크립트의 클래스명
        /// </summary>
        public string ClassName { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"ScriptAnalysisResult(Namespace={Namespace}, ClassName={ClassName})";
        }
    }
}
