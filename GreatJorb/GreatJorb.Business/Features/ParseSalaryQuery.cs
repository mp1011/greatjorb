namespace GreatJorb.Business.Features
{
    public record ParseSalaryQuery(string Text) : IRequest<ParseSalaryQuery.Result>
    {
        public record Result(decimal? Min, decimal? Max, SalaryType SalaryType);

        public class Handler : IRequestHandler<ParseSalaryQuery, Result>
        {
            public Task<Result> Handle(ParseSalaryQuery request, CancellationToken cancellationToken)
            {
                var currencyRegex = @"\$\d+(\,)?\d+";

                var currencies = Regex
                    .Matches(request.Text, currencyRegex)
                    .Select(p => p.Value.TryParseCurrency())
                    .ToArray();

                if (currencies.Length == 0)
                    return Task.FromResult(new Result(null, null, SalaryType.Unknown));

                SalaryType salaryType = SalaryType.Unknown;
                if (request.Text.Contains("/yr"))
                    salaryType = SalaryType.Annual;

                if (request.Text.Contains("/hr"))
                    salaryType = SalaryType.Hourly;

                if (currencies.Length == 1)
                {
                    return Task.FromResult(new Result(
                        Min: null,
                        Max: currencies[0],
                        SalaryType: salaryType));
                }
                else
                {

                    return Task.FromResult(new Result(
                        Min: currencies.Min(),
                        Max: currencies.Max(),
                        SalaryType: salaryType));
                }
            }
        }
    }
}
