using arquetipo.API.Controllers.Vehiculos;
using arquetipo.Domain.Interfaces.Services.Vehiculos;
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

namespace arquetipo.Test.API.Vehiculos
{
    public class VehiculoControllerTest
    {
        private readonly List<EVehiculo> _vehiculosSeed;

        public VehiculoControllerTest()
        {
            var _marcasSeed = new List<EMarca>
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
                    _marcasSeed.First(m => m.Nombre == "MAZDA").Id,
                    null,
                    1.5f,
                    18000),
                new EVehiculo(
                    Guid.NewGuid(),
                    "ABC02",
                    "Sail",
                    "CH02",
                    _marcasSeed.First(m => m.Nombre == "TOYOTA").Id,
                    null,
                    1.8f,
                    20000)
            };
        }


        #region ConsultarVehiculosAsync
        [Fact]
        public async Task Should_Returns_ListaDeVehiculos_Ok()
        {
            var vehiculoInfraestructuraMock = new Mock<IVehiculoInfraestructura>();
            vehiculoInfraestructuraMock.Setup(ci => ci.ConsultarVehiculosAsync())
                .ReturnsAsync(_vehiculosSeed);
            var vehiculoController = new VehiculoController(vehiculoInfraestructuraMock.Object);

            var result = await vehiculoController.ConsultarVehiculosAsync();

            Assert.Equal(2, result.Count());
        }
        #endregion

        #region ConsultarVehiculoPorPlacaAsync
        [Fact]
        public async Task Should_Returns_ConsultarClientePorIdentificacion_Ok()
        {
            const string PLACA = "ABC01";
            var vehiculoInfraestructuraMock = new Mock<IVehiculoInfraestructura>();
            vehiculoInfraestructuraMock.Setup(vi => vi.ConsultarVehiculoPorPlacaAsync(PLACA))
                .ReturnsAsync(_vehiculosSeed.First(v => v.Placa == PLACA));
            var vehiculoController = new VehiculoController(vehiculoInfraestructuraMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await vehiculoController.ConsultarVehiculoPorPlacaAsync(PLACA);

            var response = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EVehiculo>(
                response.Value);
            Assert.Equal(PLACA, model.Placa);
        }

        [Fact]
        public async Task Should_Returns_AcceptedResponse_When_ConsultarClientePorIdentificacion_NotFound()
        {
            const string PLACA = "AAA100";
            var vehiculoInfraestructuraMock = new Mock<IVehiculoInfraestructura>();
            vehiculoInfraestructuraMock.Setup(ci => ci.ConsultarVehiculoPorPlacaAsync(PLACA))
                .ThrowsAsync(new CrAutoExcepcion(CrAutoErrores.VehiculoNoExisteError));

            var vehiculoController = new VehiculoController(vehiculoInfraestructuraMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await vehiculoController.ConsultarVehiculoPorPlacaAsync(PLACA);

            Assert.IsType<ContentResult>(result);
        }
        #endregion

    }
}
