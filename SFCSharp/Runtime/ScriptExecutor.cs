using SFCSharp.Context;
using SFCSharp.Execution;
using SFCSharp.Execution.UnityExec;
using System;
using System.Collections.Generic;

namespace SFCSharp.Runtime
{
    /// <summary>
    /// SFCSharp 스크립트 실행 엔진
    /// 파싱된 명령어를 실제로 실행합니다.
    /// </summary>
    public class ScriptExecutor
    {
        private readonly SFContext _context;

        public ScriptExecutor(SFContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// 명령어를 실행합니다.
        /// </summary>
        /// <param name="command">실행할 명령어 (예: "GameObject.Create('Player')")</param>
        /// <returns>실행 결과</returns>
        public ExecutionResult Execute(string command)
        {
            try
            {
                // 1. 명령어 파싱
                ParsedCommand parsedCommand = CommandParser.Parse(command);

                // 2. 변수 참조 해석
                object[] resolvedArgs = ResolveArguments(parsedCommand.Arguments);

                // 3. 명령어 실행
                object result = null;

                SFExecManager.ExecWrapper(
                    parsedCommand.MethodPath,
                    (obj) => result = obj,
                    resolvedArgs
                );

                return new ExecutionResult
                {
                    Success = true,
                    Result = result,
                    Command = command
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult
                {
                    Success = false,
                    Error = ex,
                    Command = command,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 여러 명령어를 순차적으로 실행합니다.
        /// </summary>
        public List<ExecutionResult> ExecuteSequence(params string[] commands)
        {
            var results = new List<ExecutionResult>();

            foreach (string command in commands)
            {
                results.Add(Execute(command));
            }

            return results;
        }

        /// <summary>
        /// 인자 내의 변수 참조를 해석합니다.
        /// </summary>
        private object[] ResolveArguments(object[] args)
        {
            var resolved = new object[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is VariableReference varRef)
                {
                    resolved[i] = _context.GetVariable(varRef.VariableName);
                }
                else
                {
                    resolved[i] = args[i];
                }
            }

            return resolved;
        }

        /// <summary>
        /// 스크립트 컨텍스트에 변수를 저장합니다.
        /// </summary>
        public void SetVariable(string name, object value)
        {
            _context.SetVariable(name, value);
        }

        /// <summary>
        /// 스크립트 컨텍스트에서 변수를 조회합니다.
        /// </summary>
        public object GetVariable(string name)
        {
            return _context.GetVariable(name);
        }
    }

    /// <summary>
    /// 명령어 실행 결과
    /// </summary>
    public class ExecutionResult
    {
        /// <summary>
        /// 실행 성공 여부
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 실행된 명령어
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// 실행 결과 (성공 시)
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// 에러 메시지 (실패 시)
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 발생한 예외 (실패 시)
        /// </summary>
        public Exception Error { get; set; }

        public override string ToString()
        {
            if (Success)
            {
                return $"✓ {Command} → {Result?.ToString() ?? "null"}";
            }
            else
            {
                return $"✗ {Command} - Error: {ErrorMessage}";
            }
        }
    }
}
