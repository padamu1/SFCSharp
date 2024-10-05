using System;

namespace SFCSharp.Excution
{
    public interface IMethodHandler
    {
        public void Excute(Action<object> execCallback, params object[] param);
    }
}
