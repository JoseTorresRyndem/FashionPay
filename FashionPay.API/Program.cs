using FashionPay.Infrastructure.Extensions;
using FashionPay.Application.Extensions;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddFluentValidationAutoValidation();
// Configurar Infrastructure (DbContext, Repositorios, Unit of Work)
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "FashionPay API",
        Version = "v1",
        Description = "API para gestión de créditos de moda"
    });
});

// Configurar CORS para desarrollo
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation(" FashionPay API iniciada correctamente");
app.Logger.LogInformation(" Documentación disponible en: https://localhost:{Port}",
    app.Environment.IsDevelopment() ? "7000" : "443");
app.Run();
