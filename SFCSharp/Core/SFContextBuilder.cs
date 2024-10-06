namespace SFCSharp.Core
{
    public class SFContextBuilder : ISFContextBuilder
    {
        private SFContextManager contextManager;

        public SFContextBuilder(SFContextManager contextManager)
        {
            this.contextManager = contextManager;
        }

        public void Build(string script)
        {
        }
    }
}
