using System;

namespace SFCSharp.Context
{
    public class SFVariable<T> : ISFVariable
    {
        private readonly T value;

        public SFVariable(T value)
        {
            this.value = value;
        }

        public Type GetValueType()
        {
            return typeof(T);
        }

        public object GetValue()
        {
            return value;
        }
    }
}
