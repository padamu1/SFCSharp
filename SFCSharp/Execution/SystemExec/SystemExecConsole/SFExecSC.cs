using SFCSharp.Execution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.SystemExec.SystemExecConsole
{
    /// <summary>
    /// System.Console 메서드 핸들러
    /// </summary>
    public class SFExecSC : SFMethodHandlerBase
    {
        /// <summary>
        /// 출력 리다이렉트 콜백
        /// Unity 환경에서는 Debug.Log로 리다이렉트할 수 있습니다.
        /// </summary>
        public static Action<string> OnWrite { get; set; }

        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"WriteLine", new WriteLineHandler() },
                {"Write", new WriteHandler() },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        private class WriteLineHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    string message = args.Length > 0 ? args[0]?.ToString() ?? "" : "";

                    if (OnWrite != null)
                        OnWrite.Invoke(message);
                    else
                        Console.WriteLine(message);

                    execCallback?.Invoke(null);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Console.WriteLine error: {ex.Message}", ex));
                }
            }
        }

        private class WriteHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    string message = args.Length > 0 ? args[0]?.ToString() ?? "" : "";

                    if (OnWrite != null)
                        OnWrite.Invoke(message);
                    else
                        Console.Write(message);

                    execCallback?.Invoke(null);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Console.Write error: {ex.Message}", ex));
                }
            }
        }
    }
}
