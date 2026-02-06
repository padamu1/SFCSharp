using System;
using System.Collections.Generic;

namespace SFCSharp.Execution.Base
{
    public abstract class SFNamespaceHandlerBase : INamespaceHandler
    {
        protected Dictionary<string, INamespaceHandler>? _namespaceHandlerDic;

        public SFNamespaceHandlerBase()
        {
            InitNamespaceHandler(ref _namespaceHandlerDic);
        }

        protected abstract void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic);

        public virtual bool Exec(string[] methodNames, Action<object> execCallback, int offset, params object[] args)
        {
            if (_namespaceHandlerDic == null)
            {
                return false;
            }

            if (_namespaceHandlerDic.ContainsKey(methodNames[offset]))
            {
                _namespaceHandlerDic[methodNames[offset]].Exec(methodNames, execCallback, ++offset, args);
                return true;
            }

            return false;
        }
    }
}
