namespace GreatJorb.Business.Features;

public record TryParsePropertyFromTextQuery(string Text) : IRequest<TextParseResult[]>
{
    public class Handler : IRequestHandler<TryParsePropertyFromTextQuery, TextParseResult[]>
    {
        private Dictionary<string, TextParseResult> _specialMatches = new();


        public Handler()
        {
            _specialMatches["contractor"] = CreateResult(nameof(JobPosting.JobType), JobType.Contract);
            _specialMatches["work from home"] = CreateResult(nameof(JobPosting.WorkplaceType), WorkplaceType.Remote);
            _specialMatches["remote-eligible"] = CreateResult(nameof(JobPosting.WorkplaceType), WorkplaceType.Remote);

        }

        public Task<TextParseResult[]> Handle(TryParsePropertyFromTextQuery request, CancellationToken cancellationToken)
        {
            List<TextParseResult> results = new();

            foreach(var textToSearch in GetTextToSearch(request.Text))
            {
                results.AddRange(TryParseText(textToSearch));
            }
            
            return Task.FromResult(results.ToArray());
        }

        private IEnumerable<string> GetTextToSearch(string requestText)
        {
            yield return requestText;

            var textInParens = Regex.Match(requestText, @"\(([^\)]+)\)", RegexOptions.IgnoreCase);

            if (textInParens.Success)
            {
                yield return textInParens.Groups[1].Value;
            }
        }

        private IEnumerable<TextParseResult> TryParseText(string text)
        {
            var maybeJobType = text.TryParseEnumAdvanced(JobType.Unknown);
            if (maybeJobType != JobType.Unknown)
                yield return CreateResult(text, nameof(JobPosting.JobType), maybeJobType);

            var maybeWorkplaceType = text.TryParseEnumAdvanced(WorkplaceType.Unknown);
            if (maybeWorkplaceType != WorkplaceType.Unknown)
                yield return CreateResult(text, nameof(JobPosting.WorkplaceType), maybeWorkplaceType);

            var maybeSalaryType = text.TryParseEnumAdvanced(SalaryType.Unknown);
            if (maybeSalaryType != SalaryType.Unknown)
                yield return CreateResult(text, nameof(JobPosting.SalaryType), maybeSalaryType);

            var maybeJobLevel = text.TryParseEnumAdvanced(JobLevel.Unknown);
            if (maybeJobLevel != JobLevel.Unknown)
                yield return CreateResult(text, nameof(JobPosting.JobLevel), maybeJobLevel);

            var specialMatch = _specialMatches.GetValueOrDefault(text.ToLower().Trim());
            if (specialMatch != null)
                yield return new TextParseResult(text, specialMatch.JobInfoProperty, specialMatch.ParsedValue);


            var maybeSalary = Regex.Match(text, @"(\$?)(\d+)(K?)(\sto\s|-|–|—)(\$?)(\d+)K(.*)", RegexOptions.IgnoreCase);

            if (maybeSalary.Success)
            {
                int[] values = new[]
                {
                    maybeSalary.Groups[2].Value.TryParseInt(0),
                    maybeSalary.Groups[6].Value.TryParseInt(0)
                }.OrderBy(p => p).ToArray();

                string type = maybeSalary.Groups[3].Value;

                if (type.Contains("year"))
                    yield return CreateResult(text, nameof(JobPosting.SalaryType), SalaryType.Annual);

                if (values[0] > 0)
                    yield return CreateResult(text, nameof(JobPosting.SalaryMin), values[0] * 1000.0m);

                if (values[1] > 0)
                    yield return CreateResult(text, nameof(JobPosting.SalaryMax), values[1] * 1000.0m);
            }
        }


        private TextParseResult CreateResult(string text, string propertyName, object value)
        {
            return new TextParseResult(text, typeof(JobPosting).GetProperty(propertyName)!, value);
        }

        private TextParseResult CreateResult(string propertyName, object value)
        {
            return new TextParseResult("", typeof(JobPosting).GetProperty(propertyName)!, value);
        }
    }
}
