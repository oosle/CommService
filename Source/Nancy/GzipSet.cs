using System.Collections.Generic;

namespace CommService
{
    public class GzipSettings
    {
        public int MinimumBytes { get; set; } = 4096;

        public IList<string> MimeTypes { get; set; } = new List<string>
        {
            "text/plain",
            "text/html",
            "text/xml",
            "text/css",
            "application/json",
            "application/javascript",
            "application/x-javascript",
            "application/ecmascript",
            "application/atom+xml",
            "application/xml"
        };
    }
}
