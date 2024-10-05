using System;
using System.Collections.Generic;
using SFCSharp.Excution.Base;

namespace SFCSharp.Excution.SystemExec.SystemConsole
{
    public class SystemConsoleExecHandler : SFMethodHandlerBase
    {
        public override void InitExecHandler(ref Dictionary<string, IMehtodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMehtodHandler>()
            {
                {"WriteLine", new SFExecSC() },
            };
        }
    }
}
