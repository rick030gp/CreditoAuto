using arquetipo.Entity.Models;

namespace arquetipo.Domain.Interfaces.Services.ClientePatio
{
    public interface IClientePatioRepositorio : IGenericRepositorio<EClientePatio, Guid>
    {
        Task<EClientePatio?> ObtenerPorParametrosAsync(Guid clienteId, Guid patioId);
    }
}
