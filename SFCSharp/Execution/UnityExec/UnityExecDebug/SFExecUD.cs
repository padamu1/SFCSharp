using SFCSharp.Execution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec.UnityExecDebug
{
    /// <summary>
    /// UnityEngine.Debug 메서드 핸들러
    /// </summary>
    public class SFExecUD : SFMethodHandlerBase
    {
        /// <summary>
        /// 로그 출력 시 호출되는 콜백
        /// Unity 환경에서는 Debug.Log로 리다이렉트할 수 있습니다.
        /// </summary>
        public static Action<string, LogType> OnLog { get; set; }

        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"Log", new LogHandler(LogType.Log) },
                {"LogWarning", new LogHandler(LogType.Warning) },
                {"LogError", new LogHandler(LogType.Error) },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        private class LogHandler : IMethodHandler
        {
            private readonly LogType _logType;

            public LogHandler(LogType logType)
            {
                _logType = logType;
            }

            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    string message = args.Length > 0 ? args[0]?.ToString() ?? "null" : "";

                    if (OnLog != null)
                    {
                        OnLog.Invoke(message, _logType);
                    }

                    execCallback?.Invoke(null);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Debug.{_logType} error: {ex.Message}", ex));
                }
            }
        }
    }

    public enum LogType
    {
        Log,
        Warning,
        Error
    }
}
