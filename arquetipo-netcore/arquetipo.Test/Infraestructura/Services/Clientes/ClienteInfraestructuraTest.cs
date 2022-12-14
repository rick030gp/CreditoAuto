using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure;
using arquetipo.Infrastructure.Exceptions;
using arquetipo.Infrastructure.Services.Clientes;
using Microsoft.AspNetCore.JsonPatch;

namespace arquetipo.Test.Infraestructura.Services.Clientes
{
    public class ClienteInfraestructuraTest
    {
        private readonly Mock<IClienteRepositorio> _clienteRepositorioMock;
        private readonly List<ECliente> _clientesSeed;

        public ClienteInfraestructuraTest()
        {
            _clientesSeed = new List<ECliente>
            {
                new ECliente(Guid.NewGuid(), "CL01", "NOM01", "AP01", 22, Convert.ToDateTime("12/01/2000"), "D01", "TL01", "SOLTERO"),
                new ECliente(Guid.NewGuid(), "CL02", "NOM02", "AP02", 20, Convert.ToDateTime("12/01/2002"), "D02", "TL02", "SOLTERO"),
            };
            _clienteRepositorioMock = new Mock<IClienteRepositorio>();
        }

        #region ConsultarClientesAsync
        [Test]
        public async Task Should_ConsultarClientes_Ok()
        {
            _clienteRepositorioMock.Setup(cr => cr.ObtenerTodoAsync())
                .ReturnsAsync(_clientesSeed);
            var clienteInfraestructura = new ClienteInfraestructura(
                _clienteRepositorioMock.Object);

            var clientes = await clienteInfraestructura.ConsultarClientesAsync();

            Assert.That(clientes.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Should_ConsultarClientes_ResultadoVacio_Ok()
        {
            _clienteRepositorioMock.Setup(cr => cr.ObtenerTodoAsync())
                .ReturnsAsync(new List<ECliente>());
            var clienteInfraestructura = new ClienteInfraestructura(
                _clienteRepositorioMock.Object);

            var clientes = await clienteInfraestructura.ConsultarClientesAsync();

            Assert.That(clientes, Is.Empty);
        }
        #endregion

        #region ConsultarClientePorIdentificacionAsync
        [Test]
        public async Task Should_ConsultarClientePorIdentificacion_Ok()
        {
            const string IDENTIFICACION = "CL01";
            var clienteSeed = new ECliente(Guid.NewGuid(), "CL01", "NOM01", "AP01", 22, Convert.ToDateTime("12/01/2000"), "D01", "TL01", "SOLTERO");
            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(clienteSeed);
            var clienteInfraestructura = new ClienteInfraestructura(
                _clienteRepositorioMock.Object);

            var cliente = await clienteInfraestructura.ConsultarClientePorIdentificacionAsync(IDENTIFICACION);

            Assert.That(cliente.Identificacion, Is.EqualTo(IDENTIFICACION));
        }

        [Test]
        public void Should_ThrowException_ClienteNoExiste_Al_ConsultarClientePorIdentificacion()
        {
            const string IDENTIFICACION = "CL02";
            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .Returns(Task.FromResult<ECliente?>(null));
            var clienteInfraestructura = new ClienteInfraestructura(
                _clienteRepositorioMock.Object);

            Task act() => clienteInfraestructura.ConsultarClientePorIdentificacionAsync(IDENTIFICACION);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(CrAutoErrores.ClienteNoExisteError.Code, Is.EqualTo(exception?.Code));
            });
        }
        #endregion

        #region CrearClienteAsync
        [Test]
        public void Should_ThrowException_ClienteYaExiste_Al_CrearCliente()
        {
            ECrearClienteDto input = new()
            {
                Identificacion = "CL01",
                Nombres = "NOM01",
                Apellidos = "AP01",
                Edad = 22,
                FechaNacimiento = Convert.ToDateTime("12/01/2000"),
                Direccion = "D01",
                Telefono = "TL01",
                EstadoCivil = "SOLTERO",
                EsSujetoCredito = true
            };

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(input.Identificacion))
                .ReturnsAsync(_clientesSeed.First(c => c.Identificacion == input.Identificacion));
            var clienteInfraestructura = new ClienteInfraestructura(
                _clienteRepositorioMock.Object);

