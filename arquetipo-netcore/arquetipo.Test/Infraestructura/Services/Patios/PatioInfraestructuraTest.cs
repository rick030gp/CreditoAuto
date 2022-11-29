using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure;
using arquetipo.Infrastructure.Exceptions;
using arquetipo.Infrastructure.Services.Patios;
using Microsoft.AspNetCore.JsonPatch;

namespace arquetipo.Test.Infraestructura.Services.Patios
{
    public class PatioInfraestructuraTest
    {
        private readonly Mock<IPatioRepositorio> _patioRepositorioMock;
        private readonly List<EPatio> _patiosSeed;

        public PatioInfraestructuraTest()
        {
            _patiosSeed = new List<EPatio>
            {
                new EPatio(Guid.NewGuid(), "Patio 1", "DIR 01", "010101", 1),
                new EPatio(Guid.NewGuid(), "Patio 2", "DIR 02", "020202", 2)
            };

            _patioRepositorioMock = new Mock<IPatioRepositorio>();
        }

        #region ConsultarPatiosAsync
        [Test]
        public async Task Should_ConsultarPatios_Ok()
        {
            _patioRepositorioMock.Setup(cr => cr.ObtenerTodoAsync())
                .ReturnsAsync(_patiosSeed);
            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            var patios = await patioInfraestructura.ConsultarPatiosAsync();

            Assert.That(patios.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Should_ConsultarPatios_ResultadoVacio_Ok()
        {
            _patioRepositorioMock.Setup(cr => cr.ObtenerTodoAsync())
                .ReturnsAsync(new List<EPatio>());
            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            var patios = await patioInfraestructura.ConsultarPatiosAsync();

            Assert.That(patios, Is.Empty);
        }
        #endregion

        #region ConsultarPatioPorPuntoVentaAsync
        [Test]
        public async Task Should_ConsultarPatioPorPuntoVenta_Ok()
        {
            const short PUNTO_VENTA = 1;
            var patioSeed = new EPatio(Guid.NewGuid(), "Patio 1", "DIR 01", "010101", 1);
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .ReturnsAsync(patioSeed);
            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            var patio = await patioInfraestructura.ConsultarPatioPorPuntoVentaAsync(PUNTO_VENTA);

            Assert.That(patio.NumeroPuntoVenta, Is.EqualTo(PUNTO_VENTA));
        }

        [Test]
        public void Should_ThrowException_PatioNoExiste_Al_ConsultarPatioPorPuntoVenta()
        {
            const short PUNTO_VENTA = 100;
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .Returns(Task.FromResult<EPatio?>(null));
            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            Task act() => patioInfraestructura.ConsultarPatioPorPuntoVentaAsync(PUNTO_VENTA);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(CrAutoErrores.PatioNoExisteError.Code, Is.EqualTo(exception.Code));
            });
        }
        #endregion

        #region CrearPatioAsync
        [Test]
        public void Should_ThrowException_PatioYaExiste_Al_CrearPatio()
        {
            ECrearPatioDto input = new()
            {
                Nombre = "Patio 3",
                Direccion = "DIR 3",
                Telefono = "TL 3",
                NumeroPuntoVenta = 1
            };

            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(input.NumeroPuntoVenta))
                .ReturnsAsync(_patiosSeed.First(p => p.NumeroPuntoVenta == input.NumeroPuntoVenta));
            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            Task act() => patioInfraestructura.CrearPatioAsync(input);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(CrAutoErrores.PatioYaExisteError.Code, Is.EqualTo(exception.Code));
            });
        }

        [Test]
        public async Task Should_CrearPatio_Ok()
        {
            ECrearPatioDto input = new()
            {
                Nombre = "Patio 3",
                Direccion = "DIR 3",
                Telefono = "TL 3",
                NumeroPuntoVenta = 3
            };

            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(input.NumeroPuntoVenta))
                .Returns(Task.FromResult<EPatio?>(null));

            _patioRepositorioMock.Setup(pr => pr.InsertarAsync(It.IsAny<EPatio>()))
                .ReturnsAsync((EPatio p) => p);

            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            var patio = await patioInfraestructura.CrearPatioAsync(input);

            Assert.That(patio.NumeroPuntoVenta, Is.EqualTo(input.NumeroPuntoVenta));
        }
        #endregion

        #region ActualizarPatioAsync
        [Test]
        public void Should_ThrowException_ActualizacionDatosVaciosExcepcion_Al_ActualizarPatio()
        {
            const short PUNTO_VENTA = 1;
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .ReturnsAsync(_patiosSeed.First(c => c.NumeroPuntoVenta == PUNTO_VENTA));

            _patioRepositorioMock.Setup(pr => pr.ActualizarAsync(It.IsAny<EPatio>()))
                .ReturnsAsync((EPatio p) => p);

            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);
            var jsonObject = new JsonPatchDocument<EPatio>();

            Task act() => patioInfraestructura.ActualizarPatioAsync(PUNTO_VENTA, jsonObject);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(CrAutoErrores.ActualizacionDatosVaciosError.Code, Is.EqualTo(exception.Code));
            });
        }

        [Test]
        public void Should_ThrowException_PatioNoExiste_Al_ActualizarPatio()
        {
            const short PUNTO_VENTA = 12;
            const string NUEVO_TELEFONO = "0999999999";

            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .Returns(Task.FromResult<EPatio?>(null));
            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EPatio>();
            jsonObject.Replace(j => j.Telefono, NUEVO_TELEFONO);
            Task act() => patioInfraestructura.ActualizarPatioAsync(PUNTO_VENTA, jsonObject);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(CrAutoErrores.PatioNoExisteError.Code, Is.EqualTo(exception.Code));
            });
        }

        [Test]
        public async Task Should_ActualizarPatio_Ok()
        {
            const short PUNTO_VENTA = 1;
            const string NUEVO_TELEFONO = "0999999999";

            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .ReturnsAsync(_patiosSeed.First(c => c.NumeroPuntoVenta == PUNTO_VENTA));

            _patioRepositorioMock.Setup(pr => pr.ActualizarAsync(It.IsAny<EPatio>()))
                .ReturnsAsync((EPatio p) => p);

            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);
            var jsonObject = new JsonPatchDocument<EPatio>();
            jsonObject.Replace(j => j.Telefono, NUEVO_TELEFONO);

            var patio = await patioInfraestructura.ActualizarPatioAsync(PUNTO_VENTA, jsonObject);

            Assert.That(patio.Telefono, Is.EqualTo(NUEVO_TELEFONO));
        }
        #endregion

        #region EliminarPatioAsync
        [Test]
        public void Should_ThrowException_PatioNoExiste_Al_EliminarPatio()
        {
            const short PUNTO_VENTA = 99;
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .Returns(Task.FromResult<EPatio?>(null));

            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            Task act() => patioInfraestructura.EliminarPatioAsync(PUNTO_VENTA);
            var exception = Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(CrAutoErrores.PatioNoExisteError.Code, Is.EqualTo(exception.Code));
            });
        }

        [Test]
        public async Task Should_EliminarPatio_Ok()
        {
            const short PUNTO_VENTA = 1;
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .ReturnsAsync(_patiosSeed.First(p => p.NumeroPuntoVenta == PUNTO_VENTA));

            _patioRepositorioMock.Setup(pr => pr.EliminarAsync(It.IsAny<EPatio>()));

            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            var resultado = await patioInfraestructura.EliminarPatioAsync(PUNTO_VENTA);

            Assert.That(resultado, Is.EqualTo(EConstante.PATIO_ELIMINADO));
        }
        #endregion
    }
}
