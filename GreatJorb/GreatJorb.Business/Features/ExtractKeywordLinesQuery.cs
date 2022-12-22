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

            return Task.FromResult(liTags
                .Union(pTags)
                .Where(p => p.InnerText.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase))
                .Select(p => CleanUpLine(p.InnerText))
                .Distinct()
                .ToArray());
        }

        private string CleanUpLine(string line)
        {
            line = line.Trim();
            line = Regex.Replace(line, @"\s+", " ");
            return line;
        }
    }
}
