namespace SFCSharp.Core
{
    public class SFContextLoader : ISFContextLoader
    {
        private SFContextManager contextManager;

        public SFContextLoader(SFContextManager contextManager)
        {
            this.contextManager = contextManager;
        }

        public void Load(string script)
        {

        }
    }
}
