using SFCSharp.Excution.Base;
using SFCSharp.Excution.UnityExec.UnityExecTransform;
using System;
using System.Collections.Generic;

namespace SFCSharp.Excution.UnityExec
{
    public class UnityExecHandler : SFNamespaceHandlerBase
    {
        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = new Dictionary<string, INamespaceHandler>()
            {
                {"Transform", new SFExecUT() },
            };
        }
    }
}
