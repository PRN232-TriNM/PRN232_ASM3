using EVCS.GrpcService.TriNM;
using EVCS.GrpcService.TriNM.Services;
using EVCS.GrpcService.TriNM.Hubs;
using EVCS.TriNM.Services.Implements;
using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Context;
using EVCS.TriNM.Repositories.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5000", "https://localhost:7250", "http://localhost:7250", 
                          "http://localhost:5001", "https://localhost:5001", "http://localhost:7000", "https://localhost:7000",
                          "http://localhost:5206", "https://localhost:5206")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5112, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
    options.ListenLocalhost(5113, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

builder.Services.AddDbContext<EVChargingDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IServiceProviders, ServiceProviders>();

var app = builder.Build();

app.UseCors();

app.MapGrpcService<GreeterService>();
app.MapGrpcService<StationGRPCService>();
app.MapHub<StationHub>("/stationHub");
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
