using System.Collections.Generic;
using SFCSharp.Excution.Base;
using SFCSharp.Excution.SystemExec.SystemConsole;

namespace SFCSharp.Excution.SystemExec
{
    public class SystemExecHandler : SFNamespaceHandlerBase
    {
        public override void InitExecHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = new Dictionary<string, INamespaceHandler>()
            {
                {"Console", new SystemConsoleExecHandler() },
            };
        }
    }
}
