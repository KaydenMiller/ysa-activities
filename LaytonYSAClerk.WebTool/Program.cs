using System.Security.AccessControl;
using LaytonYSAClerk.WebTool.Components;
using LaytonYSAClerk.WebTool.Services;
using MongoDB.Driver;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
   .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddMudServices();
builder.Services.AddSingleton<MemberService>();
builder.Services.AddSingleton<MemberRepository>();
builder.Services.AddSingleton<MongoClient>(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("default");
    return new MongoClient(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();