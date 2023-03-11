namespace GreatJorb.Business.Features.Behaviors;

public class ExceptionHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMediator _mediator;

    public ExceptionHandlerBehavior(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new BrowserPageChanged(null, "", BrowserAction.FatalError, ex));
            throw;
        }
    }
}
