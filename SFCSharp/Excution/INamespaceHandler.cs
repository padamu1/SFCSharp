using System;

namespace SFCSharp.Excution
{
    public interface INamespaceHandler
    {
        public bool Exec(string[] methodNames, Action<object> execCallback, int offset, params object[] args);
    }
}
