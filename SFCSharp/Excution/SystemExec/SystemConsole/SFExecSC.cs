using SFCSharp.Utils;
using System;

namespace SFCSharp.Excution.SystemExec.SystemConsole
{
    public class SFExecSC : IMehtodHandler
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
                System.Console.WriteLine(param[0].ToString());
                execCallback?.Invoke(null);
                return;
            }

            System.Console.WriteLine(param.ToString());
            execCallback?.Invoke(null);
        }
    }
}
