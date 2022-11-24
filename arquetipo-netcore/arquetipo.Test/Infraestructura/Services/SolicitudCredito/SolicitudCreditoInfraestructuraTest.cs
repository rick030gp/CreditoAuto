using arquetipo.Domain.Interfaces.Services.ClientePatio;
using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Domain.Interfaces.Services.SolicitudCredito;
using arquetipo.Domain.Interfaces.Services.Vehiculos;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using arquetipo.Infrastructure.Services.SolicitudCredito;
using CsvHelper;
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
        private readonly Mock<IPatioRepositorio> _patioRepositorioMock;
        private readonly Mock<IVehiculoRepositorio> _vehiculoRepositorioMock;
        private readonly Mock<IClientePatioInfraestructura> _clientePatioInfraestructuraMock;
        private readonly List<ECliente> _clientesSeed;
        private readonly List<EPatio> _patiosSeed;
        private readonly List<EVehiculo> _vehiculosSeed;
        private readonly List<ESolicitudCredito> _solicitudesSeed;

        public SolicitudCreditoInfraestructuraTest()
        {
            _clientesSeed = new List<ECliente>
            {
                new ECliente(Guid.NewGuid(), "CL01", "NOM01", "AP01", 22, Convert.ToDateTime("12/01/2000"), "D01", "TL01", "SOLTERO", null, null, true),
                new ECliente(Guid.NewGuid(), "CL02", "NOM02", "AP02", 20, Convert.ToDateTime("12/01/2002"), "D02", "TL02", "SOLTERO", null, null, false),
            };

            _patiosSeed = new List<EPatio>
            {
                new EPatio(Guid.NewGuid(), "Patio 1", "DIR 01", "010101", 1),
                new EPatio(Guid.NewGuid(), "Patio 2", "DIR 02", "020202", 2)
            };

            var marcasSeed = new List<EMarca>
            {
                new EMarca(Guid.NewGuid(), "MAZDA"),
                new EMarca(Guid.NewGuid(), "TOYOTA")
            };

            _vehiculosSeed = new List<EVehiculo>
            {
                new EVehiculo(
                    Guid.NewGuid(),
                    "ABC01",
                    "Sail",
                    "CH01",
                    marcasSeed.First(m => m.Nombre == "MAZDA").Id,
                    null,
                    1.5f,
                    18000),
                new EVehiculo(
                    Guid.NewGuid(),
                    "ABC02",
                    "Sail",
                    "CH02",
                    marcasSeed.First(m => m.Nombre == "TOYOTA").Id,
                    null,
                    1.8f,
                    20000)
            };

            _solicitudesSeed = new List<ESolicitudCredito>()
            {
                new ESolicitudCredito(
                    Guid.NewGuid(),
                    DateTime.Parse(DateTime.Now.ToString()),
                    _clientesSeed.First(c => c.Identificacion == "CL01").Id,
                    _patiosSeed.First(p => p.NumeroPuntoVenta == 1).Id,
                    _vehiculosSeed.First(v => v.Placa == "ABC01").Id,
                    46,
                    500,
                    3000,
                    Guid.NewGuid())
            };

            _solicitudCreditoRepositorioMock = new Mock<ISolicitudCreditoRepositorio>();
            _clienteRepositorioMock = new Mock<IClienteRepositorio>();
            _patioRepositorioMock = new Mock<IPatioRepositorio>();
            _vehiculoRepositorioMock = new Mock<IVehiculoRepositorio>();
            _clientePatioInfraestructuraMock = new Mock<IClientePatioInfraestructura>();
        }

        #region CrearSolicitudCreditoAsync
        [Fact]
        public async Task Should_ThrowException_ClienteNoExisteError_Al_CrearSolicitudCredito()
        {
            const string IDENTIFICACION = "CL100";
            const string PLACA_VEHICULO = "ABC01";
            const short NUMERO_PUNTO_VENTA = 1;
            ECrearSolicitudCreditoDto input = new()
            {
                FechaElaboracion = DateTime.Now,
                IdentificacionCliente = IDENTIFICACION,
                NumeroPuntoVentaPatio = NUMERO_PUNTO_VENTA,
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
                _patioRepositorioMock.Object,
                _vehiculoRepositorioMock.Object,
                _clientePatioInfraestructuraMock.Object);

            Task act() => solicitudCreditoInfraestructura.CrearSolicitudCreditoAsync(input);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.ClienteNoExisteError.Code, exception.Code);
        }

        [Fact]
        public async Task Should_ThrowException_ClienteYaTieneSolicitudError_Al_CrearSolicitudCredito()
        {
            const string IDENTIFICACION = "CL01";
            const string PLACA_VEHICULO = "ABC01";
            const short NUMERO_PUNTO_VENTA = 1;
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION);
            ECrearSolicitudCreditoDto input = new()
            {
                FechaElaboracion = DateTime.Now,
                IdentificacionCliente = IDENTIFICACION,
                NumeroPuntoVentaPatio = NUMERO_PUNTO_VENTA,
                PlacaVehiculo = PLACA_VEHICULO,
                MesesPlazo = 56,
                Cuotas = 500,
                Entrada = 5000,
                EjecutivoId = Guid.NewGuid()
            };
            var solicitudCreditoExistente = _solicitudesSeed.First(sc => sc.Estado == EstadoSolicitud.Registrada);

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(cliente);
            _solicitudCreditoRepositorioMock.Setup(scr => scr.ObtenerActivaPorClienteFechaAsync(
                cliente.Id, It.IsAny<DateTime>())).ReturnsAsync(solicitudCreditoExistente);

            var solicitudCreditoInfraestructura = new SolicitudCreditoInfraestructura(
                _solicitudCreditoRepositorioMock.Object,
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _vehiculoRepositorioMock.Object,
                _clientePatioInfraestructuraMock.Object);

            Task act() => solicitudCreditoInfraestructura.CrearSolicitudCreditoAsync(input);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.ClienteYaTieneSolicitudError.Code, exception.Code);
        }

        [Fact]
        public async Task Should_ThrowException_PatioNoExisteError_Al_CrearSolicitudCredito()
        {
            const string IDENTIFICACION = "CL01";
            const string PLACA_VEHICULO = "ABC01";
            const short NUMERO_PUNTO_VENTA = 100;
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION);
            ECrearSolicitudCreditoDto input = new()
            {
                FechaElaboracion = DateTime.Now,
                IdentificacionCliente = IDENTIFICACION,
                NumeroPuntoVentaPatio = NUMERO_PUNTO_VENTA,
                PlacaVehiculo = PLACA_VEHICULO,
                MesesPlazo = 56,
                Cuotas = 500,
                Entrada = 5000,
                EjecutivoId = Guid.NewGuid()
            };

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(cliente);
            _solicitudCreditoRepositorioMock.Setup(scr => scr.ObtenerActivaPorClienteFechaAsync(
                cliente.Id, It.IsAny<DateTime>())).Returns(Task.FromResult<ESolicitudCredito?>(null));
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaConEjecutivosAsync(NUMERO_PUNTO_VENTA))
                .Returns(Task.FromResult<EPatio?>(null));

            var solicitudCreditoInfraestructura = new SolicitudCreditoInfraestructura(
                _solicitudCreditoRepositorioMock.Object,
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _vehiculoRepositorioMock.Object,
                _clientePatioInfraestructuraMock.Object);

            Task act() => solicitudCreditoInfraestructura.CrearSolicitudCreditoAsync(input);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.PatioNoExisteError.Code, exception.Code);
        }

        [Fact]
        public async Task Should_ThrowException_EjecutivoNoDisponibleEnPatioError_Al_CrearSolicitudCredito()
        {
            const string IDENTIFICACION = "CL01";
            const string PLACA_VEHICULO = "ABC01";
            const short NUMERO_PUNTO_VENTA = 1;
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION);
            ECrearSolicitudCreditoDto input = new()
            {
                FechaElaboracion = DateTime.Now,
                IdentificacionCliente = IDENTIFICACION,
                NumeroPuntoVentaPatio = NUMERO_PUNTO_VENTA,
                PlacaVehiculo = PLACA_VEHICULO,
                MesesPlazo = 56,
                Cuotas = 500,
                Entrada = 5000,
                EjecutivoId = Guid.NewGuid()
            };

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(cliente);
            _solicitudCreditoRepositorioMock.Setup(scr => scr.ObtenerActivaPorClienteFechaAsync(
                cliente.Id, It.IsAny<DateTime>())).Returns(Task.FromResult<ESolicitudCredito?>(null));
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaConEjecutivosAsync(NUMERO_PUNTO_VENTA))
                .ReturnsAsync(_patiosSeed.First(p => p.NumeroPuntoVenta == NUMERO_PUNTO_VENTA));

            var solicitudCreditoInfraestructura = new SolicitudCreditoInfraestructura(
                _solicitudCreditoRepositorioMock.Object,
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _vehiculoRepositorioMock.Object,
                _clientePatioInfraestructuraMock.Object);

            Task act() => solicitudCreditoInfraestructura.CrearSolicitudCreditoAsync(input);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.EjecutivoNoDisponibleEnPatioError.Code, exception.Code);
        }

        [Fact]
        public async Task Should_ThrowException_VehiculoNoExisteError_Al_CrearSolicitudCredito()
        {
            const string IDENTIFICACION = "CL01";
            const string PLACA_VEHICULO = "ABC101";
            const short NUMERO_PUNTO_VENTA = 1;
            Guid EJECUTIVO_ID = Guid.NewGuid();
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION);
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == NUMERO_PUNTO_VENTA);
            ECrearSolicitudCreditoDto input = new()
            {
                FechaElaboracion = DateTime.Now,
                IdentificacionCliente = IDENTIFICACION,
                NumeroPuntoVentaPatio = NUMERO_PUNTO_VENTA,
                PlacaVehiculo = PLACA_VEHICULO,
                MesesPlazo = 56,
                Cuotas = 500,
                Entrada = 5000,
                EjecutivoId = EJECUTIVO_ID
            };

            patio.Ejecutivos = new List<EEjecutivo>()
            {
                new EEjecutivo(
                    EJECUTIVO_ID,
                    "ID EJ 01",
                    "EJ NOM 01",
                    "EJ AP 01",
                    "DIR EJ 01",
                    "TEL EJ 01",
                    "CEL EJ 01",
                    patio.Id,
                    30)
            };

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(cliente);
            _solicitudCreditoRepositorioMock.Setup(scr => scr.ObtenerActivaPorClienteFechaAsync(
                cliente.Id, It.IsAny<DateTime>())).Returns(Task.FromResult<ESolicitudCredito?>(null));
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaConEjecutivosAsync(NUMERO_PUNTO_VENTA))
                .ReturnsAsync(patio);
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(PLACA_VEHICULO))
                .Returns(Task.FromResult<EVehiculo?>(null));

            var solicitudCreditoInfraestructura = new SolicitudCreditoInfraestructura(
                _solicitudCreditoRepositorioMock.Object,
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _vehiculoRepositorioMock.Object,
                _clientePatioInfraestructuraMock.Object);

            Task act() => solicitudCreditoInfraestructura.CrearSolicitudCreditoAsync(input);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.VehiculoNoExisteError.Code, exception.Code);
        }

        [Fact]
        public async Task Should_ThrowException_VehiculoEnReservaError_Al_CrearSolicitudCredito()
        {
            const string IDENTIFICACION = "CL01";
            const string PLACA_VEHICULO = "ABC01";
            const short NUMERO_PUNTO_VENTA = 1;
            Guid EJECUTIVO_ID = Guid.NewGuid();
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION);
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == NUMERO_PUNTO_VENTA);
            var vehiculo = _vehiculosSeed.First(v => v.Placa == PLACA_VEHICULO);
            ECrearSolicitudCreditoDto input = new()
            {
                FechaElaboracion = DateTime.Now,
                IdentificacionCliente = IDENTIFICACION,
                NumeroPuntoVentaPatio = NUMERO_PUNTO_VENTA,
                PlacaVehiculo = PLACA_VEHICULO,
                MesesPlazo = 56,
                Cuotas = 500,
                Entrada = 5000,
                EjecutivoId = EJECUTIVO_ID
            };

            patio.Ejecutivos = new List<EEjecutivo>()
            {
                new EEjecutivo(
                    EJECUTIVO_ID,
                    "ID EJ 01",
                    "EJ NOM 01",
                    "EJ AP 01",
                    "DIR EJ 01",
                    "TEL EJ 01",
                    "CEL EJ 01",
                    patio.Id,
                    30)
            };

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(cliente);
            _solicitudCreditoRepositorioMock.Setup(scr => scr.ObtenerActivaPorClienteFechaAsync(
                cliente.Id, It.IsAny<DateTime>())).Returns(Task.FromResult<ESolicitudCredito?>(null));
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaConEjecutivosAsync(NUMERO_PUNTO_VENTA))
                .ReturnsAsync(patio);
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(PLACA_VEHICULO))
                .ReturnsAsync(vehiculo);
            _solicitudCreditoRepositorioMock.Setup(scr => scr.ObtenerActivaPorVehiculoAsync(vehiculo.Id))
                .ReturnsAsync(_solicitudesSeed.First(sc => sc.VehiculoId == vehiculo.Id));

            var solicitudCreditoInfraestructura = new SolicitudCreditoInfraestructura(
                _solicitudCreditoRepositorioMock.Object,
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _vehiculoRepositorioMock.Object,
                _clientePatioInfraestructuraMock.Object);

            Task act() => solicitudCreditoInfraestructura.CrearSolicitudCreditoAsync(input);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.VehiculoEnReservaError.Code, exception.Code);
        }

        [Fact]
        public async Task Should_CrearSolicitudCredito_Ok()
        {
            const string IDENTIFICACION = "CL01";
            const string PLACA_VEHICULO = "ABC01";
            const short NUMERO_PUNTO_VENTA = 1;
            Guid EJECUTIVO_ID = Guid.NewGuid();
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION);
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == NUMERO_PUNTO_VENTA);
            var vehiculo = _vehiculosSeed.First(v => v.Placa == PLACA_VEHICULO);
            ECrearSolicitudCreditoDto input = new()
            {
                FechaElaboracion = DateTime.Now,
                IdentificacionCliente = IDENTIFICACION,
                NumeroPuntoVentaPatio = NUMERO_PUNTO_VENTA,
                PlacaVehiculo = PLACA_VEHICULO,
                MesesPlazo = 56,
                Cuotas = 500,
                Entrada = 5000,
                EjecutivoId = EJECUTIVO_ID
            };

            patio.Ejecutivos = new List<EEjecutivo>()
            {
                new EEjecutivo(
                    EJECUTIVO_ID,
                    "ID EJ 01",
                    "EJ NOM 01",
                    "EJ AP 01",
                    "DIR EJ 01",
                    "TEL EJ 01",
                    "CEL EJ 01",
                    patio.Id,
                    30)
            };

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(cliente);
            _solicitudCreditoRepositorioMock.Setup(scr => scr.ObtenerActivaPorClienteFechaAsync(
                cliente.Id, It.IsAny<DateTime>())).Returns(Task.FromResult<ESolicitudCredito?>(null));
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaConEjecutivosAsync(NUMERO_PUNTO_VENTA))
                .ReturnsAsync(patio);
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(PLACA_VEHICULO))
                .ReturnsAsync(vehiculo);
            _solicitudCreditoRepositorioMock.Setup(scr => scr.ObtenerActivaPorVehiculoAsync(vehiculo.Id))
                .Returns(Task.FromResult<ESolicitudCredito?>(null));
            _solicitudCreditoRepositorioMock.Setup(scr => scr.InsertarAsync(It.IsAny<ESolicitudCredito>()))
                .ReturnsAsync((ESolicitudCredito sc) => sc);

            var solicitudCreditoInfraestructura = new SolicitudCreditoInfraestructura(
                _solicitudCreditoRepositorioMock.Object,
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _vehiculoRepositorioMock.Object,
                _clientePatioInfraestructuraMock.Object);

            var resultado = await solicitudCreditoInfraestructura.CrearSolicitudCreditoAsync(input);
            
            Assert.Equal(cliente.Id, resultado.ClienteId);
            Assert.Equal(vehiculo.Id, resultado.VehiculoId);
            Assert.Equal(EstadoSolicitud.Registrada, resultado.Estado);
        }
        #endregion
    }
}
