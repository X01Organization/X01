using ForwardNotifyToTestEnvironment;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(o =>
                     o.InputFormatters.Insert(o.InputFormatters.Count, new TextPlainInputFormatter()))
                .AddJsonOptions(options =>
                     options.JsonSerializerOptions.Converters.Add(
                         new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.Configure<FormOptions>(options =>
 {
     options.ValueLengthLimit = 10000;            // 100mb in bytes
     options.MultipartBodyLengthLimit = 10000;    // 100mb in bytes
     options.MultipartHeadersLengthLimit = 10000; // 100mb in bytes
 });
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/Error");
app.UseMiddleware<HttpStatusCodeExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
