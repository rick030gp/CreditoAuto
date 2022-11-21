using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace arquetipo.Domain.Interfaces.Services.Patios
{
    public interface IPatioInfraestructura
    {
        Task<IEnumerable<EPatio>> ConsultarPatiosAsync();
        Task<EPatio> ConsultarPatioPorIdAsync(Guid id);
        Task<EPatio> CrearPatioAsync(ECrearPatioDto input);
        Task<ECliente> ActualizarPatioAsync(Guid id, JsonPatchDocument<EPatio> input);
        Task<string> EliminarPatioAsync(Guid id);
    }
}
