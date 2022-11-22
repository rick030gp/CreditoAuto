using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace arquetipo.Domain.Interfaces.Services.Patios
{
    public interface IPatioInfraestructura
    {
        Task<IEnumerable<EPatio>> ConsultarPatiosAsync();
        Task<EPatio> ConsultarPatioPorPuntoVentaAsync(short numeroPuntoVenta);
        Task<EPatio> CrearPatioAsync(ECrearPatioDto input);
        Task<EPatio> ActualizarPatioAsync(short numeroPuntoVenta, JsonPatchDocument<EPatio> input);
        Task<string> EliminarPatioAsync(short numeroPuntoVenta);
    }
}
