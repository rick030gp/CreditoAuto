using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using arquetipo.Repository.Services.Patios;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Test.Repository.Services.Patios
{
    public class PatioRepositorioTest
    {
        private Mock<CrAutoDbContext> _dbContextMock;
        private List<EPatio> _patiosSeed = new List<EPatio>();

        [SetUp]
        public void SetUp()
        {
            _patiosSeed = new List<EPatio>
            {
                new EPatio(Guid.NewGuid(), "Patio 1", "DIR 01", "010101", 1),
                new EPatio(Guid.NewGuid(), "Patio 2", "DIR 02", "020202", 2)
            };

            var options = new DbContextOptionsBuilder<CrAutoDbContext>()
                .UseSqlServer(connectionString: "FakeConnectionString").Options;
            _dbContextMock = new Mock<CrAutoDbContext>(options);
        }

        #region ObtenerPorPuntoVentaAsync
        [Test]
        public async Task Should_ObtenerPorPuntoVenta_Ok()
        {
            const short NUMERO_PUNTO_VENTA = 1;
            var patiosDbSetMock = MockUtility.CreateDbSetMock(_patiosSeed.AsQueryable());

            _dbContextMock.Setup(m => m.Patios).Returns(patiosDbSetMock.Object);

            var patioRepositorio = new PatioRepositorio(_dbContextMock.Object);
            var patio = await patioRepositorio.ObtenerPorPuntoVentaAsync(NUMERO_PUNTO_VENTA);

            Assert.Multiple(() =>
            {
                Assert.That(patio, Is.Not.Null);
                Assert.That(NUMERO_PUNTO_VENTA, Is.EqualTo(patio?.NumeroPuntoVenta));
            });
        }

        [Test]
        public async Task Should_ObtenerPorPuntoVenta_NotFound()
        {
            const short NUMERO_PUNTO_VENTA = 10;
            var patiosDbSetMock = MockUtility.CreateDbSetMock(_patiosSeed.AsQueryable());

            _dbContextMock.Setup(m => m.Patios).Returns(patiosDbSetMock.Object);

            var patioRepositorio = new PatioRepositorio(_dbContextMock.Object);
            var patio = await patioRepositorio.ObtenerPorPuntoVentaAsync(NUMERO_PUNTO_VENTA);

            Assert.That(patio, Is.Null);
        }
        #endregion

        #region ObtenerPorPuntoVentaConEjecutivosAsync
        [Test]
        public async Task Should_ObtenerPorPuntoVentaConEjecutivos_Without_Ejecutivos_Ok()
        {
            const short NUMERO_PUNTO_VENTA = 1;
            var patiosDbSetMock = MockUtility.CreateDbSetMock(_patiosSeed.AsQueryable());

            _dbContextMock.Setup(m => m.Patios).Returns(patiosDbSetMock.Object);

            var patioRepositorio = new PatioRepositorio(_dbContextMock.Object);
            var patio = await patioRepositorio.ObtenerPorPuntoVentaConEjecutivosAsync(NUMERO_PUNTO_VENTA);

            Assert.Multiple(() =>
            {
                Assert.That(patio, Is.Not.Null);
                Assert.That(NUMERO_PUNTO_VENTA, Is.EqualTo(patio?.NumeroPuntoVenta));
            });
        }

        [Test]
        public async Task Should_ObtenerPorPuntoVentaConEjecutivos_With_Ejecutivos_Ok()
        {
            const short NUMERO_PUNTO_VENTA = 1;
            var patioSeed = _patiosSeed.First(p => p.NumeroPuntoVenta == NUMERO_PUNTO_VENTA);
            patioSeed.Ejecutivos.Add(
                new EEjecutivo(
                    Guid.NewGuid(),
                    "EJ01",
                    "Ej 01",
                    "Ej 01",
                    "Ej 01",
                    "Ej01",
                    "Ej 01",
                    patioSeed.Id,
                    12)
            );

            var patiosDbSetMock = MockUtility.CreateDbSetMock(_patiosSeed.AsQueryable());
            _dbContextMock.Setup(m => m.Patios).Returns(patiosDbSetMock.Object);

            var patioRepositorio = new PatioRepositorio(_dbContextMock.Object);
            var patio = await patioRepositorio.ObtenerPorPuntoVentaConEjecutivosAsync(NUMERO_PUNTO_VENTA);

            Assert.Multiple(() =>
            {
                Assert.That(patio, Is.Not.Null);
                Assert.That(NUMERO_PUNTO_VENTA, Is.EqualTo(patio?.NumeroPuntoVenta));
                Assert.That(patio?.Ejecutivos.Count(), Is.EqualTo(1));
            });
        }

        [Test]
        public async Task Should_ObtenerPorPuntoVentaConEjecutivos_NotFound()
        {
            const short NUMERO_PUNTO_VENTA = 10;
            var patiosDbSetMock = MockUtility.CreateDbSetMock(_patiosSeed.AsQueryable());

            _dbContextMock.Setup(m => m.Patios).Returns(patiosDbSetMock.Object);

            var patioRepositorio = new PatioRepositorio(_dbContextMock.Object);
            var patio = await patioRepositorio.ObtenerPorPuntoVentaConEjecutivosAsync(NUMERO_PUNTO_VENTA);

            Assert.That(patio, Is.Null);
        }
        #endregion
    }
}