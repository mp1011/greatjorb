namespace GreatJorb.Business.Features;

public record SetPropertiesFromTextCommand(JobPosting Posting, string Text) : IRequest<JobPosting>
{
    public class Handler : IRequestHandler<SetPropertiesFromTextCommand, JobPosting>
    {
        private readonly IMediator _mediator;

        public Handler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<JobPosting> Handle(SetPropertiesFromTextCommand request, CancellationToken cancellationToken)
        {
            var propertyMatches = await _mediator.Send(new TryParsePropertyFromTextQuery(request.Text));

            foreach(var match in propertyMatches)
            {
                match.JobInfoProperty.SetValue(request.Posting, match.ParsedValue);
            }

            return request.Posting;
        }
    }
}
