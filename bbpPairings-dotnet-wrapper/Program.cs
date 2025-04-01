using bbpPairings_dotnet_wrapper;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ExecutableOptions>(builder.Configuration.GetSection("Executable"));
builder.Services.AddScoped<Pairer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/generate-dummy-tournaments", async ([FromServices] Pairer pairer, [FromQuery] int playersNumber, [FromQuery] int roundsNumber) =>
    {
        string output = await pairer.GenerateRandomTournament(PairingSystem.Fast, playersNumber, roundsNumber);

        return Results.Ok(output.Split("\r"));
    })
    .WithOpenApi();

app.Run();