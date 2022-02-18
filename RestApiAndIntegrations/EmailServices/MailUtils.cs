using System;
using Newtonsoft.Json;
using Markdig;

namespace AD.CAAPS.EmailServices
{
    public static class MailUtils
    {
        public static string CurrentLocalTimeAsJson()
        {
            return JsonConvert.ToString(DateTime.Now);
        }

        public static string CurrentUtcTimeAsJson()
        {
            return JsonConvert.ToString(DateTime.UtcNow);
        }

        public static string MdToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            return Markdown.ToHtml(markdown, pipeline);
        }
    }

}
