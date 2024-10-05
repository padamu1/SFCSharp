using System;

namespace SFCSharp.Context
{
    public class SFVariable<T> : ISFVariable
    {
        private T value;

        public SFVariable()
        {
            value = default(T);
        }

        public Type GetValueType()
        {
            return typeof(T);
        }

        public object GetValue()
        {
            return value;
        }

        public void SetValue(T t)
        {
            value = t;
        }
    }
}
