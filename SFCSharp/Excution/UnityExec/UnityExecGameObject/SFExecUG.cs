using SFCSharp.Excution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Excution.UnityExec.UnityExecGameObject
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
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        // Create 메서드 핸들러 (새로운 GameObject 생성)
        private class CreateHandler : IMethodHandler
        {
            public void Excute(Action<object> execCallback, params object[] args)
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
            public void Excute(Action<object> execCallback, params object[] args)
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
            public void Excute(Action<object> execCallback, params object[] args)
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
            public void Excute(Action<object> execCallback, params object[] args)
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
            public void Excute(Action<object> execCallback, params object[] args)
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
            public void Excute(Action<object> execCallback, params object[] args)
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
    }
}
