namespace GreatJorb.Business.Features;

public record DisposeBrowserCommand() : IRequest<bool>
{
    public class Handler : IRequestHandler<DisposeBrowserCommand, bool>
    {
        private BrowserProvider _browserProvider;

        public Handler(BrowserProvider browserProvider)
        {
            _browserProvider = browserProvider;
        }

        public Task<bool> Handle(DisposeBrowserCommand request, CancellationToken cancellationToken)
        {
            _browserProvider.UnloadBrowser();
            return Task.FromResult(true);
        }
    }
}
