using SFCSharp.Execution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec.UnityExecMathf
{
    /// <summary>
    /// UnityEngine.Mathf 메서드 핸들러
    /// </summary>
    public class SFExecUM : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"Abs", new AbsHandler() },
                {"Clamp", new ClampHandler() },
                {"Lerp", new LerpHandler() },
                {"Sin", new TrigHandler(Math.Sin) },
                {"Cos", new TrigHandler(Math.Cos) },
                {"Tan", new TrigHandler(Math.Tan) },
                {"Sqrt", new SingleArgHandler(v => (float)Math.Sqrt(v)) },
                {"Floor", new SingleArgHandler(v => (float)Math.Floor(v)) },
                {"Ceil", new SingleArgHandler(v => (float)Math.Ceiling(v)) },
                {"Round", new SingleArgHandler(v => (float)Math.Round(v)) },
                {"Min", new TwoArgHandler(Math.Min) },
                {"Max", new TwoArgHandler(Math.Max) },
                {"Pow", new TwoArgHandler((a, b) => (float)Math.Pow(a, b)) },
                {"Atan2", new TwoArgHandler((a, b) => (float)Math.Atan2(a, b)) },
                {"PI", new ConstantHandler((float)Math.PI) },
                {"Deg2Rad", new ConstantHandler((float)(Math.PI / 180.0)) },
                {"Rad2Deg", new ConstantHandler((float)(180.0 / Math.PI)) },
                {"Infinity", new ConstantHandler(float.PositiveInfinity) },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        private class AbsHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("Abs requires 1 argument");
                    float value = Convert.ToSingle(args[0]);
                    execCallback?.Invoke(Math.Abs(value));
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Mathf.Abs error: {ex.Message}", ex));
                }
            }
        }

        private class ClampHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 3)
                        throw new ArgumentException("Clamp requires 3 arguments: value, min, max");
                    float value = Convert.ToSingle(args[0]);
                    float min = Convert.ToSingle(args[1]);
                    float max = Convert.ToSingle(args[2]);
                    execCallback?.Invoke(Math.Max(min, Math.Min(max, value)));
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Mathf.Clamp error: {ex.Message}", ex));
                }
            }
        }

        private class LerpHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 3)
                        throw new ArgumentException("Lerp requires 3 arguments: a, b, t");
                    float a = Convert.ToSingle(args[0]);
                    float b = Convert.ToSingle(args[1]);
                    float t = Convert.ToSingle(args[2]);
                    t = Math.Max(0f, Math.Min(1f, t));
                    execCallback?.Invoke(a + (b - a) * t);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Mathf.Lerp error: {ex.Message}", ex));
                }
            }
        }

        private class TrigHandler : IMethodHandler
        {
            private readonly Func<double, double> _func;

            public TrigHandler(Func<double, double> func)
            {
                _func = func;
            }

            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("Requires 1 argument");
                    float value = Convert.ToSingle(args[0]);
                    execCallback?.Invoke((float)_func(value));
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Mathf error: {ex.Message}", ex));
                }
            }
        }

        private class SingleArgHandler : IMethodHandler
        {
            private readonly Func<float, float> _func;

            public SingleArgHandler(Func<float, float> func)
            {
                _func = func;
            }

            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("Requires 1 argument");
                    float value = Convert.ToSingle(args[0]);
                    execCallback?.Invoke(_func(value));
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Mathf error: {ex.Message}", ex));
                }
            }
        }

        private class TwoArgHandler : IMethodHandler
        {
            private readonly Func<float, float, float> _func;

            public TwoArgHandler(Func<float, float, float> func)
            {
                _func = func;
            }

            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("Requires 2 arguments");
                    float a = Convert.ToSingle(args[0]);
                    float b = Convert.ToSingle(args[1]);
                    execCallback?.Invoke(_func(a, b));
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Mathf error: {ex.Message}", ex));
                }
            }
        }

        private class ConstantHandler : IMethodHandler
        {
            private readonly float _value;

            public ConstantHandler(float value)
            {
                _value = value;
            }

            public void Execute(Action<object> execCallback, params object[] args)
            {
                execCallback?.Invoke(_value);
            }
        }
    }
}
