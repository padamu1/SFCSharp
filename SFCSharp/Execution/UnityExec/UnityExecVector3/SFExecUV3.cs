using SFCSharp.Execution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec.UnityExecVector3
{
    /// <summary>
    /// UnityEngine.Vector3 메서드 핸들러
    /// </summary>
    public class SFExecUV3 : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"Create", new CreateHandler() },
                {"Zero", new ZeroHandler() },
                {"One", new OneHandler() },
                {"Distance", new DistanceHandler() },
                {"Magnitude", new MagnitudeHandler() },
                {"Normalized", new NormalizedHandler() },
                {"Dot", new DotHandler() },
                {"Cross", new CrossHandler() },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        // Create 메서드 핸들러 (새로운 Vector3 생성)
        private class CreateHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    float x = args.Length > 0 ? Convert.ToSingle(args[0]) : 0;
                    float y = args.Length > 1 ? Convert.ToSingle(args[1]) : 0;
                    float z = args.Length > 2 ? Convert.ToSingle(args[2]) : 0;

                    SFVector3 vector = new SFVector3(x, y, z);
                    execCallback?.Invoke(vector);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Vector3.Create error: {ex.Message}", ex));
                }
            }
        }

        // Zero 메서드 핸들러
        private class ZeroHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    execCallback?.Invoke(SFVector3.zero);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Vector3.Zero error: {ex.Message}", ex));
                }
            }
        }

        // One 메서드 핸들러
        private class OneHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    execCallback?.Invoke(SFVector3.one);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Vector3.One error: {ex.Message}", ex));
                }
            }
        }

        // Distance 메서드 핸들러
        private class DistanceHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("Distance requires 2 arguments: vectorA, vectorB");

                    if (!(args[0] is SFVector3 vecA))
                        throw new ArgumentException("First argument must be a Vector3");

                    if (!(args[1] is SFVector3 vecB))
                        throw new ArgumentException("Second argument must be a Vector3");

                    float distance = SFVector3.Distance(vecA, vecB);
                    execCallback?.Invoke(distance);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Vector3.Distance error: {ex.Message}", ex));
                }
            }
        }

        // Magnitude 메서드 핸들러
        private class MagnitudeHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("Magnitude requires 1 argument: vector");

                    if (!(args[0] is SFVector3 vector))
                        throw new ArgumentException("First argument must be a Vector3");

                    execCallback?.Invoke(vector.magnitude);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Vector3.Magnitude error: {ex.Message}", ex));
                }
            }
        }

        // Normalized 메서드 핸들러
        private class NormalizedHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("Normalized requires 1 argument: vector");

                    if (!(args[0] is SFVector3 vector))
                        throw new ArgumentException("First argument must be a Vector3");

                    execCallback?.Invoke(vector.normalized);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Vector3.Normalized error: {ex.Message}", ex));
                }
            }
        }

        // Dot 메서드 핸들러
        private class DotHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("Dot requires 2 arguments: vectorA, vectorB");

                    if (!(args[0] is SFVector3 vecA))
                        throw new ArgumentException("First argument must be a Vector3");

                    if (!(args[1] is SFVector3 vecB))
                        throw new ArgumentException("Second argument must be a Vector3");

                    float dot = SFVector3.Dot(vecA, vecB);
                    execCallback?.Invoke(dot);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Vector3.Dot error: {ex.Message}", ex));
                }
            }
        }

        // Cross 메서드 핸들러
        private class CrossHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("Cross requires 2 arguments: vectorA, vectorB");

                    if (!(args[0] is SFVector3 vecA))
                        throw new ArgumentException("First argument must be a Vector3");

                    if (!(args[1] is SFVector3 vecB))
                        throw new ArgumentException("Second argument must be a Vector3");

                    SFVector3 cross = SFVector3.Cross(vecA, vecB);
                    execCallback?.Invoke(cross);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Vector3.Cross error: {ex.Message}", ex));
                }
            }
        }
    }
}
