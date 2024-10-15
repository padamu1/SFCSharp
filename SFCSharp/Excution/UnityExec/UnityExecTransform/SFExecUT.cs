using SFCSharp.Excution.Base;
using System.Collections.Generic;

namespace SFCSharp.Excution.UnityExec.UnityExecTransform
{
    public class SFExecUT : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {

            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = new Dictionary<string, INamespaceHandler>()
            {

            };
        }
    }
}
