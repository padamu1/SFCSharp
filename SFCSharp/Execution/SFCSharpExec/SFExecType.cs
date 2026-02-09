using SFCSharp.Execution.Base;
using SFCSharp.Execution.UnityExec;
using SFCSharp.TypeSystem;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.SFCSharpExec
{
    /// <summary>
    /// SFCSharp.Type 메서드 핸들러
    /// 타입 정의, 타입 체크, 프로퍼티 접근 기능을 제공합니다.
    /// </summary>
    public class SFExecType : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"Define", new DefineHandler()},
                {"DefineProperty", new DefinePropertyHandler()},
                {"Implement", new ImplementHandler()},
                {"Is", new IsHandler()},
                {"IsSubclassOf", new IsSubclassOfHandler()},
                {"ImplementsInterface", new ImplementsInterfaceHandler()},
                {"GetTypeName", new GetTypeNameHandler()},
                {"SetProperty", new SetPropertyHandler()},
                {"GetProperty", new GetPropertyHandler()},
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler> _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        /// <summary>
        /// 커스텀 타입을 정의합니다.
        /// 사용법: SFCSharp.Type.Define('Enemy', 'Component', 'IMovable', 'IDamageable')
        /// 첫 번째 인자: 타입 이름
        /// 두 번째 인자: 부모 타입 이름
        /// 나머지 인자: 구현할 인터페이스 이름들 (선택)
        /// </summary>
        private class DefineHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("Type.Define requires at least 2 arguments: typeName, baseTypeName");

                    string typeName = args[0].ToString();
                    string baseTypeName = args[1].ToString();

                    var interfaces = new List<string>();
                    for (int i = 2; i < args.Length; i++)
                    {
                        interfaces.Add(args[i].ToString());
                    }

                    var info = new SFTypeInfo(typeName, baseTypeName, interfaces);
                    SFTypeRegistry.Instance.RegisterType(info);

                    execCallback?.Invoke(true);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Type.Define error: {ex.Message}", ex));
                }
            }
        }

        /// <summary>
        /// 타입에 기본 프로퍼티를 정의합니다.
        /// 사용법: SFCSharp.Type.DefineProperty('Enemy', 'health', 100)
        /// </summary>
        private class DefinePropertyHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 3)
                        throw new ArgumentException("Type.DefineProperty requires 3 arguments: typeName, propertyName, defaultValue");

                    string typeName = args[0].ToString();
                    string propertyName = args[1].ToString();
                    object defaultValue = args[2];

                    SFTypeRegistry.Instance.DefineProperty(typeName, propertyName, defaultValue);

                    execCallback?.Invoke(true);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Type.DefineProperty error: {ex.Message}", ex));
                }
            }
        }

        /// <summary>
        /// 기존 타입에 인터페이스를 추가합니다.
        /// 사용법: SFCSharp.Type.Implement('Text', 'IUIElement')
        /// </summary>
        private class ImplementHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("Type.Implement requires 2 arguments: typeName, interfaceName");

                    string typeName = args[0].ToString();
                    string interfaceName = args[1].ToString();

                    SFTypeRegistry.Instance.AddInterfaceToType(typeName, interfaceName);

                    execCallback?.Invoke(true);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Type.Implement error: {ex.Message}", ex));
                }
            }
        }

        /// <summary>
        /// 컴포넌트가 특정 타입 또는 인터페이스에 할당 가능한지 확인합니다.
        /// 사용법: SFCSharp.Type.Is($comp, 'IMovable')
        /// </summary>
        private class IsHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("Type.Is requires 2 arguments: component, typeOrInterfaceName");

                    if (!(args[0] is SFComponent component))
                        throw new ArgumentException("First argument must be a Component");

                    string targetName = args[1].ToString();
                    bool result = SFTypeRegistry.Instance.IsAssignableTo(
                        component.ComponentTypeName, targetName);

                    execCallback?.Invoke(result);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Type.Is error: {ex.Message}", ex));
                }
            }
        }

        /// <summary>
        /// 타입의 상속 관계를 확인합니다.
        /// 사용법: SFCSharp.Type.IsSubclassOf('Enemy', 'Component')
        /// </summary>
        private class IsSubclassOfHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("Type.IsSubclassOf requires 2 arguments: derivedType, baseType");

                    string derivedType = args[0].ToString();
                    string baseType = args[1].ToString();

                    bool result = SFTypeRegistry.Instance.IsSubclassOf(derivedType, baseType);

                    execCallback?.Invoke(result);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Type.IsSubclassOf error: {ex.Message}", ex));
                }
            }
        }

        /// <summary>
        /// 타입이 인터페이스를 구현하는지 확인합니다.
        /// 사용법: SFCSharp.Type.ImplementsInterface('Enemy', 'IMovable')
        /// </summary>
        private class ImplementsInterfaceHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("Type.ImplementsInterface requires 2 arguments: typeName, interfaceName");

                    string typeName = args[0].ToString();
                    string interfaceName = args[1].ToString();

                    bool result = SFTypeRegistry.Instance.ImplementsInterface(typeName, interfaceName);

                    execCallback?.Invoke(result);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Type.ImplementsInterface error: {ex.Message}", ex));
                }
            }
        }

        /// <summary>
        /// 컴포넌트의 타입 이름을 반환합니다.
        /// 사용법: SFCSharp.Type.GetTypeName($comp)
        /// </summary>
        private class GetTypeNameHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("Type.GetTypeName requires 1 argument: component");

                    if (!(args[0] is SFComponent component))
                        throw new ArgumentException("First argument must be a Component");

                    execCallback?.Invoke(component.ComponentTypeName);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Type.GetTypeName error: {ex.Message}", ex));
                }
            }
        }

        /// <summary>
        /// 커스텀 컴포넌트의 프로퍼티 값을 설정합니다.
        /// 사용법: SFCSharp.Type.SetProperty($comp, 'health', 80)
        /// </summary>
        private class SetPropertyHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 3)
                        throw new ArgumentException("Type.SetProperty requires 3 arguments: component, propertyName, value");

                    if (!(args[0] is SFCustomComponent component))
                        throw new ArgumentException("First argument must be a CustomComponent");

                    string propertyName = args[1].ToString();
                    object value = args[2];

                    component.SetProperty(propertyName, value);

                    execCallback?.Invoke(component);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Type.SetProperty error: {ex.Message}", ex));
                }
            }
        }

        /// <summary>
        /// 커스텀 컴포넌트의 프로퍼티 값을 조회합니다.
        /// 사용법: SFCSharp.Type.GetProperty($comp, 'health')
        /// </summary>
        private class GetPropertyHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("Type.GetProperty requires 2 arguments: component, propertyName");

                    if (!(args[0] is SFCustomComponent component))
                        throw new ArgumentException("First argument must be a CustomComponent");

                    string propertyName = args[1].ToString();
                    object value = component.GetProperty(propertyName);

                    execCallback?.Invoke(value);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Type.GetProperty error: {ex.Message}", ex));
                }
            }
        }
    }
}
