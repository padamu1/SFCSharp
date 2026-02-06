using System;

namespace SFCSharp.Execution
{
    public interface INamespaceHandler
    {
        public bool Exec(string[] methodNames, Action<object> execCallback, int offset, params object[] args);
    }
}
