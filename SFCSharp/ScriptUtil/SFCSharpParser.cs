using System.Text.RegularExpressions;
using System;

namespace SFCSharp.ScriptUtil
{
    public static class SFCSharpParser
    {
        public static void ExtractClassesWithSFCSharp(string sourceCode)
        {
            // SFCSharp 어트리뷰트를 찾고 클래스 이름과 바디를 추출하는 정규식
            string pattern = @"\[SFCSharp\]\s*public\s*class\s*(\w+)\s*\{([\s\S]*?)\}";
            Regex regex = new Regex(pattern);

            // 클래스 찾기
            MatchCollection matches = regex.Matches(sourceCode);

            foreach (Match match in matches)
            {
                string className = match.Groups[1].Value;   // 클래스 이름
                string classBody = match.Groups[2].Value;   // 클래스 내용

                Console.WriteLine($"Found class: {className}");
                Console.WriteLine($"Class body: {classBody}");

                // 여기서 메서드 추출도 추가 가능
                ExtractMethodsFromClass(classBody);
            }
        }

        public static void ExtractMethodsFromClass(string classBody)
        {
            // 메서드 시그니처와 바디를 추출하는 간단한 정규식
            string methodPattern = @"public\s*(\w+)\s*(\w+)\s*\(([^)]*)\)\s*\{([\s\S]*?)\}";
            Regex methodRegex = new Regex(methodPattern);

            MatchCollection methodMatches = methodRegex.Matches(classBody);

            foreach (Match methodMatch in methodMatches)
            {
                string returnType = methodMatch.Groups[1].Value;    // 반환 타입
                string methodName = methodMatch.Groups[2].Value;    // 메서드 이름
                string parameters = methodMatch.Groups[3].Value;    // 매개변수 리스트
                string methodBody = methodMatch.Groups[4].Value;    // 메서드 본문

                Console.WriteLine($"\tMethod: {methodName} ({parameters})");
                Console.WriteLine($"\tReturn type: {returnType}");
                Console.WriteLine($"\tMethod body: {methodBody}");
            }
        }
    }
}
