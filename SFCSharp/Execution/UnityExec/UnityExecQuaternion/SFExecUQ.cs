using SFCSharp.Execution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec.UnityExecQuaternion
{
    /// <summary>
    /// UnityEngine.Quaternion 메서드 핸들러
    /// </summary>
    public class SFExecUQ : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"Euler", new EulerHandler() },
                {"identity", new IdentityHandler() },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        private class EulerHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    float x = args.Length > 0 ? Convert.ToSingle(args[0]) : 0;
                    float y = args.Length > 1 ? Convert.ToSingle(args[1]) : 0;
                    float z = args.Length > 2 ? Convert.ToSingle(args[2]) : 0;
                    execCallback?.Invoke(SFQuaternion.Euler(x, y, z));
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Quaternion.Euler error: {ex.Message}", ex));
                }
            }
        }

        private class IdentityHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                execCallback?.Invoke(SFQuaternion.identity);
            }
        }
    }
}
