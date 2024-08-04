namespace NetBB.Sources.Constants
{
    public class AllowedPostContentType
    {
        public static ISet<string> ContentTypes { get; set; } = new HashSet<string>()
        {
            "markdown_v1", // markdown from https://github.com/Vanessa219/vditor
        };
    }
}
