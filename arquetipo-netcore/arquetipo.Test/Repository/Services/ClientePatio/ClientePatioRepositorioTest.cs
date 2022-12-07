using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using arquetipo.Repository.Services.ClientePatio;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Test.Repository.Services.ClientePatio
{
    public class ClientePatioRepositorioTest
    {
        private Mock<CrAutoDbContext> _dbContextMock;
        private List<EPatio> _patiosSeed = new List<EPatio>();
        private List<ECliente> _clientesSeed = new List<ECliente>();

        [SetUp]
        public void SetUp()
        {
            _clientesSeed = new List<ECliente>
            {
                new ECliente(Guid.NewGuid(), "CL01", "NOM01", "AP01", 22, Convert.ToDateTime("12/01/2000"), "D01", "TL01", "SOLTERO"),
                new ECliente(Guid.NewGuid(), "CL02", "NOM02", "AP02", 20, Convert.ToDateTime("12/01/2002"), "D02", "TL02", "SOLTERO"),
            };

            _patiosSeed = new List<EPatio>
            {
                new EPatio(Guid.NewGuid(), "Patio 1", "DIR 01", "010101", 1),
                new EPatio(Guid.NewGuid(), "Patio 2", "DIR 02", "020202", 2)
            };

            var options = new DbContextOptionsBuilder<CrAutoDbContext>()
                .UseSqlServer(connectionString: "FakeConnectionString").Options;
            _dbContextMock = new Mock<CrAutoDbContext>(options);
        }

        #region ObtenerPorParametrosAsync
        [Test]
        public async Task Should_ObtenerPorParametros_Ok()
        {
            const short NUMERO_PUNTO_VENTA = 1;
            const string IDENTIFICACION_CLIENTE = "CL01";
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION_CLIENTE);
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == NUMERO_PUNTO_VENTA);

            var clientePatioSeed = new EClientePatio(
                Guid.NewGuid(),
                cliente.Id,
                patio.Id,
                new DateTime());
            
            var clientePatiosDbSetMock = MockUtility.CreateDbSetMock(
                new List<EClientePatio>(){clientePatioSeed}.AsQueryable());

            _dbContextMock.Setup(m => m.ClientePatios).Returns(clientePatiosDbSetMock.Object);

            var clientePatioRepositorio = new ClientePatioRepositorio(_dbContextMock.Object);
            var clientePatio = await clientePatioRepositorio.ObtenerPorParametrosAsync(cliente.Id, patio.Id);

            Assert.Multiple(() =>
            {
                Assert.That(clientePatio, Is.Not.Null);
                Assert.That(cliente.Id, Is.EqualTo(clientePatio?.ClienteId));
                Assert.That(patio.Id, Is.EqualTo(clientePatio?.PatioId));
            });
        }

        [Test]
        public async Task Should_ObtenerPorParametros_NotFound()
        {
            var clientePatiosDbSetMock = MockUtility.CreateDbSetMock(
                new List<EClientePatio>().AsQueryable());

            _dbContextMock.Setup(m => m.ClientePatios).Returns(clientePatiosDbSetMock.Object);

            var clientePatioRepositorio = new ClientePatioRepositorio(_dbContextMock.Object);
            var clientePatio = await clientePatioRepositorio.ObtenerPorParametrosAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.That(clientePatio, Is.Null);
        }
        #endregion
    }
}