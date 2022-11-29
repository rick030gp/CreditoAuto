using arquetipo.Domain.Interfaces.Services.ClientePatio;
using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure;
using arquetipo.Infrastructure.Exceptions;
using arquetipo.Infrastructure.Services.ClientePatio;
using Microsoft.AspNetCore.JsonPatch;

namespace arquetipo.Test.Infraestructura.Services.ClientePatio
{
    public class ClientePatioInfraestructuraTest
    {
        private readonly Mock<IClienteRepositorio> _clienteRepositorioMock;
        private readonly Mock<IPatioRepositorio> _patioRepositorioMock;
        private readonly Mock<IClientePatioRepositorio> _clientePatioRepositorioMock;
        private readonly List<ECliente> _clientesSeed;
        private readonly List<EPatio> _patiosSeed;

        public ClientePatioInfraestructuraTest()
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

            _clienteRepositorioMock = new Mock<IClienteRepositorio>();
            _patioRepositorioMock = new Mock<IPatioRepositorio>();
            _clientePatioRepositorioMock = new Mock<IClientePatioRepositorio>();
        }

        #region AsociarClientePatioAsync
        [Test]
        public void Should_ThrowException_ClienteNoExiste_Al_AsociarClientePatio()
        {
            const string IDENTIFICACION = "CL10";
            const short NUMERO_PUNTO_VENTA = 1;

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .Returns(Task.FromResult<ECliente?>(null));

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var input = new EAsociarClientePatioDto()
            {
                IdentificacionCliente = IDENTIFICACION,
                NumeroPuntoVenta = NUMERO_PUNTO_VENTA
            };

            Task act() => clientePatioInfraestructura.AsociarClientePatioAsync(input);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.That(exception.Code, Is.EqualTo(CrAutoErrores.ClienteNoExisteError.Code));
        }

        [Test]
        public void Should_ThrowException_PatioNoExiste_Al_AsociarClientePatio()
        {
            const string IDENTIFICACION = "CL01";
            const short NUMERO_PUNTO_VENTA = 100;

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(_clientesSeed.First(c => c.Identificacion == IDENTIFICACION));
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(NUMERO_PUNTO_VENTA))
                .Returns(Task.FromResult<EPatio?>(null));

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var input = new EAsociarClientePatioDto()
            {
                IdentificacionCliente = IDENTIFICACION,
                NumeroPuntoVenta = NUMERO_PUNTO_VENTA
            };

