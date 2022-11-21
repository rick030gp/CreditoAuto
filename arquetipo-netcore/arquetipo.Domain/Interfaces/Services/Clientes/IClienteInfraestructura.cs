using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace arquetipo.Domain.Interfaces.Services.Clientes
{
    public interface IClienteInfraestructura
    {
        Task<IEnumerable<ECliente>> ConsultarClientesAsync();
        Task<ECliente> ConsultarClientePorIdentificacionAsync(string identificacion);
        Task<ECliente> CrearClienteAsync(ECrearClienteDto input);
        Task<ECliente> ActualizarClienteAsync(string identificacion, JsonPatchDocument<ECliente> input);
        Task<string> EliminarClienteAsync(string identificacion);
    }
}
