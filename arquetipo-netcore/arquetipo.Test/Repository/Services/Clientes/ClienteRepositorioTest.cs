using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using arquetipo.Repository.Services.Clientes;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Test.Repository.Services.Clientes
{
    public class ClienteRepositorioTest
    {
        private Mock<CrAutoDbContext> _dbContextMock;
        private List<ECliente> _clientesSeed = new List<ECliente>();
        private Guid _clienteIdSeed;

        [SetUp]
        public void SetUp()
        {
            _clienteIdSeed = Guid.NewGuid();
            _clientesSeed = new List<ECliente>
            {
                new ECliente(_clienteIdSeed, "CL01", "NOM01", "AP01", 22, Convert.ToDateTime("12/01/2000"), "D01", "TL01", "SOLTERO"),
                new ECliente(Guid.NewGuid(), "CL02", "NOM02", "AP02", 20, Convert.ToDateTime("12/01/2002"), "D02", "TL02", "SOLTERO"),
            };

            var options = new DbContextOptionsBuilder<CrAutoDbContext>()
                .UseSqlServer(connectionString: "FakeConnectionString").Options;
            _dbContextMock = new Mock<CrAutoDbContext>(options);
        }

        #region ObtenerTodoAsync 
        [Test]
        public async Task Should_ObtenerTodo_Ok() 
        {            
            var clientesDbSetMock = MockUtility.CreateDbSetMock(_clientesSeed.AsQueryable());
            
            _dbContextMock.Setup(dc => dc.Set<ECliente>()).Returns(clientesDbSetMock.Object);

            var clienteRepositorio = new ClienteRepositorio(_dbContextMock.Object);
            var clientes = await clienteRepositorio.ObtenerTodoAsync();
            Assert.That(clientes.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Should_ObtenerTodo_Vacio_Ok() 
        {            
            var clientesDbSetMock = MockUtility.CreateDbSetMock(new List<ECliente>().AsQueryable());
            
            _dbContextMock.Setup(dc => dc.Set<ECliente>()).Returns(clientesDbSetMock.Object);

            var clienteRepositorio = new ClienteRepositorio(_dbContextMock.Object);
            var clientes = await clienteRepositorio.ObtenerTodoAsync();
            Assert.That(clientes.Count(), Is.EqualTo(0));
        }
        #endregion

        #region ObtenerPorIdAsync
        [Test]
        public async Task Should_ObtenerPorId_Ok()
        {
            var clientesDbSetMock = MockUtility.CreateDbSetMock(_clientesSeed.AsQueryable());
            clientesDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns((object[] r) =>
            {
                return new ValueTask<ECliente?>(clientesDbSetMock.Object.FirstOrDefaultAsync(b => b.Id == (Guid) r[0]));
            });

            _dbContextMock.Setup(m => m.Set<ECliente>()).Returns(clientesDbSetMock.Object);

            var clienteRepositorio = new ClienteRepositorio(_dbContextMock.Object);
            var cliente = await clienteRepositorio.ObtenerPorIdAsync(_clienteIdSeed);

            Assert.Multiple(() =>
            {
                Assert.That(cliente, Is.Not.Null);
                Assert.That(_clienteIdSeed, Is.EqualTo(cliente?.Id));
            });
        }

        [Test]
        public async Task Should_ObtenerPorId_NotFound()
        {
            var clientesDbSetMock = MockUtility.CreateDbSetMock(_clientesSeed.AsQueryable());
            clientesDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns((object[] r) =>
            {
                return new ValueTask<ECliente?>(clientesDbSetMock.Object.FirstOrDefaultAsync(b => b.Id == (Guid) r[0]));
            });

            _dbContextMock.Setup(m => m.Set<ECliente>()).Returns(clientesDbSetMock.Object);

            var clienteRepositorio = new ClienteRepositorio(_dbContextMock.Object);
            var cliente = await clienteRepositorio.ObtenerPorIdAsync(Guid.NewGuid());

            Assert.That(cliente, Is.Null);
        }
        #endregion

        #region InsertarAsync
        [Test]
        public async Task Should_InsertarCliente_Ok()
        {
            var clientesList = new List<ECliente>();
            var clientesDbSetMock = MockUtility.CreateDbSetMock(clientesList.AsQueryable());
            clientesDbSetMock.Setup(m => m.AddAsync(It.IsAny<ECliente>(), default))
                .Callback<ECliente, CancellationToken>((c, token) =>
                {
                    clientesList.Add(c);
                }
            );

            _dbContextMock.Setup(m => m.Set<ECliente>()).Returns(clientesDbSetMock.Object);

            var clienteRepositorio = new ClienteRepositorio(_dbContextMock.Object);
            var clienteAEliminar = _clientesSeed.First();

            var resultado = await clienteRepositorio.InsertarAsync(clienteAEliminar);
            
            Assert.Multiple(() => 
            {
                Assert.That(resultado, Is.Not.Null);
                Assert.That(clienteAEliminar.Id, Is.EqualTo(resultado.Id));
            });
        }
        #endregion

        #region EliminarAsync
        [Test]
        public async Task Should_EliminarCliente_Ok()
        {
            var clientesDbSetMock = MockUtility.CreateDbSetMock(_clientesSeed.AsQueryable());
            clientesDbSetMock.Setup(m => m.Remove(It.IsAny<ECliente>()))
                .Callback<ECliente>((c) =>
                {
                    _clientesSeed.Remove(_clientesSeed.Find(cs => cs.Id == c.Id));
                }
            );

            _dbContextMock.Setup(m => m.Set<ECliente>()).Returns(clientesDbSetMock.Object);

            var clienteRepositorio = new ClienteRepositorio(_dbContextMock.Object);
            var clienteAEliminar = _clientesSeed.First();

            await clienteRepositorio.EliminarAsync(clienteAEliminar);
            
            Assert.That(_clientesSeed.Count(), Is.EqualTo(1));
        }
        #endregion

        #region  ActualizarAsync
        [Test]
        public async Task Should_ActualizarCliente_Ok()
        {
            const string NUEVA_DIRECCION = "Dirección actualizada";
            var clienteAActualizar = _clientesSeed.First(c => c.Id == _clienteIdSeed);
            clienteAActualizar.Direccion = NUEVA_DIRECCION;

            var clientesDbSetMock = MockUtility.CreateDbSetMock(_clientesSeed.AsQueryable());
            _dbContextMock.Setup(m => m.Set<ECliente>()).Returns(clientesDbSetMock.Object);

            var clienteRepositorio = new ClienteRepositorio(_dbContextMock.Object);
            var resultado = await clienteRepositorio.ActualizarAsync(clienteAActualizar);
            
            Assert.That(_clientesSeed.Count(), Is.EqualTo(2));
            Assert.That(NUEVA_DIRECCION, Is.EqualTo(resultado.Direccion));
        }
        #endregion

        #region ObtenerPorIdentificacionAsync
        [Test]
        public async Task Should_ObtenerPorIdentificacion_Ok()
        {
            const string IDENTIFICACION = "CL01";
            var clientesDbSetMock = MockUtility.CreateDbSetMock(_clientesSeed.AsQueryable());

            _dbContextMock.Setup(m => m.Clientes).Returns(clientesDbSetMock.Object);

            var clienteRepositorio = new ClienteRepositorio(_dbContextMock.Object);
            var cliente = await clienteRepositorio.ObtenerPorIdentificacionAsync(IDENTIFICACION);

            Assert.Multiple(() =>
            {
                Assert.That(cliente, Is.Not.Null);
                Assert.That(cliente?.Identificacion, Is.EqualTo(IDENTIFICACION));
            });
        }

        [Test]
        public async Task Should_ObtenerPorIdentificacion_NotFound()
        {
            const string IDENTIFICACION = "CL100";
            var clientesDbSetMock = MockUtility.CreateDbSetMock(_clientesSeed.AsQueryable());

            _dbContextMock.Setup(m => m.Clientes).Returns(clientesDbSetMock.Object);

            var clienteRepositorio = new ClienteRepositorio(_dbContextMock.Object);
            var cliente = await clienteRepositorio.ObtenerPorIdentificacionAsync(IDENTIFICACION);

            Assert.That(cliente, Is.Null);
        }
        #endregion
    }
}