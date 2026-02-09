using SFCSharp.Execution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec.UnityExecGameObject
{
    /// <summary>
    /// UnityEngine.GameObject 메서드 핸들러
    /// </summary>
    public class SFExecUG : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"Create", new CreateHandler() },
                {"GetName", new GetNameHandler() },
                {"SetName", new SetNameHandler() },
                {"SetActive", new SetActiveHandler() },
                {"GetActive", new GetActiveHandler() },
                {"GetTransform", new GetTransformHandler() },
                {"AddComponent", new AddComponentHandler() },
                {"GetComponent", new GetComponentHandler() },
                {"GetComponents", new GetComponentsHandler() },
                {"HasComponent", new HasComponentHandler() },
                {"RemoveComponent", new RemoveComponentHandler() },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        // Create 메서드 핸들러 (새로운 GameObject 생성)
        private class CreateHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    string name = args.Length > 0 ? args[0].ToString() ?? "GameObject" : "GameObject";
                    SFGameObject gameObject = new SFGameObject(name);
                    execCallback?.Invoke(gameObject);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"GameObject.Create error: {ex.Message}", ex));
                }
            }
        }

        // GetName 메서드 핸들러
        private class GetNameHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("GetName requires 1 argument: gameObject");

                    if (!(args[0] is SFGameObject gameObject))
                        throw new ArgumentException("First argument must be a GameObject");

                    execCallback?.Invoke(gameObject.name);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"GameObject.GetName error: {ex.Message}", ex));
                }
            }
        }

        // SetName 메서드 핸들러
        private class SetNameHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("SetName requires 2 arguments: gameObject, name");

                    if (!(args[0] is SFGameObject gameObject))
                        throw new ArgumentException("First argument must be a GameObject");

                    gameObject.name = args[1].ToString() ?? "GameObject";
                    execCallback?.Invoke(gameObject);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"GameObject.SetName error: {ex.Message}", ex));
                }
            }
        }

        // SetActive 메서드 핸들러
        private class SetActiveHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("SetActive requires 2 arguments: gameObject, active");

                    if (!(args[0] is SFGameObject gameObject))
                        throw new ArgumentException("First argument must be a GameObject");

                    bool active = Convert.ToBoolean(args[1]);
                    gameObject.SetActive(active);
                    execCallback?.Invoke(gameObject);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"GameObject.SetActive error: {ex.Message}", ex));
                }
            }
        }

        // GetActive 메서드 핸들러
        private class GetActiveHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("GetActive requires 1 argument: gameObject");

                    if (!(args[0] is SFGameObject gameObject))
                        throw new ArgumentException("First argument must be a GameObject");

                    execCallback?.Invoke(gameObject.activeSelf);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"GameObject.GetActive error: {ex.Message}", ex));
                }
            }
        }

        // GetTransform 메서드 핸들러
        private class GetTransformHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("GetTransform requires 1 argument: gameObject");

                    if (!(args[0] is SFGameObject gameObject))
                        throw new ArgumentException("First argument must be a GameObject");

                    execCallback?.Invoke(gameObject.transform);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"GameObject.GetTransform error: {ex.Message}", ex));
                }
            }
        }

        // AddComponent 메서드 핸들러
        private class AddComponentHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("AddComponent requires 2 arguments: gameObject, componentType");

                    if (!(args[0] is SFGameObject gameObject))
                        throw new ArgumentException("First argument must be a GameObject");

                    string typeName = args[1].ToString();
                    var component = gameObject.AddComponent(typeName);
                    execCallback?.Invoke(component);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"GameObject.AddComponent error: {ex.Message}", ex));
                }
            }
        }

        // GetComponent 메서드 핸들러
        private class GetComponentHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("GetComponent requires 2 arguments: gameObject, componentType");

                    if (!(args[0] is SFGameObject gameObject))
                        throw new ArgumentException("First argument must be a GameObject");

                    string typeName = args[1].ToString();
                    var component = gameObject.GetComponent(typeName);
                    execCallback?.Invoke(component);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"GameObject.GetComponent error: {ex.Message}", ex));
                }
            }
        }

        // GetComponents 메서드 핸들러 (인터페이스/상속 기반 다중 조회)
        private class GetComponentsHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("GetComponents requires 2 arguments: gameObject, componentType");

                    if (!(args[0] is SFGameObject gameObject))
                        throw new ArgumentException("First argument must be a GameObject");

                    string typeName = args[1].ToString();
                    var components = gameObject.GetComponents(typeName);
                    execCallback?.Invoke(components);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"GameObject.GetComponents error: {ex.Message}", ex));
                }
            }
        }

        // HasComponent 메서드 핸들러
        private class HasComponentHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("HasComponent requires 2 arguments: gameObject, componentType");

                    if (!(args[0] is SFGameObject gameObject))
                        throw new ArgumentException("First argument must be a GameObject");

                    string typeName = args[1].ToString();
                    bool result = gameObject.HasComponent(typeName);
                    execCallback?.Invoke(result);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"GameObject.HasComponent error: {ex.Message}", ex));
                }
            }
        }

        // RemoveComponent 메서드 핸들러
        private class RemoveComponentHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("RemoveComponent requires 2 arguments: gameObject, componentType");

                    if (!(args[0] is SFGameObject gameObject))
                        throw new ArgumentException("First argument must be a GameObject");

                    string typeName = args[1].ToString();
                    bool result = gameObject.RemoveComponent(typeName);
                    execCallback?.Invoke(result);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"GameObject.RemoveComponent error: {ex.Message}", ex));
                }
            }
        }
    }
}
