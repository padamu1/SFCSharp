using SFCSharp.Execution.Base;
using System.Collections.Generic;

namespace SFCSharp.Execution.SFCSharpExec
{
    /// <summary>
    /// SFCSharp 네임스페이스 핸들러
    /// Type과 Interface 서브 네임스페이스를 라우팅합니다.
    /// </summary>
    public class SFCSharpExecHandler : SFNamespaceHandlerBase
    {
        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler> _namespaceHandlerDic)
        {
            _namespaceHandlerDic = new Dictionary<string, INamespaceHandler>()
            {
                {"Type", new SFExecType()},
                {"Interface", new SFExecInterface()},
            };
        }
    }
}
