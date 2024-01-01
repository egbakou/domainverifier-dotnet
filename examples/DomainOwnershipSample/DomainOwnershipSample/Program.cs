using Carter;
using DomainOwnershipSample.Database;
using DomainOwnershipSample.Database.Context;
using DomainOwnershipSample.Entities;
using DomainOwnershipSample.Features.DomainVerifications;
using DomainVerifier.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddDatabase();
builder.Services.AddBackgroundServices();
builder.Services.AddDomainVerifierServices(builder.Configuration);

builder.Services
    .AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Description = "Domain name ownership verification API",
        Version = "v1",
        Title = "DomainVerifier Demo",
    });
});

builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGroup("api/users").MapIdentityApi<User>()
    .WithTags("users");

app.MapCarter();

app.UseHttpsRedirection();
app.UseAuthorization();

app.Run();
