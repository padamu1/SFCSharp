using SFCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFCSharp.Excution.Base
{
    public abstract class SFMethodHandlerBase : INamespaceHandler
    {
        protected Dictionary<string, IMehtodHandler> _methodHandlerDic;

        public SFMethodHandlerBase()
        {
            InitExecHandler(ref _methodHandlerDic);
        }

        public abstract void InitExecHandler(ref Dictionary<string, IMehtodHandler> _methodHandlerDic);

        public void Exec(string[] methodNames, Action<object> execCallback, int offset, params object[] args)
        {
            if (_methodHandlerDic == null)
            {
                Logger.Error("_methodHandlerDic is null");
                return;
            }

            if (_methodHandlerDic.ContainsKey(methodNames[offset]))
            {
                _methodHandlerDic[methodNames[offset]].Excute(execCallback, args);
                return;
            }

            Logger.Error($"{string.Join(',', methodNames)} method not exist");
        }
    }
}