            Task act() => clienteInfraestructura.CrearClienteAsync(input);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(CrAutoErrores.ClienteYaExisteError.Code, Is.EqualTo(exception?.Code));
            });
        }

        [Test]
        public async Task Should_CrearCliente_Ok()
        {
            ECrearClienteDto input = new()
            {
                Identificacion = "CL03",
                Nombres = "NOM03",
                Apellidos = "AP03",
                Edad = 23,
                FechaNacimiento = Convert.ToDateTime("12/01/1999"),
                Direccion = "D03",
                Telefono = "TL03",
                EstadoCivil = "SOLTERO",
                EsSujetoCredito = true
            };

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(input.Identificacion))
                .Returns(Task.FromResult<ECliente?>(null));
            _clienteRepositorioMock.Setup(cr => cr.InsertarAsync(It.IsAny<ECliente>()))
                .ReturnsAsync((ECliente c) => c);

            var clienteInfraestructura = new ClienteInfraestructura(
                _clienteRepositorioMock.Object);

            var cliente = await clienteInfraestructura.CrearClienteAsync(input);

            Assert.That(cliente.Identificacion, Is.EqualTo(input.Identificacion));
        }
        #endregion

        #region ActualizarClienteAsync
        [Test]
        public void Should_ThrowException_ActualizacionDatosVaciosExcepcion_Al_ActualizarCliente()
        {
            const string IDENTIFICACION = "CL01";
            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(_clientesSeed.First(c => c.Identificacion == IDENTIFICACION));

            _clienteRepositorioMock.Setup(cr => cr.ActualizarAsync(It.IsAny<ECliente>()))
                .ReturnsAsync((ECliente c) => c);

            var clienteInfraestructura = new ClienteInfraestructura(
                _clienteRepositorioMock.Object);

            JsonPatchDocument<ECliente> jsonPatch = new();
            Task act() => clienteInfraestructura.ActualizarClienteAsync(IDENTIFICACION, jsonPatch);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);
            
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(CrAutoErrores.ActualizacionDatosVaciosError.Code, Is.EqualTo(exception?.Code));
            });
        }

        [Test]
        public void Should_ThrowException_ClienteNoExiste_Al_ActualizarCliente()
        {
            const string IDENTIFICACION = "CL01";
            const string NUEVO_NOMBRE = "Juan";
            const string NUEVO_APELLIDO = "Arteaga";
            const short NUEVA_EDAD = 25;

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .Returns(Task.FromResult<ECliente?>(null));
            var clienteInfraestructura = new ClienteInfraestructura(
                _clienteRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<ECliente>();
            jsonObject.Replace(j => j.Nombres, NUEVO_NOMBRE);
            jsonObject.Replace(j => j.Apellidos, NUEVO_APELLIDO);
            jsonObject.Replace(j => j.Edad, NUEVA_EDAD);
            Task act() => clienteInfraestructura.ActualizarClienteAsync(IDENTIFICACION, jsonObject);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);
            
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(CrAutoErrores.ClienteNoExisteError.Code, Is.EqualTo(exception?.Code));
            });
        }

        [Test]
        public async Task Should_ActualizarCliente_Ok()
        {
            const string IDENTIFICACION = "CL01";
            const string NUEVO_NOMBRE = "Juan";
            const string NUEVO_APELLIDO = "Arteaga";
            const short NUEVA_EDAD = 25;
            DateTime NUEVA_FECHA_NACIMIENTO = Convert.ToDateTime("12/01/1997");

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(_clientesSeed.First(c => c.Identificacion == IDENTIFICACION));
            _clienteRepositorioMock.Setup(cr => cr.ActualizarAsync(It.IsAny<ECliente>()))
                .ReturnsAsync((ECliente c) => c);

            var clienteInfraestructura = new ClienteInfraestructura(
                _clienteRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<ECliente>();
            jsonObject.Replace(j => j.Nombres, NUEVO_NOMBRE);
            jsonObject.Replace(j => j.Apellidos, NUEVO_APELLIDO);
            jsonObject.Replace(j => j.Edad, NUEVA_EDAD);
            jsonObject.Replace(j => j.FechaNacimiento, NUEVA_FECHA_NACIMIENTO);
            var cliente = await clienteInfraestructura.ActualizarClienteAsync(IDENTIFICACION, jsonObject);
            
            Assert.Multiple(() =>
            {
                Assert.That(cliente.Nombres, Is.EqualTo(NUEVO_NOMBRE));
                Assert.That(cliente.Edad, Is.EqualTo(NUEVA_EDAD));
            });
        }
        #endregion

        #region EliminarClienteAsync
        [Test]
        public void Should_ThrowException_ClienteNoExiste_Al_EliminarCliente()
        {
            const string IDENTIFICACION = "CL10";
            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .Returns(Task.FromResult<ECliente?>(null));

            var clienteInfraestructura = new ClienteInfraestructura(
                _clienteRepositorioMock.Object);

            Task act() => clienteInfraestructura.EliminarClienteAsync(IDENTIFICACION);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);
            
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(CrAutoErrores.ClienteNoExisteError.Code, Is.EqualTo(exception?.Code));
            });
        }

        //[Test]
        //public async void Should_ThrowException_RelacionesExistentes_Al_EliminarCliente()
        //{
        //    const string IDENTIFICACION = "CL01";

        //    _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
        //        .ReturnsAsync(_clientesSeed.First(c => c.Identificacion == IDENTIFICACION));

        //    _clienteRepositorioMock.Setup(cr => cr.EliminarAsync(
        //        _clientesSeed.First(c => c.Identificacion == IDENTIFICACION)))
        //        .Throws(new DbUpdateException("conflicto con la restricción REFERENCE"));

        //    var clienteInfraestructura = new ClienteInfraestructura(
        //        _clienteRepositorioMock.Object);

        //    Task act() => clienteInfraestructura.EliminarClienteAsync(IDENTIFICACION);
        //    var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

        //    Assert.NotNull(exception);
        //    Assert.AreEqual(exception.Code, CrAutoErrores.EliminarRelacionesExistentesError.Code);
        //}

        [Test]
        public async Task Should_EliminarCliente_Ok()
        {
            const string IDENTIFICACION = "CL01";
            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(_clientesSeed.First(c => c.Identificacion == IDENTIFICACION));

            _clienteRepositorioMock.Setup(cr => cr.EliminarAsync(It.IsAny<ECliente>()));

            var clienteInfraestructura = new ClienteInfraestructura(
                _clienteRepositorioMock.Object);

            var resultado = await clienteInfraestructura.EliminarClienteAsync(IDENTIFICACION);
            
            Assert.That(resultado, Is.EqualTo(EConstante.CLIENTE_ELIMINADO));
        }
        #endregion
    }
}
