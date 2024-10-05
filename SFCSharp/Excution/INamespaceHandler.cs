using System;

namespace SFCSharp.Excution
{
    public interface INamespaceHandler
    {
        public void Exec(string[] methodNames, Action<object> execCallback, int offset, params object[] args);
    }
}
