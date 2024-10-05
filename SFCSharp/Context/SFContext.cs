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

        HashSet<string> privateMethods = new HashSet<string>();
        HashSet<string> publicMethods = new HashSet<string>();

        Dictionary<string, ISFVariable> publicVariableDic = new Dictionary<string, ISFVariable>();
        Dictionary<string, ISFVariable> privateVariableDic = new Dictionary<string, ISFVariable>();



    }
}
