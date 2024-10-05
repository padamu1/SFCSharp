using SFCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFCSharp.Excution.Base
{
    public abstract class SFMethodHandlerBase : SFNamespaceHandlerBase
    {
        protected Dictionary<string, IMethodHandler> _methodHandlerDic;

        public SFMethodHandlerBase() : base()
        {
            InitMethodHandler(ref _methodHandlerDic);
        }

        protected abstract void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic);

        public override bool Exec(string[] methodNames, Action<object> execCallback, int offset, params object[] args)
        {
            if(base.Exec(methodNames, execCallback, offset, args))
            {
                return true;
            }

            if (_methodHandlerDic == null)
            {
                return false;
            }

            if (_methodHandlerDic.ContainsKey(methodNames[offset]))
            {
                _methodHandlerDic[methodNames[offset]].Excute(execCallback, args);
                return true;
            }

            return false;
        }
    }
}
