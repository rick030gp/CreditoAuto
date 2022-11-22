using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure;
using arquetipo.Infrastructure.Exceptions;
using arquetipo.Infrastructure.Services.Patios;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
        [Fact]
        public async void Should_ConsultarPatios_Ok()
        {
            _patioRepositorioMock.Setup(cr => cr.ObtenerTodoAsync())
                .ReturnsAsync(_patiosSeed);
            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            var patios = await patioInfraestructura.ConsultarPatiosAsync();

            Assert.Equal(2, patios.Count());
        }

        [Fact]
        public async void Should_ConsultarPatios_ResultadoVacio_Ok()
        {
            _patioRepositorioMock.Setup(cr => cr.ObtenerTodoAsync())
                .ReturnsAsync(new List<EPatio>());
            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            var patios = await patioInfraestructura.ConsultarPatiosAsync();

            Assert.Empty(patios);
        }
        #endregion

        #region ConsultarPatioPorPuntoVentaAsync
        [Fact]
        public async void Should_ConsultarPatioPorPuntoVenta_Ok()
        {
            const short PUNTO_VENTA = 1;
            var patioSeed = new EPatio(Guid.NewGuid(), "Patio 1", "DIR 01", "010101", 1);
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .ReturnsAsync(patioSeed);
            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            var patio = await patioInfraestructura.ConsultarPatioPorPuntoVentaAsync(PUNTO_VENTA);

            Assert.Equal(PUNTO_VENTA, patio.NumeroPuntoVenta);
        }

        [Fact]
        public async void Should_ThrowException_PatioNoExiste_Al_ConsultarPatioPorPuntoVenta()
        {
            const short PUNTO_VENTA = 100;
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .Returns(Task.FromResult<EPatio?>(null));
            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            Task act() => patioInfraestructura.ConsultarPatioPorPuntoVentaAsync(PUNTO_VENTA);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.PatioNoExisteError.Code);
        }
        #endregion

        #region CrearPatioAsync
        [Fact]
        public async void Should_ThrowException_PatioYaExiste_Al_CrearPatio()
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
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.PatioYaExisteError.Code);
        }

        [Fact]
        public async void Should_CrearPatio_Ok()
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
            Assert.Equal(input.NumeroPuntoVenta, patio.NumeroPuntoVenta);
        }
        #endregion

        #region ActualizarPatioAsync
        [Fact]
        public async void Should_ThrowException_ActualizacionDatosVaciosExcepcion_Al_ActualizarPatio()
        {
            const short PUNTO_VENTA = 1;
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .ReturnsAsync(_patiosSeed.First(c => c.NumeroPuntoVenta == PUNTO_VENTA));

            _patioRepositorioMock.Setup(pr => pr.ActualizarAsync(It.IsAny<EPatio>()))
                .ReturnsAsync((EPatio p) => p);

            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);
            var jsonObject = new JsonPatchDocument<EPatio>();

            Task act() => patioInfraestructura.ActualizarPatioAsync(PUNTO_VENTA, jsonObject);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.ActualizacionDatosVaciosError.Code);
        }

        [Fact]
        public async void Should_ThrowException_PatioNoExiste_Al_ActualizarPatio()
        {
            const short PUNTO_VENTA = 12;
            const string NUEVO_TELEFONO = "0999999999";
            
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .Returns(Task.FromResult<EPatio?>(null));
            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EPatio>();
            jsonObject.Replace(j => j.Telefono, NUEVO_TELEFONO);
            Task act() => patioInfraestructura.ActualizarPatioAsync(PUNTO_VENTA, jsonObject);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.PatioNoExisteError.Code);
        }

        [Fact]
        public async void Should_ActualizarPatio_Ok()
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
            Assert.Equal(NUEVO_TELEFONO, patio.Telefono);
        }
        #endregion

        #region EliminarPatioAsync
        [Fact]
        public async void Should_ThrowException_PatioNoExiste_Al_EliminarPatio()
        {
            const short PUNTO_VENTA = 99;
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .Returns(Task.FromResult<EPatio?>(null));

            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            Task act() => patioInfraestructura.EliminarPatioAsync(PUNTO_VENTA);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.PatioNoExisteError.Code);
        }

        
        [Fact]
        public async void Should_EliminarPatio_Ok()
        {
            const short PUNTO_VENTA = 1;
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorPuntoVentaAsync(PUNTO_VENTA))
                .ReturnsAsync(_patiosSeed.First(p => p.NumeroPuntoVenta == PUNTO_VENTA));

            _patioRepositorioMock.Setup(pr => pr.EliminarAsync(It.IsAny<EPatio>()));

            var patioInfraestructura = new PatioInfraestructura(_patioRepositorioMock.Object);

            var resultado = await patioInfraestructura.EliminarPatioAsync(PUNTO_VENTA);
            Assert.Equal(EConstante.PATIO_ELIMINADO, resultado);
        }
        #endregion
    }
}
