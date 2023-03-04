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

            var keywordLines = liTags
                .Union(pTags)
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

        private IEnumerable<string> ExtractBullets(string html, string keyword)
        {
            char bullet = '•';

            int index = -1;
            while(index < html.Length)
            {
                index = html.IndexOf(bullet, index+1);
                if (index == -1)
                    break;

                int endIndex = html.IndexOf("\n", index);
                if (endIndex == -1)
                    endIndex = html.Length;

                var line = html
                    .Substring(index + 1, (endIndex - index)-1)
                    .Trim();

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
