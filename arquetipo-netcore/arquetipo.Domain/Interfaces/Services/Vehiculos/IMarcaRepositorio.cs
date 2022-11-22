using arquetipo.Entity.Models;

namespace arquetipo.Domain.Interfaces.Services.Vehiculos
{
    public interface IMarcaRepositorio : IGenericRepositorio<EMarca, Guid>
    {
        Task<EMarca?> ObtenerPorNombreAsync(string nombre);
    }
}
