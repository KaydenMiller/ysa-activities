using Cocona;
using LaytonYSAClerk.Cli;
using LaytonYSAClerk.Cli.Commands;
using LaytonYSAClerk.Cli.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddHostedService<ChurchDatabaseInitializerService>();
builder.Services.AddScoped<WebsiteService>();
builder.Services.AddScoped<MembersRepository>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddDbContext<ChurchDbContext>(options =>
{
    options.UseSqlite($"Data Source={builder.Environment.ContentRootPath}/Church.db");
});
builder.Services.AddOpenTelemetry()
   .WithTracing(tracing => tracing.AddSource(ChurchDatabaseInitializerService.ActivitySourceName));

var app = builder.Build();

app.RegisterMembersCommand();



await app.RunAsync();