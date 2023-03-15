using HtmlAgilityPack;

namespace GreatJorb.Business.Features;

public record ExtractKeywordLinesQuery(string Keyword, string Html) : IRequest<string[]>
{
    public class Handler : IRequestHandler<ExtractKeywordLinesQuery, string[]>
    {
        public Task<string[]> Handle(ExtractKeywordLinesQuery request, CancellationToken cancellationToken)
        {
            var html = new HtmlDocument();
            html.LoadHtml(request.Html);

            var liTags = html.DocumentNode.Descendants("li");
            var pTags = html.DocumentNode.Descendants("p");
            var divTags = html.DocumentNode.Descendants("div")
                .Where(p => MayBeKeywordLine(p))
                .ToArray();

            var keywordLines = liTags
                .Union(pTags)
                .Union(divTags)
                .Select(p => CleanUpLine(p.InnerText))
                .Union(ExtractBullets(request.Html, request.Keyword))
                .SelectMany(p => ExtractSentences(p))
                .Where(p => p.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .ToArray();

            keywordLines = keywordLines
                .Where(p => !keywordLines.Any(q => q != p && p.Contains(q, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            return Task.FromResult(keywordLines);
        }

        private bool MayBeKeywordLine(HtmlNode divTag)
        {
            var html = divTag.InnerHtml;
            var bulletIndex = html.IndexOf('•');
            if(bulletIndex == -1)
                return false; ;

            var nextBulletIndex = html.IndexOf('•', bulletIndex + 1);
            return nextBulletIndex == -1;
        }

        private IEnumerable<string> ExtractBullets(string html, string keyword)
        {
            char bullet = '•';

            int index = -1;
            while(index < html.Length)
            {
                index = html.IndexOf(bullet, index+1);
                if (index == -1)
                    break;

                int nextNewlineIndex = html.IndexOf("\n", index);
                int nextBulletIndex = html.IndexOf(bullet, index + 1);

                if (nextNewlineIndex == -1)
                    nextNewlineIndex = int.MaxValue;
                if (nextBulletIndex == -1)
                    nextBulletIndex = int.MaxValue;

                int endIndex = Math.Min(nextNewlineIndex,nextBulletIndex);
                if (endIndex == -1)
                    endIndex = html.Length;

                int length = (endIndex - index) - 1;

                string line;
                if (index + 1 + length >= html.Length)
                {
                    line = html
                        .Substring(index + 1)
                        .Trim();
                }
                else
                {
                    line = html
                        .Substring(index + 1, length)
                        .Trim();
                }

                line = Regex.Replace(line, "<.*?>", "");

                if (line.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    yield return line;

                index = endIndex;
            }
        }

        private IEnumerable<string> ExtractSentences(string input)
        {
            var maybeSentences = input.Split(". ");
            if (maybeSentences.Length >= 3)
                return maybeSentences;
            else
                return new string[] { input };
        }

        private string CleanUpLine(string line)
        {
            line = line.Trim();
            line = Regex.Replace(line, @"\s+", " ");
            return line;
        }
    }
}
