namespace SFCSharp.Core
{
    public class SFContextManager
    {
        private static SFContextManager instance;

        public static void Init()
        {
            if (instance == null)
            {
                instance = new SFContextManager();
            }
        }

        public ISFContextBuilder Builder => builder;

        public ISFContextLoader Loader => loader;


        private SFContextBuilder builder;
        private SFContextLoader loader;
        private SFContextCache cache;

        private SFContextManager()
        {
            builder = new SFContextBuilder(this);
            loader = new SFContextLoader(this);
            cache = new SFContextCache(this);
        }
    }
}
