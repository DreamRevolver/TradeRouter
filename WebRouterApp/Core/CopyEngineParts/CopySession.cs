namespace WebRouterApp.Core.CopyEngineParts
{
    public class CopySession
    {
        public CopySession(Publisher publisher)
        {
            Publisher = publisher;
        }
        
        public Publisher Publisher { get; }
    }
}