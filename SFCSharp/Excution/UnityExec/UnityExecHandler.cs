using SFCSharp.Excution.Base;
using SFCSharp.Excution.UnityExec.UnityExecGameObject;
using SFCSharp.Excution.UnityExec.UnityExecTransform;
using SFCSharp.Excution.UnityExec.UnityExecVector3;
using System;
using System.Collections.Generic;

namespace SFCSharp.Excution.UnityExec
{
    /// <summary>
    /// UnityEngine 네임스페이스 핸들러
    /// GameObject, Transform, Vector3, Quaternion 등을 관리합니다.
    /// </summary>
    public class UnityExecHandler : SFNamespaceHandlerBase
    {
        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = new Dictionary<string, INamespaceHandler>()
            {
                {"GameObject", new SFExecUG() },
                {"Transform", new SFExecUT() },
                {"Vector3", new SFExecUV3() },
            };
        }
    }
}
