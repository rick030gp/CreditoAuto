using arquetipo.API;
using arquetipo.Domain.Interfaces.Services.ClientePatio;
using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Domain.Interfaces.Services.SolicitudCredito;
using arquetipo.Domain.Interfaces.Services.Vehiculos;
using arquetipo.Infrastructure;
using arquetipo.Infrastructure.Seeders;
using arquetipo.Infrastructure.Services.ClientePatio;
using arquetipo.Infrastructure.Services.Clientes;
using arquetipo.Infrastructure.Services.Patios;
using arquetipo.Infrastructure.Services.SolicitudCredito;
using arquetipo.Infrastructure.Services.Vehiculos;
using arquetipo.Repository.Context;
using arquetipo.Repository.Services.ClientePatio;
using arquetipo.Repository.Services.Clientes;
using arquetipo.Repository.Services.Patios;
using arquetipo.Repository.Services.SolicitudCredito;
using arquetipo.Repository.Services.Vehiculos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddFile("Logs/CrAuto-{Date}.txt");

builder.Services.AddDbContext<CrAutoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mapper
var mapperConfig = new MapperConfiguration(m =>
{
    m.AddProfile(new CrAutoMapperProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<SeederDb>();
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IClienteInfraestructura, ClienteInfraestructura>();
builder.Services.AddScoped<IPatioRepositorio, PatioRepositorio>();
builder.Services.AddScoped<IPatioInfraestructura, PatioInfraestructura>();
builder.Services.AddScoped<IMarcaRepositorio, MarcaRepositorio>();
builder.Services.AddScoped<IVehiculoRepositorio, VehiculoRepositorio>();
builder.Services.AddScoped<IVehiculoInfraestructura, VehiculoInfraestructura>();
builder.Services.AddScoped<IClientePatioRepositorio, ClientePatioRepositorio>();
builder.Services.AddScoped<IClientePatioInfraestructura, ClientePatioInfraestructura>();
builder.Services.AddScoped<ISolicitudCreditoRepositorio, SolicitudCreditoRepositorio>();
builder.Services.AddScoped<ISolicitudCreditoInfraestructura, SolicitudCreditoInfraestructura>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseItToSeedSqlServer();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

[ExcludeFromCodeCoverage]
public partial class Program { }