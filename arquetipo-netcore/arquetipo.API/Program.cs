using arquetipo.API;
using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Infrastructure;
using arquetipo.Infrastructure.Seeders;
using arquetipo.Infrastructure.Services.Clientes;
using arquetipo.Repository.Context;
using arquetipo.Repository.Services.Clientes;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
