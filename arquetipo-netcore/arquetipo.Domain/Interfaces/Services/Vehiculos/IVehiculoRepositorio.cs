using arquetipo.Entity.Models;

namespace arquetipo.Domain.Interfaces.Services.Vehiculos
{
    public interface IVehiculoRepositorio : IGenericRepositorio<EVehiculo, Guid>
    {
        Task<EVehiculo?> ObtenerPorPlacaAsync(string placa);
        Task<List<EVehiculo>> ObtenerPorParametrosAsync(Guid? marcaId = null, string? modelo = null);
    }
}
