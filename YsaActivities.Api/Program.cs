using YsaActivities.Api;
using YsaActivities.Api.Endpoints;
using YsaActivities.Api.Models.MemberAggregate;
using YsaActivities.Api.RouteConstraints;
using YsaActivities.Api.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSchemasForIdentifiersInAssembly<NoteId>();
});
builder.Services.AddEndpointModulesAssemblyWithType<Program>();
builder.Services.AddScoped<UnitRepository>();
builder.Services.AddScoped<ActivityRepository>();

builder.Services.Configure<RouteOptions>(routeOptions =>
{
    routeOptions.ConstraintMap.Add("noteid", typeof(NoteRouteConstraint));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseEndpointModules();

await app.RunAsync();