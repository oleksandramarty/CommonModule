using CommonModule.Core.Errors;
using CommonModule.Shared.Constants;

namespace CommonModule.Core.Exceptions;

public class AuthenticationException : LoggerException
{
    public AuthenticationException(string message, int _statusCode, Guid? _userId = null, string _payload = null) : base(message, _statusCode, _userId, null, _payload)
    {
    }
    
    new public ErrorMessage ToErrorMessage()
    {
        if(statusCode == 409)
        {
            return new ErrorMessage(ErrorMessages.UserBlocked, statusCode);
        }

        return new ErrorMessage(ErrorMessages.WrongAuth, statusCode);   
    }
}