using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace arquetipo.Domain.Interfaces.Services.Vehiculos
{
    public interface IVehiculoInfraestructura
    {
        Task<IEnumerable<EVehiculo>> ConsultarVehiculosAsync();
        Task<EVehiculo> ConsultarVehiculoPorPlacaAsync(string placa);
        Task<IEnumerable<EVehiculo>> ConsultarVehiculosPorParametrosAsync(string? marca = null, string? modelo = null);
        Task<EVehiculo> CrearVehiculoAsync(ECrearVehiculoDto input);
        Task<EVehiculo> ActualizarVehiculoAsync(string placa, JsonPatchDocument<EVehiculo> input);
        Task<string> EliminarVehiculoAsync(string placa);
    }
}
