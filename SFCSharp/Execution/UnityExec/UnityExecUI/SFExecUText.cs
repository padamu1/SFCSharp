using SFCSharp.Execution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec.UnityExecUI
{
    /// <summary>
    /// UnityEngine.UI.Text 메서드 핸들러
    /// </summary>
    public class SFExecUText : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"SetText", new SetTextHandler() },
                {"GetText", new GetTextHandler() },
                {"SetFontSize", new SetFontSizeHandler() },
                {"GetFontSize", new GetFontSizeHandler() },
                {"SetColor", new SetColorHandler() },
                {"GetColor", new GetColorHandler() },
                {"SetAlignment", new SetAlignmentHandler() },
                {"SetFontStyle", new SetFontStyleHandler() },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        private static SFText GetTextComponent(object arg)
        {
            if (arg is SFText text)
                return text;

            if (arg is SFGameObject go)
            {
                var comp = go.GetComponent("Text");
                if (comp is SFText t)
                    return t;
                throw new ArgumentException("GameObject does not have a Text component");
            }

            throw new ArgumentException("Argument must be a Text component or a GameObject with Text");
        }

        private class SetTextHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("SetText requires 2 arguments: textComponent, text");

                    var text = GetTextComponent(args[0]);
                    text.text = args[1]?.ToString() ?? "";
                    execCallback?.Invoke(text);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Text.SetText error: {ex.Message}", ex));
                }
            }
        }

        private class GetTextHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("GetText requires 1 argument: textComponent");

                    var text = GetTextComponent(args[0]);
                    execCallback?.Invoke(text.text);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Text.GetText error: {ex.Message}", ex));
                }
            }
        }

        private class SetFontSizeHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("SetFontSize requires 2 arguments: textComponent, size");

                    var text = GetTextComponent(args[0]);
                    text.fontSize = Convert.ToInt32(args[1]);
                    execCallback?.Invoke(text);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Text.SetFontSize error: {ex.Message}", ex));
                }
            }
        }

        private class GetFontSizeHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("GetFontSize requires 1 argument: textComponent");

                    var text = GetTextComponent(args[0]);
                    execCallback?.Invoke(text.fontSize);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Text.GetFontSize error: {ex.Message}", ex));
                }
            }
        }

        private class SetColorHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("SetColor requires 2 arguments: textComponent, color");

                    var text = GetTextComponent(args[0]);

                    if (args[1] is SFColor color)
                        text.color = color;
                    else
                        throw new ArgumentException("Second argument must be a Color");

                    execCallback?.Invoke(text);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Text.SetColor error: {ex.Message}", ex));
                }
            }
        }

        private class GetColorHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("GetColor requires 1 argument: textComponent");

                    var text = GetTextComponent(args[0]);
                    execCallback?.Invoke(text.color);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Text.GetColor error: {ex.Message}", ex));
                }
            }
        }

        private class SetAlignmentHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("SetAlignment requires 2 arguments: textComponent, alignment");

                    var text = GetTextComponent(args[0]);
                    var alignStr = args[1].ToString();

                    if (Enum.TryParse<TextAnchor>(alignStr, true, out var anchor))
                        text.alignment = anchor;
                    else
                        throw new ArgumentException($"Unknown alignment: {alignStr}");

                    execCallback?.Invoke(text);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Text.SetAlignment error: {ex.Message}", ex));
                }
            }
        }

        private class SetFontStyleHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("SetFontStyle requires 2 arguments: textComponent, fontStyle");

                    var text = GetTextComponent(args[0]);
                    var styleStr = args[1].ToString();

                    if (Enum.TryParse<FontStyle>(styleStr, true, out var style))
                        text.fontStyle = style;
                    else
                        throw new ArgumentException($"Unknown font style: {styleStr}");

                    execCallback?.Invoke(text);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Text.SetFontStyle error: {ex.Message}", ex));
                }
            }
        }
    }
}
