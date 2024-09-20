using CommonModule.Core.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommonModule.Core;

[Route("api/{version}/[controller]")]
[ApiController]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status417ExpectationFailed)]
[ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status500InternalServerError)]
public class BaseController : Controller
{
    public BaseController()
    {
    }
}