using CommonModule.Core.Auth;
using CommonModule.Core.Mediatr.Auth.Requests;
using MediatR;

namespace CommonModule.Core.Mediatr.Auth.Handlers;

public class GetCurrentUserIdRequestHandler: IRequestHandler<GetCurrentUserIdRequest, Guid?>
{
    private readonly IAuthService authService;

    public GetCurrentUserIdRequestHandler(IAuthService authService)
    {
        this.authService = authService;
    }

    public async Task<Guid?> Handle(GetCurrentUserIdRequest request, CancellationToken cancellationToken)
    {
        return await this.authService.GetCurrentUserIdAsync();
    }
}