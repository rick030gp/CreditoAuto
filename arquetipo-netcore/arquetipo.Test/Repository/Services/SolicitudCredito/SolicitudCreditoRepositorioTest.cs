using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using arquetipo.Repository.Services.SolicitudCredito;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Test.Repository.Services.SolicitudCredito
{
    public class SolicitudCreditoRepositorioTest
    {
        private Mock<CrAutoDbContext> _dbContextMock;
        private List<ECliente> _clientesSeed = new();
        private List<EPatio> _patiosSeed = new();
        private List<EVehiculo> _vehiculosSeed = new();
        private List<ESolicitudCredito> _solicitudesSeed = new();

        [SetUp]
        public void SetUp()
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

            var options = new DbContextOptionsBuilder<CrAutoDbContext>()
                .UseSqlServer(connectionString: "FakeConnectionString").Options;
            _dbContextMock = new Mock<CrAutoDbContext>(options);
        }

        #region ObtenerActivaPorVehiculoAsync
        [Test]
        public async Task Should_ObtenerActivaPorVehiculo_Ok()
        {
            const string IDENTIFICACION_CLIENTE = "CL01";
            const string PLACA_VEHICULO = "ABC01";
            const short NUMERO_PUNTO_VENTA = 1;
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION_CLIENTE);
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == NUMERO_PUNTO_VENTA);
            var vehiculo = _vehiculosSeed.First(v => v.Placa == PLACA_VEHICULO);
            var fechaActual = DateTime.Now;

            var solicitudSeed = new ESolicitudCredito(
                Guid.NewGuid(),
                fechaActual,
                cliente.Id,
                patio.Id,
                vehiculo.Id,
                36,
                300,
                3000,
                Guid.NewGuid()
            );
            
            var solicitudesDbSetMock = MockUtility.CreateDbSetMock(
                new List<ESolicitudCredito>(){solicitudSeed}.AsQueryable());

            _dbContextMock.Setup(m => m.SolicitudesCredito).Returns(solicitudesDbSetMock.Object);

            var solicitudCreditoRepositorio = new SolicitudCreditoRepositorio(_dbContextMock.Object);
            var solicitud = await solicitudCreditoRepositorio.ObtenerActivaPorVehiculoAsync(vehiculo.Id);

            Assert.Multiple(() =>
            {
                Assert.That(solicitud, Is.Not.Null);
                Assert.That(vehiculo.Id, Is.EqualTo(solicitud?.VehiculoId));
                Assert.That(EstadoSolicitud.Registrada, Is.EqualTo(solicitud?.Estado));
            });
        }

        [Test]
        public async Task Should_ObtenerActivaPorVehiculo_NotFound()
        {
            var solicitudesDbSetMock = MockUtility.CreateDbSetMock(
                new List<ESolicitudCredito>().AsQueryable());

            _dbContextMock.Setup(m => m.SolicitudesCredito).Returns(solicitudesDbSetMock.Object);

            var solicitudCreditoRepositorio = new SolicitudCreditoRepositorio(_dbContextMock.Object);
            var solicitud = await solicitudCreditoRepositorio.ObtenerActivaPorVehiculoAsync(Guid.NewGuid());

            Assert.That(solicitud, Is.Null);
        }
        #endregion

        #region ObtenerActivaPorClienteFechaAsync
        [Test]
        public async Task Should_ObtenerActivaPorClienteFecha_Ok()
        {
            const string IDENTIFICACION_CLIENTE = "CL01";
            const string PLACA_VEHICULO = "ABC01";
            const short NUMERO_PUNTO_VENTA = 1;
            var fechaActual = DateTime.Now;
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION_CLIENTE);
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == NUMERO_PUNTO_VENTA);
            var vehiculo = _vehiculosSeed.First(v => v.Placa == PLACA_VEHICULO);

            var solicitudSeed = new ESolicitudCredito(
                Guid.NewGuid(),
                fechaActual,
                cliente.Id,
                patio.Id,
                vehiculo.Id,
                36,
                300,
                3000,
                Guid.NewGuid()
            );
            
            var solicitudesDbSetMock = MockUtility.CreateDbSetMock(
                new List<ESolicitudCredito>(){solicitudSeed}.AsQueryable());

            _dbContextMock.Setup(m => m.SolicitudesCredito).Returns(solicitudesDbSetMock.Object);

            var solicitudCreditoRepositorio = new SolicitudCreditoRepositorio(_dbContextMock.Object);
            var solicitud = await solicitudCreditoRepositorio.ObtenerActivaPorClienteFechaAsync(
                cliente.Id,
                fechaActual);

            Assert.Multiple(() =>
            {
                Assert.That(solicitud, Is.Not.Null);
                Assert.That(cliente.Id, Is.EqualTo(solicitud?.ClienteId));
                Assert.That(EstadoSolicitud.Registrada, Is.EqualTo(solicitud?.Estado));
            });
        }

        [Test]
        public async Task Should_ObtenerActivaPorClienteFecha_NotFound()
        {
            const string IDENTIFICACION_CLIENTE = "CL01";
            const string PLACA_VEHICULO = "ABC01";
            const short NUMERO_PUNTO_VENTA = 1;
            var fechaActual = DateTime.Now;
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION_CLIENTE);
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == NUMERO_PUNTO_VENTA);
            var vehiculo = _vehiculosSeed.First(v => v.Placa == PLACA_VEHICULO);

            var solicitudSeed = new ESolicitudCredito(
                Guid.NewGuid(),
                new DateTime(2020, 01, 13),
                cliente.Id,
                patio.Id,
                vehiculo.Id,
                36,
                300,
                3000,
                Guid.NewGuid()
            );
            
            var solicitudesDbSetMock = MockUtility.CreateDbSetMock(
                new List<ESolicitudCredito>(){solicitudSeed}.AsQueryable());

            _dbContextMock.Setup(m => m.SolicitudesCredito).Returns(solicitudesDbSetMock.Object);

            var solicitudCreditoRepositorio = new SolicitudCreditoRepositorio(_dbContextMock.Object);
            var solicitud = await solicitudCreditoRepositorio.ObtenerActivaPorClienteFechaAsync(
                cliente.Id,
                fechaActual);

            Assert.That(solicitud, Is.Null);
        }
        #endregion
    }
}