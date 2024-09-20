namespace CommonModule.Core.Strategies.Create;

public interface ICreateStrategy<in TCommand, TResponse>
{
    Task<TResponse> ExecuteAsync(TCommand command ,CancellationToken cancellationToken);
}