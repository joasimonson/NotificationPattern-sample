using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<Service>();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapGet("/domain-request/{date}/{valid}", async (
    [FromRoute] DateTime date,
    [FromRoute] bool valid,
    [FromServices] Service service,
    CancellationToken cancelationToken) =>
{
    var result = await service.Operation(date, valid, cancelationToken);

    return result.Result();
});

app.Run();

public partial class Program { }