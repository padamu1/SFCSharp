using SFCSharp.Execution.Base;
using SFCSharp.Execution.UnityExec.UnityExecDebug;
using SFCSharp.Execution.UnityExec.UnityExecGameObject;
using SFCSharp.Execution.UnityExec.UnityExecMathf;
using SFCSharp.Execution.UnityExec.UnityExecQuaternion;
using SFCSharp.Execution.UnityExec.UnityExecTime;
using SFCSharp.Execution.UnityExec.UnityExecTransform;
using SFCSharp.Execution.UnityExec.UnityExecVector3;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec
{
    /// <summary>
    /// UnityEngine 네임스페이스 핸들러
    /// GameObject, Transform, Vector3, Quaternion, Debug, Mathf, Time 등을 관리합니다.
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
                {"Quaternion", new SFExecUQ() },
                {"Debug", new SFExecUD() },
                {"Mathf", new SFExecUM() },
                {"Time", new SFExecUTime() },
            };
        }
    }
}
