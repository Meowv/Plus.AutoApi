namespace Plus.AutoApi
{
    public class AssemblyAutoApiOptions
    {
        public AssemblyAutoApiOptions(string apiPrefix = null, string httpVerb = null)
        {
            ApiPrefix = apiPrefix;
            HttpVerb = httpVerb;
        }

        public string ApiPrefix { get; }

        public string HttpVerb { get; }
    }
}