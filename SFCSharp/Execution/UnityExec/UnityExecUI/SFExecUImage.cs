using SFCSharp.Execution.Base;
using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.UnityExec.UnityExecUI
{
    /// <summary>
    /// UnityEngine.UI.Image 메서드 핸들러
    /// </summary>
    public class SFExecUImage : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                {"SetColor", new SetColorHandler() },
                {"GetColor", new GetColorHandler() },
                {"SetFillAmount", new SetFillAmountHandler() },
                {"GetFillAmount", new GetFillAmountHandler() },
                {"SetEnabled", new SetEnabledHandler() },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = null;
        }

        private static SFImage GetImageComponent(object arg)
        {
            if (arg is SFImage image)
                return image;

            if (arg is SFGameObject go)
            {
                var comp = go.GetComponent("Image");
                if (comp is SFImage img)
                    return img;
                throw new ArgumentException("GameObject does not have an Image component");
            }

            throw new ArgumentException("Argument must be an Image component or a GameObject with Image");
        }

        private class SetColorHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("SetColor requires 2 arguments: imageComponent, color");

                    var image = GetImageComponent(args[0]);

                    if (args[1] is SFColor color)
                        image.color = color;
                    else
                        throw new ArgumentException("Second argument must be a Color");

                    execCallback?.Invoke(image);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Image.SetColor error: {ex.Message}", ex));
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
                        throw new ArgumentException("GetColor requires 1 argument: imageComponent");

                    var image = GetImageComponent(args[0]);
                    execCallback?.Invoke(image.color);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Image.GetColor error: {ex.Message}", ex));
                }
            }
        }

        private class SetFillAmountHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("SetFillAmount requires 2 arguments: imageComponent, amount");

                    var image = GetImageComponent(args[0]);
                    float amount = Convert.ToSingle(args[1]);
                    image.fillAmount = Math.Max(0f, Math.Min(1f, amount));
                    execCallback?.Invoke(image);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Image.SetFillAmount error: {ex.Message}", ex));
                }
            }
        }

        private class GetFillAmountHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 1)
                        throw new ArgumentException("GetFillAmount requires 1 argument: imageComponent");

                    var image = GetImageComponent(args[0]);
                    execCallback?.Invoke(image.fillAmount);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Image.GetFillAmount error: {ex.Message}", ex));
                }
            }
        }

        private class SetEnabledHandler : IMethodHandler
        {
            public void Execute(Action<object> execCallback, params object[] args)
            {
                try
                {
                    if (args.Length < 2)
                        throw new ArgumentException("SetEnabled requires 2 arguments: imageComponent, enabled");

                    var image = GetImageComponent(args[0]);
                    image.enabled = Convert.ToBoolean(args[1]);
                    execCallback?.Invoke(image);
                }
                catch (Exception ex)
                {
                    execCallback?.Invoke(new Exception($"Image.SetEnabled error: {ex.Message}", ex));
                }
            }
        }
    }
}
