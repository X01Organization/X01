using System.Net;
using System.Text;
using System.Text.Json;


namespace ForwardNotifyToTestEnvironment
{
    public sealed class HttpStatusCodeExceptionMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        /// <summary>
        ///     Initialize dependencies.
        /// </summary>
        /// <param name="next">Request delegate.</param>
        /// <param name="logger">Logger.</param>
        public HttpStatusCodeExceptionMiddleware(RequestDelegate next, ILogger<HttpStatusCodeExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        ///     Method execution of middleware.
        /// </summary>
        /// <param name="context">Http context.</param>
        /// <returns>Returns task.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("TravelReport"))
                {
                }
                else
                {
                    string formattedRequest = await FormatRequest(context.Request);
                }

                await _next(context).ConfigureAwait(false);
                if ((context.Request?.Path.HasValue ?? false)
                 && context.Request.Path.Value.Contains("negotiate", StringComparison.OrdinalIgnoreCase))
                {
                    var resDto = new
                    {
                        Request = context?.Request?.Path.Value,
                        RequestHeaders = context?.Request?.Headers?.Select(x => new
                        {
                            Header = x.Key.ToString(),
                            Value = x.Value.ToString()
                        }),
                        ResponseHeaders = context?.Response?.Headers?.Select(x => new
                        {
                            Header = x.Key.ToString(),
                            Value = x.Value.ToString()
                        })
                    };
                    _logger.LogInformation(JsonSerializer.Serialize(resDto, new JsonSerializerOptions()
                    {
                        WriteIndented = true
                    }));
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error when executing request.");
                await WriteResponse(context, exception.Message);
            }
        }

        /// <summary>
        ///     Format request.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Returns info about request in string representation.</returns>
        private static async Task<string> FormatRequest(HttpRequest request)
        {
            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            string bodyAsText;
            if (IsParsableMime(request.ContentType))
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyAsText = await reader.ReadToEndAsync();
                }

                request.Body.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                bodyAsText = $"{nameof(request.ContentLength)}: {request.ContentLength}";
            }


            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        /// <summary>
        ///     Write the response of the specified context.
        /// </summary>
        /// <param name="context">Http context.</param>
        /// <param name="statusCode">Status code.</param>
        /// <param name="message">Message of answer.</param>
        private static async Task WriteResponse(HttpContext context,
            string message,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = @"text/plain; charset=utf-8";
            await context.Response.WriteAsync(message);
        }

        private static bool IsParsableMime(string type)
        {
            if (string.  IsNullOrEmpty(type))
                return true;
            string[] mime = type.Split("/");
            return mime[0] switch
            {
                "application" => mime[1] switch
                {
                    "xml" => true,
                    "json" => true,
                    "xhtml+xml" => true,
                    "x-www-form-urlencoded" => true,
                    _ => false
                },
                "text" => true,
                _ => false
            };
        }
    }
}
