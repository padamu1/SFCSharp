using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFCSharp.Runtime
{
    /// <summary>
    /// 텍스트 형식의 스크립트 명령어를 파싱하는 클래스
    /// 예: "GameObject.Create('Player')" → 메서드명 + 인자 추출
    /// </summary>
    public class CommandParser
    {
        /// <summary>
        /// 명령어를 분석하여 메서드명과 인자로 구분합니다.
        /// </summary>
        /// <param name="command">파싱할 명령어 (예: "GameObject.Create('Player')")</param>
        /// <returns>파싱된 명령어 정보</returns>
        public static ParsedCommand Parse(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Command cannot be null or empty");

            command = command.Trim();

            // 메서드명과 인자 분리
            // 패턴: methodName(arg1, arg2, ...)
            // 메서드명에 공백이 있을 수 있지만 점(.)으로만 구분되어야 함
            var match = Regex.Match(command, @"^([\w\s\.]+?)\s*\((.*)\)$");

            if (!match.Success)
                throw new InvalidOperationException($"Invalid command format: {command}");

            string methodPath = Regex.Replace(match.Groups[1].Value, @"\s+", "").Trim();
            string argsString = match.Groups[2].Value;

            string[] methodNames = methodPath.Split('.');
            object[] args = ParseArguments(argsString);

            return new ParsedCommand
            {
                MethodPath = methodPath,
                MethodNames = methodNames,
                Arguments = args
            };
        }

        /// <summary>
        /// 메서드 인자 문자열을 파싱합니다.
        /// </summary>
        private static object[] ParseArguments(string argsString)
        {
            if (string.IsNullOrWhiteSpace(argsString))
                return Array.Empty<object>();

            var arguments = new List<object>();
            var argParts = SplitArguments(argsString);

            foreach (string arg in argParts)
            {
                arguments.Add(ParseValue(arg.Trim()));
            }

            return arguments.ToArray();
        }

        /// <summary>
        /// 인자 문자열을 쉼표로 분리합니다. (중괄호 내 쉼표는 무시)
        /// </summary>
        private static List<string> SplitArguments(string args)
        {
            var result = new List<string>();
            var current = new System.Text.StringBuilder();
            int parenLevel = 0;
            bool inString = false;
            char stringChar = '\0';

            foreach (char c in args)
            {
                if ((c == '"' || c == '\'') && (current.Length == 0 || current[current.Length - 1] != '\\'))
                {
                    if (!inString)
                    {
                        inString = true;
                        stringChar = c;
                    }
                    else if (c == stringChar)
                    {
                        inString = false;
                    }
                }

                if (!inString && c == '(')
                    parenLevel++;
                else if (!inString && c == ')')
                    parenLevel--;
                else if (!inString && c == ',' && parenLevel == 0)
                {
                    result.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(c);
            }

            if (current.Length > 0)
                result.Add(current.ToString());

            return result;
        }

        /// <summary>
        /// 값을 적절한 타입으로 변환합니다.
        /// </summary>
        private static object ParseValue(string value)
        {
            // Null 체크
            if (value.Equals("null", StringComparison.OrdinalIgnoreCase))
                return null;

            // 문자열
            if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                (value.StartsWith("'") && value.EndsWith("'")))
            {
                return value.Substring(1, value.Length - 2);
            }

            // Boolean
            if (bool.TryParse(value, out bool boolValue))
                return boolValue;

            // Integer (먼저 체크해야 float로 변환되지 않음)
            if (int.TryParse(value, out int intValue))
                return intValue;

            // Float
            if (float.TryParse(value, out float floatValue))
                return floatValue;

            // Double
            if (double.TryParse(value, out double doubleValue))
                return doubleValue;

            // Vector3 생성: Vector3(x, y, z) 형식
            if (value.StartsWith("Vector3(") && value.EndsWith(")"))
            {
                string innerContent = value.Substring(8, value.Length - 9);
                var parts = SplitArguments(innerContent);
                if (parts.Count == 3 &&
                    float.TryParse(parts[0], out float x) &&
                    float.TryParse(parts[1], out float y) &&
                    float.TryParse(parts[2], out float z))
                {
                    return new Execution.UnityExec.SFVector3(x, y, z);
                }
            }

            // GameObject 참조: $variableName
            if (value.StartsWith("$"))
            {
                return new VariableReference(value.Substring(1));
            }

            throw new InvalidOperationException($"Cannot parse value: {value}");
        }
    }

    /// <summary>
    /// 파싱된 명령어 정보
    /// </summary>
    public class ParsedCommand
    {
        /// <summary>
        /// 전체 메서드 경로 (예: "GameObject.Create")
        /// </summary>
        public string MethodPath { get; set; }

        /// <summary>
        /// 메서드명을 네임스페이스별로 분할 (예: ["GameObject", "Create"])
        /// </summary>
        public string[] MethodNames { get; set; }

        /// <summary>
        /// 메서드 인자 배열
        /// </summary>
        public object[] Arguments { get; set; }

        public override string ToString()
        {
            string argsStr = Arguments.Length > 0
                ? string.Join(", ", System.Array.ConvertAll(Arguments, a => a?.ToString() ?? "null"))
                : "";
            return $"{MethodPath}({argsStr})";
        }
    }

    /// <summary>
    /// 변수 참조 (예: $myVariable)
    /// </summary>
    public class VariableReference
    {
        public string VariableName { get; }

        public VariableReference(string variableName)
        {
            VariableName = variableName;
        }

        public override string ToString()
        {
            return $"${VariableName}";
        }
    }
}
