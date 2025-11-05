using SFCSharp.Excution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Excution.UnityExec.UnityExecTransform
{
    /// <summary>
    /// UnityEngine.Transform 메서드 핸들러
    /// </summary>
    public class SFExecUT : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"Translate", new TranslateHandler() },
                {"Rotate", new RotateHandler() },
                {"LookAt", new LookAtHandler() },
                {"GetPosition", new GetPositionHandler() },
                {"SetPosition", new SetPositionHandler() },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        // Translate 메서드 핸들러
        private class TranslateHandler : IMethodHandler
        {
            public void Excute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 4)
                        throw new ArgumentException("Translate requires at least 4 arguments: transform, x, y, z");

                    if (!(args[0] is SFTransform transform))
                        throw new ArgumentException("First argument must be a Transform");

                    float x = Convert.ToSingle(args[1]);
                    float y = Convert.ToSingle(args[2]);
                    float z = Convert.ToSingle(args[3]);

                    transform.Translate(x, y, z);
                    execCallback?.Invoke(transform);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Transform.Translate error: {ex.Message}", ex));
                }
            }
        }

        // Rotate 메서드 핸들러
        private class RotateHandler : IMethodHandler
        {
            public void Excute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 4)
                        throw new ArgumentException("Rotate requires at least 4 arguments: transform, x, y, z");

                    if (!(args[0] is SFTransform transform))
                        throw new ArgumentException("First argument must be a Transform");

                    float x = Convert.ToSingle(args[1]);
                    float y = Convert.ToSingle(args[2]);
                    float z = Convert.ToSingle(args[3]);

                    transform.Rotate(x, y, z);
                    execCallback?.Invoke(transform);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Transform.Rotate error: {ex.Message}", ex));
                }
            }
        }

        // LookAt 메서드 핸들러
        private class LookAtHandler : IMethodHandler
        {
            public void Excute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("LookAt requires at least 2 arguments: transform, targetPosition");

                    if (!(args[0] is SFTransform transform))
                        throw new ArgumentException("First argument must be a Transform");

                    SFVector3 targetPos;
                    if (args[1] is SFVector3 vec)
                    {
                        targetPos = vec;
                    }
                    else
                    {
                        throw new ArgumentException("Second argument must be a Vector3");
                    }

                    transform.LookAt(targetPos);
                    execCallback?.Invoke(transform);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Transform.LookAt error: {ex.Message}", ex));
                }
            }
        }

        // GetPosition 메서드 핸들러
        private class GetPositionHandler : IMethodHandler
        {
            public void Excute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("GetPosition requires 1 argument: transform");

                    if (!(args[0] is SFTransform transform))
                        throw new ArgumentException("First argument must be a Transform");

                    execCallback?.Invoke(transform.position);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Transform.GetPosition error: {ex.Message}", ex));
                }
            }
        }

        // SetPosition 메서드 핸들러
        private class SetPositionHandler : IMethodHandler
        {
            public void Excute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 4)
                        throw new ArgumentException("SetPosition requires at least 4 arguments: transform, x, y, z");

                    if (!(args[0] is SFTransform transform))
                        throw new ArgumentException("First argument must be a Transform");

                    float x = Convert.ToSingle(args[1]);
                    float y = Convert.ToSingle(args[2]);
                    float z = Convert.ToSingle(args[3]);

                    transform.position = new SFVector3(x, y, z);
                    execCallback?.Invoke(transform);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Transform.SetPosition error: {ex.Message}", ex));
                }
            }
        }
    }
}
