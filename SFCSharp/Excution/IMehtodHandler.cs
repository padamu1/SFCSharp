using System;

namespace SFCSharp.Excution
{
    public interface IMehtodHandler
    {
        public void Excute(Action<object> execCallback, params object[] param);
    }
}
