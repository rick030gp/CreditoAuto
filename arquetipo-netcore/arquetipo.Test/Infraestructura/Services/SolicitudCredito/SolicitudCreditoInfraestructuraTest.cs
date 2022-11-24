using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Domain.Interfaces.Services.SolicitudCredito;
using arquetipo.Domain.Interfaces.Services.Vehiculos;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using arquetipo.Infrastructure.Services.SolicitudCredito;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace arquetipo.Test.Infraestructura.Services.SolicitudCredito
{
    public class SolicitudCreditoInfraestructuraTest
    {
        private readonly Mock<ISolicitudCreditoRepositorio> _solicitudCreditoRepositorioMock;
        private readonly Mock<IClienteRepositorio> _clienteRepositorioMock;
        private readonly Mock<IVehiculoRepositorio> _vehiculoRepositorioMock;
        private readonly List<ECliente> _clientesSeed;
        private readonly List<ESolicitudCredito> _solicitudesSeed;

        public SolicitudCreditoInfraestructuraTest()
        {
            _clientesSeed = new List<ECliente>
            {
                new ECliente(Guid.NewGuid(), "CL01", "NOM01", "AP01", 22, Convert.ToDateTime("12/01/2000"), "D01", "TL01", "SOLTERO", null, null, true),
                new ECliente(Guid.NewGuid(), "CL02", "NOM02", "AP02", 20, Convert.ToDateTime("12/01/2002"), "D02", "TL02", "SOLTERO", null, null, false),
            };

            _solicitudesSeed = new List<ESolicitudCredito>()
            {
                new ESolicitudCredito(
                    Guid.NewGuid(),
                    DateTime.Parse(new DateTime().ToString()),
                    _clientesSeed.First(c => c.Identificacion == "CL01").Id,
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    46,
                    500,
                    3000,
                    Guid.NewGuid())
            };

            _solicitudCreditoRepositorioMock = new Mock<ISolicitudCreditoRepositorio>();
            _clienteRepositorioMock = new Mock<IClienteRepositorio>();
            _vehiculoRepositorioMock = new Mock<IVehiculoRepositorio>();
        }

        #region CrearSolicitudCreditoAsync
        [Fact]
        public async Task Should_ThrowException_ClienteNoExiste_Al_CrearSolicitudCredito()
        {
            const string IDENTIFICACION = "CL100";
            const string PLACA_VEHICULO = "ABC01";
            ECrearSolicitudCreditoDto input = new()
            {
                FechaElaboracion = new DateTime(),
                IdentificacionCliente = IDENTIFICACION,
                PatioId = Guid.NewGuid(),
                PlacaVehiculo = PLACA_VEHICULO,
                MesesPlazo = 56,
                Cuotas = 500,
                Entrada = 5000,
                EjecutivoId = Guid.NewGuid()
            };

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .Returns(Task.FromResult<ECliente?>(null));

            var solicitudCreditoInfraestructura = new SolicitudCreditoInfraestructura(
                _solicitudCreditoRepositorioMock.Object,
                _clienteRepositorioMock.Object,
                _vehiculoRepositorioMock.Object);

            Task act() => solicitudCreditoInfraestructura.CrearSolicitudCreditoAsync(input);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.ClienteNoExisteError.Code, exception.Code);
        }

        [Fact]
        public async Task Should_ThrowException_ClienteYaTieneSolicitud_Al_CrearSolicitudCredito()
        {
            const string IDENTIFICACION = "CL01";
            const string PLACA_VEHICULO = "ABC01";
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION);
            ECrearSolicitudCreditoDto input = new()
            {
                FechaElaboracion = new DateTime(),
                IdentificacionCliente = IDENTIFICACION,
                PatioId = Guid.NewGuid(),
                PlacaVehiculo = PLACA_VEHICULO,
                MesesPlazo = 56,
                Cuotas = 500,
                Entrada = 5000,
                EjecutivoId = Guid.NewGuid()
            };

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(cliente);
            _solicitudCreditoRepositorioMock.Setup(scr => scr.ObtenerActivaPorClienteFechaAsync(
                cliente.Id, new DateTime())).ReturnsAsync(_solicitudesSeed.First());

            var solicitudCreditoInfraestructura = new SolicitudCreditoInfraestructura(
                _solicitudCreditoRepositorioMock.Object,
                _clienteRepositorioMock.Object,
                _vehiculoRepositorioMock.Object);

            Task act() => solicitudCreditoInfraestructura.CrearSolicitudCreditoAsync(input);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.ClienteYaTieneSolicitudError.Code, exception.Code);
        }
        #endregion
    }
}
