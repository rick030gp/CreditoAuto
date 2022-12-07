using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using arquetipo.Repository.Services.Vehiculos;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Test.Repository.Services.Vehiculos
{
    public class MarcasRepositorioTest
    {
        private Mock<CrAutoDbContext> _dbContextMock;
        private List<EMarca> _marcasSeed = new();

        [SetUp]
        public void SetUp()
        {
            _marcasSeed = new List<EMarca>
            {
                new EMarca(Guid.NewGuid(), "MAZDA"),
                new EMarca(Guid.NewGuid(), "TOYOTA")
            };

            var options = new DbContextOptionsBuilder<CrAutoDbContext>()
                .UseSqlServer(connectionString: "FakeConnectionString").Options;
            _dbContextMock = new Mock<CrAutoDbContext>(options);
        }

        #region ObtenerPorNombreAsync
        [Test]
        public async Task Should_ObtenerPorNombre_Ok()
        {
            const string NOMBRE = "MAZDA";
            var marcasDbSetMock = MockUtility.CreateDbSetMock(_marcasSeed.AsQueryable());

            _dbContextMock.Setup(m => m.Marcas).Returns(marcasDbSetMock.Object);

            var marcaRepositorio = new MarcaRepositorio(_dbContextMock.Object);
            var marca = await marcaRepositorio.ObtenerPorNombreAsync(NOMBRE);

            Assert.Multiple(() =>
            {
                Assert.That(marca, Is.Not.Null);
                Assert.That(NOMBRE, Is.EqualTo(marca?.Nombre));
            });
        }

        [Test]
        public async Task Should_ObtenerPorNombre_NotFound()
        {
            const string NOMBRE = "BMW";
            var marcasDbSetMock = MockUtility.CreateDbSetMock(_marcasSeed.AsQueryable());

            _dbContextMock.Setup(m => m.Marcas).Returns(marcasDbSetMock.Object);

            var marcaRepositorio = new MarcaRepositorio(_dbContextMock.Object);
            var marca = await marcaRepositorio.ObtenerPorNombreAsync(NOMBRE);

            Assert.That(marca, Is.Null);
        }
        #endregion
    }
}