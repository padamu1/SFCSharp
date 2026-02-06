using SFCSharp.Execution.Base;
using SFCSharp.Execution.SystemExec.SystemExecConsole;
using System.Collections.Generic;

namespace SFCSharp.Execution.SystemExec
{
    /// <summary>
    /// System 네임스페이스 핸들러
    /// </summary>
    public class SystemExecHandler : SFNamespaceHandlerBase
    {
        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = new Dictionary<string, INamespaceHandler>()
            {
                {"Console", new SFExecSC() },
            };
        }
    }
}
