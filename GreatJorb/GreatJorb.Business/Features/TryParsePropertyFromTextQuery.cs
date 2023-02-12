namespace GreatJorb.Business.Features;

public record TryParsePropertyFromTextQuery(string Text) : IRequest<TextParseResult[]>
{
    public class Handler : IRequestHandler<TryParsePropertyFromTextQuery, TextParseResult[]>
    {
        private readonly IMediator _mediator;
        private Dictionary<string, TextParseResult> _specialMatches = new();


        public Handler(IMediator mediator)
        {
            _mediator = mediator;
            _specialMatches["contractor"] = CreateResult(nameof(JobPosting.JobType), JobType.Contract);
            _specialMatches["work from home"] = CreateResult(nameof(JobPosting.WorkplaceType), WorkplaceType.Remote);
            _specialMatches["remote-eligible"] = CreateResult(nameof(JobPosting.WorkplaceType), WorkplaceType.Remote);

        }

        public async Task<TextParseResult[]> Handle(TryParsePropertyFromTextQuery request, CancellationToken cancellationToken)
        {
            List<TextParseResult> results = new();

            foreach(var textToSearch in GetTextToSearch(request.Text))
            {
                results.AddRange(TryParseText(textToSearch));
                results.AddRange(await TryParseSalary(textToSearch));
            }

            return results.ToArray();
        }

        private IEnumerable<string> GetTextToSearch(string requestText)
        {
            yield return requestText;

            var textInParens = Regex.Match(requestText, @"\(([^\)]+)\)", RegexOptions.IgnoreCase);

            if (textInParens.Success)
            {
                yield return textInParens.Groups[1].Value;
            }

            var splitByDot = requestText
                .Split('·')
                .Select(p => p.Trim())
                .ToArray();

            if(splitByDot.Length > 1)
            {
                foreach (var item in splitByDot)
                    yield return item;
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
        }

        private async Task<IEnumerable<TextParseResult>> TryParseSalary(string text)
        {
            List<TextParseResult> results = new();

            var maybeSalary = await _mediator.Send(new ParseSalaryQuery(text));

            if (maybeSalary.Max.HasValue && maybeSalary.Max.Value == 401000)
                return results; //text is 401k, not a salary

            if (maybeSalary.Min.HasValue)
                results.Add(CreateResult(text, nameof(JobPosting.SalaryMin), maybeSalary.Min.Value));

            if (maybeSalary.Max.HasValue)
                results.Add(CreateResult(text, nameof(JobPosting.SalaryMax), maybeSalary.Max.Value));

            if (maybeSalary.SalaryType != SalaryType.Unknown)
                results.Add(CreateResult(text, nameof(JobPosting.SalaryType), maybeSalary.SalaryType));

            return results;
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
