using bbpPairings_dotnet_wrapper;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ExecutableOptions>(builder.Configuration.GetSection("Executable"));
builder.Services.AddScoped<Pairer>();
builder.Services
    
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app
    .UseDefaultExceptionHandler()
    .UseFastEndpoints()
    .UseSwaggerGen();

app.Run();