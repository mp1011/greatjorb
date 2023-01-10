namespace GreatJorb.Business.Features
{
    public record ParseSalaryQuery(string Text) : IRequest<ParseSalaryQuery.Result>
    {
        public record Result(decimal? Min, decimal? Max, SalaryType SalaryType);

        public class Handler : IRequestHandler<ParseSalaryQuery, Result>
        {
            public Task<Result> Handle(ParseSalaryQuery request, CancellationToken cancellationToken)
            {

                SalaryType salaryType = SalaryType.Unknown;

                if (request.Text.Contains("/yr", StringComparison.OrdinalIgnoreCase))
                    salaryType = SalaryType.Annual;
                else if (request.Text.Contains("/hr", StringComparison.OrdinalIgnoreCase))
                    salaryType = SalaryType.Hourly;
                else if (request.Text.Contains("hour", StringComparison.OrdinalIgnoreCase))
                    salaryType = SalaryType.Hourly;
                else if (request.Text.Contains("year", StringComparison.OrdinalIgnoreCase))
                    salaryType = SalaryType.Annual;
                
                var currencyRegex1 = @"\$\d+(\,)?\d+(K?)"; // $555K or $555
                var currencyRegex2 = @"\d+(\,)?\d+(K)"; // 555k
                var numberRegex = @"\d+(\,)?\d+(K?)";

                var currencies = GetCurrencies(currencyRegex1, request.Text)
                    .Union(GetCurrencies(currencyRegex2, request.Text))
                    .Distinct()
                    .ToArray();

                var otherNumbers = GetCurrencies(numberRegex, request.Text)
                    .Except(currencies)
                    .ToArray();

                if(currencies.Any() && otherNumbers.Any())
                {
                    currencies = currencies
                        .Union(otherNumbers)
                        .ToArray();
                }

                if(!currencies.Any() && otherNumbers.Any() && salaryType != SalaryType.Unknown)
                {
                    currencies = otherNumbers;
                }

                currencies = currencies
                    .Where(p => p.HasValue)
                    .OrderBy(p => p.Value)
                    .ToArray();

                if (currencies.Length == 0)
                    return Task.FromResult(new Result(null, null, SalaryType.Unknown));

                if(currencies.Length == 2 
                    && currencies[0] < 1000
                    && currencies[1] > 10000)
                {
                    currencies[0] *= 1000.0m;
                }

             

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
      
            private IEnumerable<decimal?> GetCurrencies(string regex, string text)
            {
                return Regex
                  .Matches(text, regex, RegexOptions.IgnoreCase)
                  .Select(p => p.Value.TryParseCurrency());
            }
        }
    }
}
