using SFCSharp.Utils;
using System;
using System.Collections.Generic;

namespace SFCSharp.Excution.Base
{
    public abstract class SFNamespaceHandlerBase : INamespaceHandler
    {
        protected Dictionary<string, INamespaceHandler>? _namespaceHandlerDic;

        public SFNamespaceHandlerBase()
        {
            InitExecHandler(ref _namespaceHandlerDic);
        }

        public abstract void InitExecHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic);

        public void Exec(string[] methodNames, Action<object> execCallback, int offset, params object[] args)
        {
            if (_namespaceHandlerDic == null)
            {
                Logger.Error("_execHandlerDic is null");
                return;
            }

            if (_namespaceHandlerDic.ContainsKey(methodNames[offset]))
            {
                _namespaceHandlerDic[methodNames[offset]].Exec(methodNames, execCallback, ++offset, args);
                return;
            }

            Logger.Error($"{string.Join(',', methodNames)} namespace not exist");
        }
    }
}
