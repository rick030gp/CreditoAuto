using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace arquetipo.Domain.Interfaces.Services.ClientePatio
{
    public interface IClientePatioInfraestructura
    {
        Task<EClientePatio> AsociarClientePatioAsync(EAsociarClientePatioDto input);
        Task<EClientePatio> ActualizarAsociacionClientePatioAsync(Guid id, JsonPatchDocument<EClientePatio> input);
        Task<string> EliminarAsociacionClientePatioAsync(Guid id);
    }
}
