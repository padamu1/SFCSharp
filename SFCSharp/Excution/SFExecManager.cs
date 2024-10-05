using System;

namespace SFCSharp.Excution
{
    public class SFExecManager
    {
        private static SFExecManager instance;

        private SFExecManager()
        {
            sfExecHandler = new SFExecHandler();
        }

        INamespaceHandler sfExecHandler;

        public static void Exec(string method, Action<object> execCallback, params object[] args)
        {
            if (instance == null)
            {
                instance = new SFExecManager();
            }

            instance.Exec(method.Split('.'), execCallback, args);
        }

        private void Exec(string[] methods, Action<object> execCallback, params object[] args)
        {
            sfExecHandler.Exec(methods, execCallback, 0, args);
        }
    }
}
