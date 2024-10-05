using System;

namespace SFCSharp.Context
{
    public interface ISFMethod
    {
        public void Excute(Action<object> callback);
    }
}
