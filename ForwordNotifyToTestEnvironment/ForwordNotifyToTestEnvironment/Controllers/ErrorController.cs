using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ForwordNotifyToTestEnvironment.Controllers;


[AllowAnonymous]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> Log;

    /// <summary>
    ///     Main constructor for this class.
    /// </summary>
    /// <param name="pContext">Out DBContext, which will be used to get a db connection</param>
    /// <param name="logger">NLog Injection</param>
    public ErrorController(ILogger<ErrorController> logger)
    {
        Log = logger;
    }

    /// <summary>
    ///     Get method for providing detailed exception messages
    /// </summary>
    /// <returns>IActionResult with the HTTP status code</returns>
    [Route("")]
    [AllowAnonymous]
    [HttpGet]
    [HttpPost]
    [HttpPut]
    [HttpDelete]
    [HttpHead]
    [HttpOptions]
    [HttpPatch]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Get()
    {
        // Get the details of the exception
        IExceptionHandlerPathFeature? exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature != null)
            Log.LogError(string.Format("Exception at route: '{0}'. Exception: {1}", exceptionFeature.Path,
                exceptionFeature.Error));

        //we better do not show the whole exception stack trace
        //return StatusCode(StatusCodes.Status500InternalServerError, exceptionFeature.Error);

        return StatusCode(StatusCodes.Status500InternalServerError, exceptionFeature?.Error?.Message);
    }
}