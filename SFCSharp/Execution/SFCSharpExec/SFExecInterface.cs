using SFCSharp.Execution.Base;
using SFCSharp.TypeSystem;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.SFCSharpExec
{
    /// <summary>
    /// SFCSharp.Interface 메서드 핸들러
    /// 인터페이스 정의 및 조회 기능을 제공합니다.
    /// </summary>
    public class SFExecInterface : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"Define", new DefineHandler()},
                {"IsDefined", new IsDefinedHandler()},
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler> _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        /// <summary>
        /// 인터페이스를 정의합니다.
        /// 사용법: SFCSharp.Interface.Define('IMovable')
        /// 사용법: SFCSharp.Interface.Define('IColorMovable', 'IMovable', 'IColorable')
        /// 첫 번째 인자: 인터페이스 이름
        /// 나머지 인자: 부모 인터페이스 이름들 (선택)
        /// </summary>
        private class DefineHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("Interface.Define requires at least 1 argument: interfaceName");

                    string interfaceName = args[0].ToString();

                    var parentInterfaces = new List<string>();
                    for (int i = 1; i < args.Length; i++)
                    {
                        parentInterfaces.Add(args[i].ToString());
                    }

                    var info = new SFInterfaceInfo(interfaceName, parentInterfaces);
                    SFTypeRegistry.Instance.RegisterInterface(info);

                    execCallback?.Invoke(true);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Interface.Define error: {ex.Message}", ex));
                }
            }
        }

        /// <summary>
        /// 인터페이스가 정의되어 있는지 확인합니다.
        /// 사용법: SFCSharp.Interface.IsDefined('IMovable')
        /// </summary>
        private class IsDefinedHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("Interface.IsDefined requires 1 argument: interfaceName");

                    string interfaceName = args[0].ToString();
                    bool result = SFTypeRegistry.Instance.HasInterface(interfaceName);

                    execCallback?.Invoke(result);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Interface.IsDefined error: {ex.Message}", ex));
                }
            }
        }
    }
}
