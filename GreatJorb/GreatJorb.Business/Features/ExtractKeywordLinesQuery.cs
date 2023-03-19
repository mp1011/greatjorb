using HtmlAgilityPack;

namespace GreatJorb.Business.Features;

public record ExtractKeywordLinesQuery(JobFilter Filter, string Html) : IRequest<KeywordLine[]>
{
    public class Handler : IRequestHandler<ExtractKeywordLinesQuery, KeywordLine[]>
    {
        public Task<KeywordLine[]> Handle(ExtractKeywordLinesQuery request, CancellationToken cancellationToken)
        {           
            var html = new HtmlDocument();
            html.LoadHtml(request.Html);

            var liTags = html.DocumentNode.Descendants("li");
            var pTags = html.DocumentNode.Descendants("p");
            var divTags = html.DocumentNode.Descendants("div")
                .Where(p => MayBeKeywordLine(p))
                .ToArray();


            var lines = liTags
                .Union(pTags)
                .Union(divTags)
                .Select(p => CleanUpLine(p.InnerText))
                .Union(ExtractBullets(request.Html))
                .SelectMany(p => ExtractSentences(p))
                .Distinct()
                .ToArray();

            var keywordLines = lines
                .Select(p=>ToKeywordLineOrDefault(p, request.Filter))
                .Where(p => p != null)
                .Distinct()
                .ToArray();

            keywordLines = keywordLines
                .Where(p => !keywordLines.Any(q => q != p && p.Line.Contains(q.Line, StringComparison.OrdinalIgnoreCase)))
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

        private IEnumerable<string> ExtractBullets(string html)
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

                if(line.Length > 0)
                    yield return line;

                index = endIndex;
            }
        }

        private KeywordLine? ToKeywordLineOrDefault(string line, JobFilter filter)
        {
            if (line.ContainsWord(filter.Query))
                return new KeywordLine(line, KeywordLineType.Query);

            foreach(var watchWord in filter.WatchWords)
            {
                if (line.ContainsWord(watchWord.Phrase))
                {
                    return new KeywordLine(line, watchWord.IsGood switch
                    {
                        true => KeywordLineType.PositiveKeyword,
                        false => KeywordLineType.NegativeKeyword,
                        _ => KeywordLineType.NeutralKeyword
                    });
                }
            }

            return null;
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
            line = line.Replace("•", "");
            line = Regex.Replace(line, @"\s+", " ");
            return line.Trim();
        }
    }
}
