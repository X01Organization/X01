﻿using Microsoft.AspNetCore.Mvc.Formatters;

namespace ForwardNotifyToTestEnvironment
{
  public class TextPlainInputFormatter : InputFormatter
{
    private const string ContentType = "text/plain";

    public TextPlainInputFormatter()
    {
        SupportedMediaTypes.Add(ContentType);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var request = context.HttpContext.Request;
        using (var reader = new System.IO.StreamReader(request.Body))
        {
            string content = await reader.ReadToEndAsync();
            return await InputFormatterResult.SuccessAsync(content);
        }
    }

    public override bool CanRead(InputFormatterContext context)
    {
        string contentType = context.HttpContext.Request.ContentType;
        return contentType.StartsWith(ContentType);
    }
}}
