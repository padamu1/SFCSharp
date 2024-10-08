﻿using System;
using System.Collections.Generic;
using SFCSharp.Excution.Base;
using SFCSharp.Excution.SystemExec.SystemConsole.SystemConsoleError;

namespace SFCSharp.Excution.SystemExec.SystemConsole
{
    public class SystemConsoleExecHandler : SFMethodHandlerBase
    {
        protected override void InitMethodHandler(ref Dictionary<string, IMethodHandler> _methodHandlerDic)
        {
            _methodHandlerDic = new Dictionary<string, IMethodHandler>()
            {
                { "WriteLine", new SFExecSC() },
            };
        }

        protected override void InitNamespaceHandler(ref Dictionary<string, INamespaceHandler>? _namespaceHandlerDic)
        {
            _namespaceHandlerDic = new Dictionary<string, INamespaceHandler>()
            {
                { "Error", new SystemConsoleErrorExecHandler() },
            };
        }
    }
}
