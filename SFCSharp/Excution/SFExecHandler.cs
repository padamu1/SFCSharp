using SFCSharp.Excution.Base;
using SFCSharp.Excution.SystemExec;
using SFCSharp.Utils;
using System.Collections.Generic;

namespace SFCSharp.Excution
{
    public class SFExecHandler : SFNamespaceHandlerBase
    {
        public override void InitExecHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = new Dictionary<string, INamespaceHandler>()
            {
                {"System", new SystemExecHandler() },
            };
        }
    }
}
