using arquetipo.API.Controllers.Patios;
using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace arquetipo.Test.API.Patios
{
    public class PatioControllerTest
    {
        private readonly List<EPatio> _patiosSeed;

        public PatioControllerTest()
        {
            _patiosSeed = new List<EPatio>
            {
                new EPatio(Guid.NewGuid(), "Patio 1", "DIR 01", "010101", 1),
                new EPatio(Guid.NewGuid(), "Patio 2", "DIR 02", "020202", 2)
            };
        }

        #region ConsultarPatiosAsync
        [Fact]
        public async Task Should_Returns_ListaDePatios_Ok()
        {
            var patioInfraestructuraMock = new Mock<IPatioInfraestructura>();
            patioInfraestructuraMock.Setup(pi => pi.ConsultarPatiosAsync())
                .ReturnsAsync(_patiosSeed);
            var patioController = new PatioController(patioInfraestructuraMock.Object);

            var result = await patioController.ConsultarPatiosAsync();

            Assert.Equal(2, result.Count());
        }
        #endregion

        #region ConsultarPatioPorPuntoVentaAsync
        [Fact]
        public async Task Should_Returns_PatioEncontrado_Ok()
        {
            const short PUNTO_VENTA = 1;
            var patioInfraestructuraMock = new Mock<IPatioInfraestructura>();
            patioInfraestructuraMock.Setup(pi => pi.ConsultarPatioPorPuntoVentaAsync(PUNTO_VENTA))
                .ReturnsAsync(_patiosSeed.First(p => p.NumeroPuntoVenta == PUNTO_VENTA));
            var patioController = new PatioController(patioInfraestructuraMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await patioController.ConsultarPatioPorPuntoVentaAsync(PUNTO_VENTA);

            var response = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EPatio>(
                response.Value);
            Assert.Equal(PUNTO_VENTA, model.NumeroPuntoVenta);
        }

        [Fact]
        public async Task Should_Returns_AcceptedResponse_When_ConsultarPatioPorPuntoVenta_NotFound()
        {
            const short PUNTO_VENTA = 1;
            var patioInfraestructuraMock = new Mock<IPatioInfraestructura>();
            patioInfraestructuraMock.Setup(pi => pi.ConsultarPatioPorPuntoVentaAsync(PUNTO_VENTA))
                .ThrowsAsync(new CrAutoExcepcion(CrAutoErrores.PatioNoExisteError));

            var patioController = new PatioController(patioInfraestructuraMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await patioController.ConsultarPatioPorPuntoVentaAsync(PUNTO_VENTA);

            Assert.IsType<ContentResult>(result);
        }
        #endregion

    }
}
