using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using arquetipo.Repository.Services.Vehiculos;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Test.Repository.Services.Vehiculos
{
    public class VehiculoRepositorioTest
    {
        private Mock<CrAutoDbContext> _dbContextMock;
        private List<EVehiculo> _vehiculosSeed = new();

        [SetUp]
        public void SetUp()
        {
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

            var options = new DbContextOptionsBuilder<CrAutoDbContext>()
                .UseSqlServer(connectionString: "FakeConnectionString").Options;
            _dbContextMock = new Mock<CrAutoDbContext>(options);
        }

        #region ObtenerPorPlacaAsync
        [Test]
        public async Task Should_ObtenerPorPlaca_Ok()
        {
            const string PLACA = "ABC01";
            var vehiculosDbSetMock = MockUtility.CreateDbSetMock(_vehiculosSeed.AsQueryable());

            _dbContextMock.Setup(m => m.Vehiculos).Returns(vehiculosDbSetMock.Object);

            var vehiculoRepositorio = new VehiculoRepositorio(_dbContextMock.Object);
            var vehiculo = await vehiculoRepositorio.ObtenerPorPlacaAsync(PLACA);

            Assert.Multiple(() =>
            {
                Assert.That(vehiculo, Is.Not.Null);
                Assert.That(PLACA, Is.EqualTo(vehiculo?.Placa));
            });
        }

        [Test]
        public async Task Should_ObtenerPorPlaca_NotFound()
        {
            const string PLACA = "ABC100";
            var vehiculosDbSetMock = MockUtility.CreateDbSetMock(new List<EVehiculo>().AsQueryable());

            _dbContextMock.Setup(m => m.Vehiculos).Returns(vehiculosDbSetMock.Object);

            var vehiculoRepositorio = new VehiculoRepositorio(_dbContextMock.Object);
            var vehiculo = await vehiculoRepositorio.ObtenerPorPlacaAsync(PLACA);

            Assert.That(vehiculo, Is.Null);
        }
        #endregion

        #region ObtenerPorParametrosAsync
        [Test]
        public async Task Should_ObtenerPorParametros_Ok()
        {
            const string MODELO = "Sail";
            var vehiculosDbSetMock = MockUtility.CreateDbSetMock(_vehiculosSeed.AsQueryable());

            _dbContextMock.Setup(m => m.Vehiculos).Returns(vehiculosDbSetMock.Object);

            var vehiculoRepositorio = new VehiculoRepositorio(_dbContextMock.Object);
            var vehiculos = await vehiculoRepositorio.ObtenerPorParametrosAsync(null, MODELO);

            Assert.That(vehiculos.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Should_ObtenerPorParametros_NotFound()
        {
            const string MODELO = "TEST";
            var vehiculosDbSetMock = MockUtility.CreateDbSetMock(_vehiculosSeed.AsQueryable());

            _dbContextMock.Setup(m => m.Vehiculos).Returns(vehiculosDbSetMock.Object);

            var vehiculoRepositorio = new VehiculoRepositorio(_dbContextMock.Object);
            var vehiculos = await vehiculoRepositorio.ObtenerPorParametrosAsync(null, MODELO);

            Assert.That(vehiculos.Count(), Is.EqualTo(0));
        }
        #endregion
    }
}