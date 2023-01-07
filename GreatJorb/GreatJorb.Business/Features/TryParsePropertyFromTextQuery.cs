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
        }

        public Task<TextParseResult[]> Handle(TryParsePropertyFromTextQuery request, CancellationToken cancellationToken)
        {
            List<TextParseResult> results = new();

            var maybeJobType = request.Text.TryParseEnumAdvanced(JobType.Unknown);
            if (maybeJobType != JobType.Unknown)
                results.Add(CreateResult(request, nameof(JobPosting.JobType), maybeJobType));

            var maybeWorkplaceType = request.Text.TryParseEnumAdvanced(WorkplaceType.Unknown);
            if (maybeWorkplaceType != WorkplaceType.Unknown)
                results.Add(CreateResult(request, nameof(JobPosting.WorkplaceType), maybeWorkplaceType));

            var maybeSalaryType = request.Text.TryParseEnumAdvanced(SalaryType.Unknown);
            if (maybeSalaryType != SalaryType.Unknown)
                results.Add(CreateResult(request, nameof(JobPosting.SalaryType), maybeSalaryType));

            var maybeJobLevel = request.Text.TryParseEnumAdvanced(JobLevel.Unknown);
            if (maybeJobLevel != JobLevel.Unknown)
                results.Add(CreateResult(request, nameof(JobPosting.JobLevel), maybeJobLevel));

            var specialMatch = _specialMatches.GetValueOrDefault(request.Text.ToLower().Trim());
            if(specialMatch != null)
                results.Add(new TextParseResult(request.Text, specialMatch.JobInfoProperty, specialMatch.ParsedValue));


            var maybeSalary = Regex.Match(request.Text, @"(\$?)(\d+)(K?)(\sto\s|-|–|—)(\$?)(\d+)K(.*)", RegexOptions.IgnoreCase);

            if (maybeSalary.Success)
            {
                int[] values = new[]
                {
                    maybeSalary.Groups[2].Value.TryParseInt(0),
                    maybeSalary.Groups[6].Value.TryParseInt(0)
                }.OrderBy(p => p).ToArray();

                string type = maybeSalary.Groups[3].Value;

                if (type.Contains("year"))
                    results.Add(CreateResult(request, nameof(JobPosting.SalaryType), SalaryType.Annual));

                if (values[0] > 0)
                    results.Add(CreateResult(request, nameof(JobPosting.SalaryMin), values[0] * 1000.0m));

                if (values[1] > 0)
                    results.Add(CreateResult(request, nameof(JobPosting.SalaryMax), values[1] * 1000.0m));
            }


            return Task.FromResult(results.ToArray());
        }

        private TextParseResult CreateResult(TryParsePropertyFromTextQuery request, string propertyName, object value)
        {
            return new TextParseResult(request.Text, typeof(JobPosting).GetProperty(propertyName)!, value);
        }

        private TextParseResult CreateResult(string propertyName, object value)
        {
            return new TextParseResult("", typeof(JobPosting).GetProperty(propertyName)!, value);
        }
    }
}
