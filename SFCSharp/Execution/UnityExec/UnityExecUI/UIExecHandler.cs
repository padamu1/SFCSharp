using SFCSharp.Execution.Base;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec.UnityExecUI
{
    /// <summary>
    /// UnityEngine.UI 네임스페이스 핸들러
    /// Text, Image 등 UI 컴포넌트를 관리합니다.
    /// </summary>
    public class UIExecHandler : SFNamespaceHandlerBase
    {
        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = new Dictionary<string, INamespaceHandler>()
            {
                {"Text", new SFExecUText() },
                {"Image", new SFExecUImage() },
            };
        }
    }
}
