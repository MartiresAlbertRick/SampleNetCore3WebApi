using System.Collections.Generic;

namespace AD.CAAPS.API.Models
{
    public class CustomCacheSettings
    {
        public int DurationSeconds { get; set; }
        public List<string> VaryByQueryKeys { get; } = new List<string>();
        public string VaryByHeader { get; set; }
    }
}