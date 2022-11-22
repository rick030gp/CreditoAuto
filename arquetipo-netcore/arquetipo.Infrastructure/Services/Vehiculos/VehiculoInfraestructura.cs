using arquetipo.Domain.Interfaces.Services.Vehiculos;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.JsonPatch;

namespace arquetipo.Infrastructure.Services.Vehiculos
{
    public class VehiculoInfraestructura : IVehiculoInfraestructura
    {
        private readonly IVehiculoRepositorio _vehiculoRepositorio;
        private readonly IMarcaRepositorio _marcaRepositorio;

        public VehiculoInfraestructura(IVehiculoRepositorio vehiculoRepositorio, IMarcaRepositorio marcaRepositorio)
        {
            _vehiculoRepositorio = vehiculoRepositorio;
            _marcaRepositorio = marcaRepositorio;
        }

        public async Task<EVehiculo> ActualizarVehiculoAsync(string placa, JsonPatchDocument<EVehiculo> input)
        {
            if (!input.Operations.Any())
                throw new CrAutoExcepcion(CrAutoErrores.ActualizacionDatosVaciosError);

            var vehiculo = await _vehiculoRepositorio.ObtenerPorPlacaAsync(placa);
            if (vehiculo == null)
                throw new CrAutoExcepcion(CrAutoErrores.VehiculoNoExisteError);

            var operacionMarca = input.Operations.FirstOrDefault(o => o.path.ToLower() == $"/{nameof(vehiculo.MarcaId)}".ToLower());
            if (operacionMarca != null)
            {
                var marcaId = Guid.Parse(operacionMarca.value.ToString());
                var marca = await _marcaRepositorio.ObtenerPorId(marcaId);
                if (marca == null)
                    throw new CrAutoExcepcion(CrAutoErrores.MarcaNoExisteError);
            }

            input.ApplyTo(vehiculo);

            return await _vehiculoRepositorio.ActualizarAsync(vehiculo);
        }

        public async Task<EVehiculo> ConsultarVehiculoPorPlacaAsync(string placa)
        {
            var vehiculo = await _vehiculoRepositorio.ObtenerPorPlacaAsync(placa);
            if (vehiculo == null)
                throw new CrAutoExcepcion(CrAutoErrores.VehiculoNoExisteError);

            return vehiculo;
        }

        public async Task<IEnumerable<EVehiculo>> ConsultarVehiculosAsync()
        {
            return await _vehiculoRepositorio.ObtenerTodoAsync();
        }

        public async Task<IEnumerable<EVehiculo>> ConsultarVehiculosPorParametrosAsync(string? marca = null, string? modelo = null)
        {
            if (string.IsNullOrEmpty(marca))
                return await _vehiculoRepositorio.ObtenerPorParametrosAsync(null, modelo);

            var marcaObjeto = await _marcaRepositorio.ObtenerPorNombreAsync(marca);
            if (marcaObjeto == null)
                throw new CrAutoExcepcion(CrAutoErrores.MarcaNoExisteError);

            return await _vehiculoRepositorio.ObtenerPorParametrosAsync(marcaObjeto?.Id, modelo);            
        }

        public async Task<EVehiculo> CrearVehiculoAsync(ECrearVehiculoDto input)
        {
            var vehiculoExistente = await _vehiculoRepositorio.ObtenerPorPlacaAsync(input.Placa);
            if (vehiculoExistente != null)
                throw new CrAutoExcepcion(CrAutoErrores.VehiculoYaExisteError);

            var marca = await _marcaRepositorio.ObtenerPorNombreAsync(input.Marca);
            if (marca == null)
                throw new CrAutoExcepcion(CrAutoErrores.MarcaNoExisteError);

            var vehiculo = new EVehiculo(
                Guid.NewGuid(),
                input.Placa,
                input.Modelo,
                input.NumeroChasis,
                marca.Id,
                input.Tipo,
                input.Cilindraje,
                input.Avaluo);

            return await _vehiculoRepositorio.InsertarAsync(vehiculo);
        }

        public async Task<string> EliminarVehiculoAsync(string placa)
        {
            var vehiculo = await _vehiculoRepositorio.ObtenerPorPlacaAsync(placa);
            if (vehiculo == null)
                throw new CrAutoExcepcion(CrAutoErrores.VehiculoNoExisteError);

            try
            {
                await _vehiculoRepositorio.EliminarAsync(vehiculo);
                return EConstante.VEHICULO_ELIMINADO;
            }
            catch (Exception ex)
            {
                throw new CrAutoExcepcion(CrAutoErrores.EliminarRelacionesExistentesError, ex);
            }
        }
    }
}
