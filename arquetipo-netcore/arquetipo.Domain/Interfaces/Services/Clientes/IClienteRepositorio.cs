using arquetipo.Entity.Models;

namespace arquetipo.Domain.Interfaces.Services.Clientes
{
    public interface IClienteRepositorio : IGenericRepositorio<ECliente, Guid>
    {
        Task<ECliente?> ObtenerPorIdentificacionAsync(string identificacion);
    }
}
