using System;

namespace SFCSharp.Execution
{
    public interface IMethodHandler
    {
        public void Execute(Action<object> execCallback, params object[] param);
    }
}
