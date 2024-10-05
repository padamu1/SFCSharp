using System;

namespace SFCSharp.Excution.SystemExec.SystemConsole.SystemConsoleError
{
    public class SFExecSCEW : IMethodHandler
    {
        public void Excute(Action<object> execCallback, params object[] param)
        {
            if (param == null)
            {
                execCallback?.Invoke(null);
                throw new ArgumentException();
            }

            if (param.Length == 1)
            {
                System.Console.Error.WriteLine(param[0].ToString());
                execCallback?.Invoke(null);
                return;
            }

            System.Console.Error.WriteLine(param.ToString());
            execCallback?.Invoke(null);
        }
    }
}
