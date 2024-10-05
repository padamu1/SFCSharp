using System.Collections.Generic;

namespace SFCSharp.Context
{
    public class SFContext
    {
        private readonly string contextNamespace;

        public string GetContextNamespace()
        {
            return contextNamespace;
        }

        public SFContext(string contextNamespace, string context)
        {
            this.contextNamespace = contextNamespace;
        }

        Dictionary<string, ISFMethod> privateMethodDic = new Dictionary<string, ISFMethod>();
        Dictionary<string, ISFMethod> publicMethodDic = new Dictionary<string, ISFMethod>();
        Dictionary<string, ISFVariable> publicVariableDic = new Dictionary<string, ISFVariable>();
        Dictionary<string, ISFVariable> privateVariableDic = new Dictionary<string, ISFVariable>();



    }
}
