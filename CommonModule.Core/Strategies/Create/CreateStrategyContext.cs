namespace CommonModule.Core.Strategies.Create;

public class CreateStrategyContext<TCommand, TResponse>
{
    private ICreateStrategy<TCommand, TResponse> _strategy;

    public CreateStrategyContext(ICreateStrategy<TCommand, TResponse> strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(ICreateStrategy<TCommand, TResponse> strategy)
    {
        _strategy = strategy;
    }

    public async Task<TResponse> ExecuteStrategyAsync(TCommand command, CancellationToken cancellationToken)
    {
        return await _strategy.ExecuteAsync(command, cancellationToken);
    }
}