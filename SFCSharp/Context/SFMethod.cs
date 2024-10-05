using System;

namespace SFCSharp.Context
{
    public class SFMethod : ISFMethod
    {
        SFContext sfContext;
        string context;

        public SFMethod(SFContext sfContext, string context)
        {
            this.sfContext = sfContext;
            this.context = context;
        }

        public void Excute(Action<object> callback)
        {
            callback?.Invoke(Excute());
        }

        public object Excute()
        {

            return null;
        }
    }
}
