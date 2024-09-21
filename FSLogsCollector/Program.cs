using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.AspNetCore;

using FSLogsCollector.Settings;
using FSLogsCollector;
using FSLogsCollector.Services;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog();

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ")
);

builder.Services.AddSingleton<ILoggedMessageProcessor, LoggedMessageLogger>();

builder.Services.AddHostedService<LogsCollectorService>();

var app = builder.Build();
app.Run();