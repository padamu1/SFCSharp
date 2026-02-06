using SFCSharp.Execution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec.UnityExecColor
{
    /// <summary>
    /// UnityEngine.Color 메서드 핸들러
    /// </summary>
    public class SFExecUColor : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"Create", new CreateHandler() },
                {"red", new PresetHandler(SFColor.red) },
                {"green", new PresetHandler(SFColor.green) },
                {"blue", new PresetHandler(SFColor.blue) },
                {"white", new PresetHandler(SFColor.white) },
                {"black", new PresetHandler(SFColor.black) },
                {"yellow", new PresetHandler(SFColor.yellow) },
                {"cyan", new PresetHandler(SFColor.cyan) },
                {"magenta", new PresetHandler(SFColor.magenta) },
                {"gray", new PresetHandler(SFColor.gray) },
                {"clear", new PresetHandler(SFColor.clear) },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        private class CreateHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    float r = args.Length > 0 ? Convert.ToSingle(args[0]) : 0;
                    float g = args.Length > 1 ? Convert.ToSingle(args[1]) : 0;
                    float b = args.Length > 2 ? Convert.ToSingle(args[2]) : 0;
                    float a = args.Length > 3 ? Convert.ToSingle(args[3]) : 1;
                    execCallback?.Invoke(new SFColor(r, g, b, a));
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Color.Create error: {ex.Message}", ex));
                }
            }
        }

        private class PresetHandler : IMethodHandler
        {
            private readonly SFColor _color;

            public PresetHandler(SFColor color)
            {
                _color = color;
            }

            public void Execute(Action<object> execCallback, params object[] args)
            {
                execCallback?.Invoke(new SFColor(_color.r, _color.g, _color.b, _color.a));
            }
        }
    }
}
