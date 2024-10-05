using System;

namespace SFCSharp.Context
{
    public interface ISFVariable
    {
        public Type GetValueType();

        public object GetValue();
    }
}
