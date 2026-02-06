using SFCSharp.Execution.Base;
using SFCSharp.Execution.UnityExec;
using System.Collections.Generic;

namespace SFCSharp.Execution
{
    /// <summary>
    /// 최상위 네임스페이스 핸들러
    /// 지원하는 네임스페이스를 라우팅합니다.
    /// </summary>
    public class SFExecHandler : SFNamespaceHandlerBase
    {
        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = new Dictionary<string, INamespaceHandler>()
            {
                {"UnityEngine", new UnityExecHandler() },
            };
        }
    }
}
