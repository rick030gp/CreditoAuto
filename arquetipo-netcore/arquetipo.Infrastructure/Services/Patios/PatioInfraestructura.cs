using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Infrastructure.Services.Patios
{
    public class PatioInfraestructura : IPatioInfraestructura
    {
        private readonly IPatioRepositorio _patioRepositorio;

        public PatioInfraestructura(IPatioRepositorio patioRepositorio)
        {
            _patioRepositorio = patioRepositorio;
        }

        public async Task<EPatio> ActualizarPatioAsync(short numeroPuntoVenta, JsonPatchDocument<EPatio> input)
        {
            if (!input.Operations.Any())
                throw new CrAutoExcepcion(CrAutoErrores.ActualizacionDatosVaciosError);

            var patio = await _patioRepositorio.ObtenerPorPuntoVentaAsync(numeroPuntoVenta);
            if (patio == null)
                throw new CrAutoExcepcion(CrAutoErrores.PatioNoExisteError);

            input.ApplyTo(patio);

            return await _patioRepositorio.ActualizarAsync(patio);
        }

        public async Task<EPatio> ConsultarPatioPorPuntoVentaAsync(short numeroPuntoVenta)
        {
            var patio = await _patioRepositorio.ObtenerPorPuntoVentaAsync(numeroPuntoVenta);
            if (patio == null)
                throw new CrAutoExcepcion(CrAutoErrores.PatioNoExisteError);

            return patio;
        }

        public async Task<IEnumerable<EPatio>> ConsultarPatiosAsync()
        {
            return await _patioRepositorio.ObtenerTodoAsync();
        }

        public async Task<EPatio> CrearPatioAsync(ECrearPatioDto input)
        {
            var patioExistente = await _patioRepositorio.ObtenerPorPuntoVentaAsync(input.NumeroPuntoVenta);
            if (patioExistente != null)
                throw new CrAutoExcepcion(CrAutoErrores.PatioYaExisteError);

            var patio = new EPatio(
                Guid.NewGuid(),
                input.Nombre,
                input.Direccion,
                input.Telefono,
                input.NumeroPuntoVenta);

            return await _patioRepositorio.InsertarAsync(patio);
        }

        public async Task<string> EliminarPatioAsync(short numeroPuntoVenta)
        {
            var patio = await _patioRepositorio.ObtenerPorPuntoVentaAsync(numeroPuntoVenta);
            if (patio == null)
                throw new CrAutoExcepcion(CrAutoErrores.PatioNoExisteError);

            try
            {
                await _patioRepositorio.EliminarAsync(patio);
                return EConstante.PATIO_ELIMINADO;
            }
            catch (DbUpdateException ex)
            {
                throw new CrAutoExcepcion(CrAutoErrores.EliminarRelacionesExistentesError, ex);
            }
        }
    }
}