            Task act() => clientePatioInfraestructura.AsociarClientePatioAsync(input);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.That(exception.Code, Is.EqualTo(CrAutoErrores.PatioNoExisteError.Code));
        }

        [Test]
        public async Task Should_AsociarClientePatio_OK()
        {
            const string IDENTIFICACION = "CL01";
            const short NUMERO_PUNTO_VENTA = 1;
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION);
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == NUMERO_PUNTO_VENTA);

            EAsociarClientePatioDto input = new()
            {
                IdentificacionCliente = IDENTIFICACION,
                NumeroPuntoVenta = NUMERO_PUNTO_VENTA
            };

            var clientePatioObject = new EClientePatio(
                Guid.NewGuid(),
                cliente.Id,
                patio.Id);

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(cliente);
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(NUMERO_PUNTO_VENTA))
                .ReturnsAsync(patio);
            _clientePatioRepositorioMock.Setup(cpr => cpr.InsertarAsync(It.IsAny<EClientePatio>()))
                .ReturnsAsync((EClientePatio cp) => cp);

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var clientePatio = await clientePatioInfraestructura.AsociarClientePatioAsync(input);
            
            Assert.Multiple(() =>
            {
                Assert.That(clientePatio.ClienteId, Is.EqualTo(cliente.Id));
                Assert.That(clientePatio.PatioId, Is.EqualTo(patio.Id));
            });
        }

        [Test]
        public async Task Should_Not_AsociarClientePatio_Duplicado_OK()
        {
            const string IDENTIFICACION = "CL01";
            const short NUMERO_PUNTO_VENTA = 1;
            var cliente = _clientesSeed.First(c => c.Identificacion == IDENTIFICACION);
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == NUMERO_PUNTO_VENTA);

            EAsociarClientePatioDto input = new()
            {
                IdentificacionCliente = IDENTIFICACION,
                NumeroPuntoVenta = NUMERO_PUNTO_VENTA
            };

            var clientePatioObject = new EClientePatio(
                Guid.NewGuid(),
                cliente.Id,
                patio.Id,
                new DateTime());

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(cliente);
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(NUMERO_PUNTO_VENTA))
                .ReturnsAsync(patio);
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorParametrosAsync(cliente.Id, patio.Id))
                .ReturnsAsync(clientePatioObject);

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var clientePatio = await clientePatioInfraestructura.AsociarClientePatioAsync(input);
            
            Assert.Multiple(() =>
            {
                Assert.That(clientePatio.ClienteId, Is.EqualTo(cliente.Id));
                Assert.That(clientePatio.PatioId, Is.EqualTo(patio.Id));
            });
        }
        #endregion

        #region ActualizarAsociacionClientePatiosAsync
        [Test]
        public void Should_ThrowException_ActualizacionDatosVaciosExcepcion_Al_ActualizarAsociacionClientePatios()
        {
            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            JsonPatchDocument<EClientePatio> jsonPatch = new();
            Task act() => clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(
                Guid.NewGuid(), jsonPatch);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);
            
            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(CrAutoErrores.ActualizacionDatosVaciosError.Code, Is.EqualTo(exception.Code));
            });
        }

        [Test]
        public async Task Should_ThrowException_AsociacionClientePatioNoExiste_Al_ActualizarAsociacionClientePatio()
        {
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorIdAsync(Guid.NewGuid()))
                .Returns(Task.FromResult<EClientePatio?>(null));
            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EClientePatio>();
            jsonObject.Replace(j => j.ClienteId, Guid.NewGuid());

            Task act() => clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(
                Guid.NewGuid(), jsonObject);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.That(exception.Code, Is.EqualTo(CrAutoErrores.AsociacionClientePatioNoExiste.Code));
        }

        [Test]
        public void Should_ThrowException_ClienteNoExiste_Al_ActualizarAsociacionClientePatio()
        {
            var cliente = _clientesSeed.First(c => c.Identificacion == "CL01");
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == 1);
            EClientePatio CLIENTE_PATIO_ACTUALIZAR = new(Guid.NewGuid(), cliente.Id, patio.Id);

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdAsync(Guid.NewGuid()))
                .Returns(Task.FromResult<ECliente?>(null));
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorIdAsync(CLIENTE_PATIO_ACTUALIZAR.Id))
                .ReturnsAsync(CLIENTE_PATIO_ACTUALIZAR);

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EClientePatio>();
            jsonObject.Replace(j => j.ClienteId, Guid.NewGuid());

            Task act() => clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(
                CLIENTE_PATIO_ACTUALIZAR.Id, jsonObject);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.That(exception.Code, Is.EqualTo(CrAutoErrores.ClienteNoExisteError.Code));
        }

        [Test]
        public void Should_ThrowException_PatioNoExiste_Al_ActualizarAsociacionClientePatio()
        {
            var cliente = _clientesSeed.First(c => c.Identificacion == "CL01");
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == 1);
            EClientePatio CLIENTE_PATIO_ACTUALIZAR = new(Guid.NewGuid(), cliente.Id, patio.Id);

            _patioRepositorioMock.Setup(pr => pr.ObtenerPorIdAsync(Guid.NewGuid()))
                .Returns(Task.FromResult<EPatio?>(null));
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorIdAsync(CLIENTE_PATIO_ACTUALIZAR.Id))
                .ReturnsAsync(CLIENTE_PATIO_ACTUALIZAR);

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EClientePatio>();
            jsonObject.Replace(j => j.PatioId, Guid.NewGuid());

            Task act() => clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(
                CLIENTE_PATIO_ACTUALIZAR.Id, jsonObject);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.That(exception.Code, Is.EqualTo(CrAutoErrores.PatioNoExisteError.Code));
        }

        [Test]
        public async Task Should_ActualizarAsociacionClientePatio_Ok()
        {
            var cliente = _clientesSeed.First(c => c.Identificacion == "CL01");
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == 1);
            EClientePatio CLIENTE_PATIO_ACTUALIZAR = new(Guid.NewGuid(), cliente.Id, patio.Id);
            var clienteActualizar = new ECliente(Guid.NewGuid(), "CL05", "NOM05", "AP05", 22, Convert.ToDateTime("12/01/2000"), "D05", "TL05", "SOLTERO");
            var patioActualizar = new EPatio(Guid.NewGuid(), "Patio 5", "D05", "TL05", 5);

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdAsync(clienteActualizar.Id))
                .ReturnsAsync(clienteActualizar);
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorIdAsync(patioActualizar.Id))
                .ReturnsAsync(patioActualizar);
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorIdAsync(CLIENTE_PATIO_ACTUALIZAR.Id))
                .ReturnsAsync(CLIENTE_PATIO_ACTUALIZAR);
            _clientePatioRepositorioMock.Setup(cpr => cpr.ActualizarAsync(It.IsAny<EClientePatio>()))
                .ReturnsAsync((EClientePatio cp) => cp);

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EClientePatio>();
            jsonObject.Replace(j => j.ClienteId, clienteActualizar.Id);
            jsonObject.Replace(j => j.PatioId, patioActualizar.Id);

            var resultado = await clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(
                CLIENTE_PATIO_ACTUALIZAR.Id, jsonObject);
            
            Assert.Multiple(() =>
            {
                Assert.That(resultado.ClienteId, Is.EqualTo(clienteActualizar.Id));
                Assert.That(resultado.PatioId, Is.EqualTo(patioActualizar.Id));
            });
        }

        [Test]
        public async Task Should_Not_ActualizarAsociacionClientePatio_Duplicado_Ok()
        {
            var cliente = _clientesSeed.First(c => c.Identificacion == "CL01");
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == 1);
            EClientePatio CLIENTE_PATIO_ACTUALIZAR = new(Guid.NewGuid(), cliente.Id, patio.Id);
            var clienteActualizar = new ECliente(Guid.NewGuid(), "CL05", "NOM05", "AP05", 22, Convert.ToDateTime("12/01/2000"), "D05", "TL05", "SOLTERO");
            var patioActualizar = new EPatio(Guid.NewGuid(), "Patio 5", "D05", "TL05", 5);
            EClientePatio CLIENTE_PATIO_EXISTENTE = new(Guid.NewGuid(), clienteActualizar.Id, patioActualizar.Id);

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorIdAsync(clienteActualizar.Id))
                .ReturnsAsync(clienteActualizar);
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorIdAsync(patioActualizar.Id))
                .ReturnsAsync(patioActualizar);
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorIdAsync(CLIENTE_PATIO_ACTUALIZAR.Id))
                .ReturnsAsync(CLIENTE_PATIO_ACTUALIZAR);
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorParametrosAsync(clienteActualizar.Id, patioActualizar.Id))
                .ReturnsAsync(CLIENTE_PATIO_EXISTENTE);

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EClientePatio>();
            jsonObject.Replace(j => j.ClienteId, clienteActualizar.Id);
            jsonObject.Replace(j => j.PatioId, patioActualizar.Id);

            var resultado = await clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(
                CLIENTE_PATIO_ACTUALIZAR.Id, jsonObject);
            
            Assert.Multiple(() =>
            {
                Assert.That(resultado.ClienteId, Is.EqualTo(clienteActualizar.Id));
                Assert.That(resultado.PatioId, Is.EqualTo(patioActualizar.Id));
            });
        }
        #endregion

        #region EliminarAsociacionClientePatioAsync
        [Test]
        public void Should_ThrowException_AsociacionClientePatioNoExiste_Al_EliminarAsociacionClientePatio()
        {
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorIdAsync(Guid.NewGuid()))
                .Returns(Task.FromResult<EClientePatio?>(null));
            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            Task act() => clientePatioInfraestructura.EliminarAsociacionClientePatioAsync(Guid.NewGuid());
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.That(exception.Code, Is.EqualTo(CrAutoErrores.AsociacionClientePatioNoExiste.Code));
        }


        [Test]
        public async Task Should_EliminarAsociacionClientePatio_OK()
        {
            var cliente = _clientesSeed.First(c => c.Identificacion == "CL01");
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == 1);
            var clientePatio = new EClientePatio(
                Guid.NewGuid(),
                cliente.Id,
                patio.Id);

            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorIdAsync(clientePatio.Id))
                .ReturnsAsync(clientePatio);
            _clientePatioRepositorioMock.Setup(cpr => cpr.EliminarAsync(clientePatio));

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var resultado = await clientePatioInfraestructura.EliminarAsociacionClientePatioAsync(clientePatio.Id);

            Assert.That(resultado, Is.EqualTo(EConstante.CLIENTE_PATIO_ELIMINADO));
        }
        #endregion
    }
}
