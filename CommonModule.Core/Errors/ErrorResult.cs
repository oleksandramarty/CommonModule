using System.Net;
using CommonModule.Shared.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CommonModule.Core.Errors;

public class ErrorResult : ObjectResult
{
    public ErrorResult(int code, string message) : base(new ErrorMessage(message, code))
    {
        StatusCode = code;
    }

    public ErrorResult(int code, string message, ModelStateDictionary modelState)
        : base(new ErrorSerialization(message, code, modelState))
    {
        StatusCode = code;
    }

    public ErrorResult(ModelStateDictionary modelState)
        : base(new ErrorSerialization(ErrorMessages.InternalServerError, (int) HttpStatusCode.BadRequest,
            modelState))
    {
        StatusCode = 400;
    }
}