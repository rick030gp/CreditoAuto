using arquetipo.Domain.Interfaces.Services.Vehiculos;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure;
using arquetipo.Infrastructure.Exceptions;
using arquetipo.Infrastructure.Services.Vehiculos;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace arquetipo.Test.Infraestructura.Services.Vehiculos
{
    public class VehiculoInfraestructuraTest
    {
        private readonly Mock<IVehiculoRepositorio> _vehiculoRepositorioMock;
        private readonly Mock<IMarcaRepositorio> _marcaRepositorioMock;
        private readonly List<EVehiculo> _vehiculosSeed;
        private readonly List<EMarca> _marcasSeed;

        public VehiculoInfraestructuraTest()
        {
            _marcasSeed = new List<EMarca>
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
            _vehiculoRepositorioMock = new Mock<IVehiculoRepositorio>();
            _marcaRepositorioMock = new Mock<IMarcaRepositorio>();
        }


        #region ConsultarVehiculosAsync
        [Fact]
        public async void Should_ConsultarVehiculos_Ok()
        {
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerTodoAsync())
                .ReturnsAsync(_vehiculosSeed);
            var vehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            var vehiculos = await vehiculoInfraestructura.ConsultarVehiculosAsync();

            Assert.Equal(2, vehiculos.Count());
        }

        [Fact]
        public async void Should_Consultarvehiculos_ResultadoVacio_Ok()
        {
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerTodoAsync())
                .ReturnsAsync(new List<EVehiculo>());
            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            var vehiculos = await VehiculoInfraestructura.ConsultarVehiculosAsync();

            Assert.Empty(vehiculos);
        }
        #endregion

        #region ConsultarVehiculoPorPlacaAsync
        [Fact]
        public async void Should_ConsultarVehiculoPorPlaca_Ok()
        {
            const string PLACA = "ABC01";
            var vehiculoSeed = new EVehiculo(
                Guid.NewGuid(),
                PLACA,
                "Sail",
                "CH01",
                _marcasSeed.First(m => m.Nombre == "MAZDA").Id,
                null,
                1.5f,
                18000);
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(PLACA))
                .ReturnsAsync(vehiculoSeed);
            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            var vehiculo = await VehiculoInfraestructura.ConsultarVehiculoPorPlacaAsync(PLACA);

            Assert.Equal(PLACA, vehiculo.Placa);
        }

        [Fact]
        public async void Should_ThrowException_VehiculoNoExiste_Al_ConsultarVehiculoPorPlaca()
        {
            const string PLACA = "PLAAA";
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(PLACA))
                .Returns(Task.FromResult<EVehiculo?>(null));
            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            Task act() => VehiculoInfraestructura.ConsultarVehiculoPorPlacaAsync(PLACA);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.VehiculoNoExisteError.Code);
        }
        #endregion

        #region ConsultarVehiculosPorParametrosAsync
        [Fact]
        public async void Should_ConsultarVehiculo_PorModelo_Ok()
        {
            const string MODELO = "Sail";
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorParametrosAsync(null, MODELO))
                .ReturnsAsync(_vehiculosSeed.Where(v => v.Modelo == MODELO).ToList());
            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            var vehiculos = await VehiculoInfraestructura.ConsultarVehiculosPorParametrosAsync(null, MODELO);

            Assert.Equal(2, vehiculos.Count());
        }

        [Fact]
        public async void Should_ConsultarVehiculos_PorMarca_Ok()
        {
            const string MARCA = "MAZDA";
            EMarca MARCA_OBJECT = _marcasSeed.First(m => m.Nombre == MARCA);
            var vehiculoSeed = new EVehiculo(
                Guid.NewGuid(),
                "ABC01",
                "Sail",
                "CH01",
                MARCA_OBJECT.Id,
                "T1",
                1.5f,
                18000);
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorParametrosAsync(MARCA_OBJECT.Id, It.IsAny<string>()))
                .ReturnsAsync(new List<EVehiculo>() { vehiculoSeed });
            _marcaRepositorioMock.Setup(mr => mr.ObtenerPorNombreAsync(MARCA))
                .ReturnsAsync(MARCA_OBJECT);
            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            var vehiculos = await VehiculoInfraestructura.ConsultarVehiculosPorParametrosAsync(MARCA);

            Assert.Equal(MARCA_OBJECT.Id, vehiculos.First().MarcaId);
        }

        [Fact]
        public async void Should_ConsultarVehiculo_PorMarcaModelo_Ok()
        {
            const string MARCA = "MAZDA";
            EMarca MARCA_OBJECT = _marcasSeed.First(m => m.Nombre == "MAZDA");
            const string MODELO = "Sail";
            var vehiculoSeed = new EVehiculo(
                Guid.NewGuid(),
                "ABC01",
                MODELO,
                "CH01",
                MARCA_OBJECT.Id,
                null,
                1.5f,
                18000);
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorParametrosAsync(MARCA_OBJECT.Id, MODELO))
                .ReturnsAsync(new List<EVehiculo>() { vehiculoSeed });
            _marcaRepositorioMock.Setup(mr => mr.ObtenerPorNombreAsync(MARCA))
                .ReturnsAsync(MARCA_OBJECT);
            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            var vehiculos = await VehiculoInfraestructura.ConsultarVehiculosPorParametrosAsync(MARCA, MODELO);

            Assert.Equal(MARCA_OBJECT.Id, vehiculos.First().MarcaId);
            Assert.Equal(MODELO, vehiculos.First().Modelo);
        }

        [Fact]
        public async void Should_ConsultarVehiculosPorParametros_ResultadoVacio_Ok()
        {
            const string MARCA = "MAZDA";
            EMarca MARCA_OBJECT = _marcasSeed.First(m => m.Nombre == "MAZDA");
            const string MODELO = "2018";
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorParametrosAsync(MARCA_OBJECT.Id, MODELO))
                .ReturnsAsync(new List<EVehiculo>());
            _marcaRepositorioMock.Setup(mr => mr.ObtenerPorNombreAsync(MARCA))
                .ReturnsAsync(MARCA_OBJECT);
            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            var vehiculos = await VehiculoInfraestructura.ConsultarVehiculosPorParametrosAsync(MARCA, MODELO);
            Assert.Empty(vehiculos);
        }

        [Fact]
        public async void Should_ThrowException_MarcaNoExiste_Al_ConsultarVehiculosPorParametros()
        {
            ECrearVehiculoDto input = new()
            {
                Placa = "ABC01",
                Modelo = "Sail",
                Marca = "BMW",
                Cilindraje = 1.5f,
                Avaluo = 18000
            };

            _marcaRepositorioMock.Setup(mr => mr.ObtenerPorNombreAsync(input.Marca))
                .Returns(Task.FromResult<EMarca?>(null));

            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            Task act() => VehiculoInfraestructura.ConsultarVehiculosPorParametrosAsync(input.Marca, input.Modelo);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.MarcaNoExisteError.Code);
        }
        #endregion

        #region CrearVehiculoAsync
        [Fact]
        public async void Should_ThrowException_VehiculoYaExiste_Al_CrearVehiculo()
        {
            ECrearVehiculoDto input = new()
            {
                Placa = "ABC01",
                Modelo = "Sail",
                Marca = "MAZDA",
                Cilindraje = 1.5f,
                Avaluo = 18000
            };

            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(input.Placa))
                .ReturnsAsync(_vehiculosSeed.First(c => c.Placa == input.Placa));
            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            Task act() => VehiculoInfraestructura.CrearVehiculoAsync(input);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.VehiculoYaExisteError.Code);
        }

        [Fact]
        public async void Should_ThrowException_MarcaNoExiste_Al_CrearVehiculo()
        {
            ECrearVehiculoDto input = new()
            {
                Placa = "ABC10",
                Modelo = "Sail",
                Marca = "BMW",
                Cilindraje = 1.5f,
                Avaluo = 18000
            };

            _marcaRepositorioMock.Setup(mr => mr.ObtenerPorNombreAsync(input.Marca))
                .Returns(Task.FromResult<EMarca?>(null));

            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            Task act() => VehiculoInfraestructura.CrearVehiculoAsync(input);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.MarcaNoExisteError.Code);
        }

        [Fact]
        public async void Should_CrearVehiculo_Ok()
        {
            ECrearVehiculoDto input = new()
            {
                Placa = "AAA01",
                Modelo = "SUV 2018",
                Marca = "TOYOTA",
                Cilindraje = 2.0f,
                Avaluo = 22000,
                Tipo = "Manual"
            };

            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(input.Placa))
                .Returns(Task.FromResult<EVehiculo?>(null));
            _vehiculoRepositorioMock.Setup(vr => vr.InsertarAsync(It.IsAny<EVehiculo>()))
                .ReturnsAsync((EVehiculo v) => v);
            _marcaRepositorioMock.Setup(mr => mr.ObtenerPorNombreAsync(input.Marca))
                .ReturnsAsync(_marcasSeed.First(m => m.Nombre == input.Marca));

            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            EVehiculo vehiculo = await VehiculoInfraestructura.CrearVehiculoAsync(input);
            Assert.Equal(input.Placa, vehiculo.Placa);
        }
        #endregion

        #region ActualizarVehiculoAsync
        [Fact]
        public async void Should_ThrowException_ActualizacionDatosVaciosExcepcion_Al_ActualizarVehiculo()
        {
            const string PLACA = "ABC01";
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(PLACA))
                .ReturnsAsync(_vehiculosSeed.First(v => v.Placa == PLACA));

            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            JsonPatchDocument<EVehiculo> jsonPatch = new();
            Task act() => VehiculoInfraestructura.ActualizarVehiculoAsync(PLACA, jsonPatch);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.ActualizacionDatosVaciosError.Code);
        }

        [Fact]
        public async void Should_ThrowException_VehiculoNoExiste_Al_ActualizarVehiculo()
        {
            const string PLACA = "ABC011";
            const decimal NUEVO_AVALUO = 15500;

            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(PLACA))
                .Returns(Task.FromResult<EVehiculo?>(null));
            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EVehiculo>();
            jsonObject.Replace(j => j.Avaluo, NUEVO_AVALUO);
            Task act() => VehiculoInfraestructura.ActualizarVehiculoAsync(PLACA, jsonObject);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.VehiculoNoExisteError.Code);
        }

        [Fact]
        public async void Should_ThrowException_MarcaNoExiste_Al_ActualizarVehiculo()
        {
            const string PLACA = "ABC01";
            Guid NUEVA_MARCA = Guid.NewGuid();

            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(PLACA))
                .ReturnsAsync(_vehiculosSeed.First(v => v.Placa == PLACA));
            _marcaRepositorioMock.Setup(mr => mr.ObtenerPorIdAsync(NUEVA_MARCA))
                .Returns(Task.FromResult<EMarca?>(null));
            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EVehiculo>();
            jsonObject.Replace(j => j.MarcaId, NUEVA_MARCA);
            Task act() => VehiculoInfraestructura.ActualizarVehiculoAsync(PLACA, jsonObject);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.MarcaNoExisteError.Code);
        }

        [Fact]
        public async void Should_ActualizarVehiculo_Ok()
        {
            const string PLACA = "ABC01";
            const decimal NUEVO_AVALUO = 15500;

            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(PLACA))
                .ReturnsAsync(_vehiculosSeed.First(v => v.Placa == PLACA));

            _vehiculoRepositorioMock.Setup(vr => vr.ActualizarAsync(It.IsAny<EVehiculo>()))
                .ReturnsAsync((EVehiculo v) => v);

            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EVehiculo>();
            jsonObject.Replace(j => j.Avaluo, NUEVO_AVALUO);
            EVehiculo vehiculo = await VehiculoInfraestructura.ActualizarVehiculoAsync(PLACA, jsonObject);
            Assert.Equal(NUEVO_AVALUO, vehiculo.Avaluo);
        }
        #endregion

        #region EliminarVehiculoAsync
        [Fact]
        public async void Should_ThrowException_VehiculoNoExiste_Al_EliminarVehiculo()
        {
            const string PLACA = "CL10";
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(PLACA))
                .Returns(Task.FromResult<EVehiculo?>(null));

            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            Task act() => VehiculoInfraestructura.EliminarVehiculoAsync(PLACA);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.VehiculoNoExisteError.Code);
        }

        [Fact]
        public async void Should_EliminarVehiculo_Ok()
        {
            const string PLACA = "ABC01";
            _vehiculoRepositorioMock.Setup(vr => vr.ObtenerPorPlacaAsync(PLACA))
                .ReturnsAsync(_vehiculosSeed.First(v => v.Placa == PLACA));

            _vehiculoRepositorioMock.Setup(vr => vr.EliminarAsync(It.IsAny<EVehiculo>()));

            var VehiculoInfraestructura = new VehiculoInfraestructura(
                _vehiculoRepositorioMock.Object,
                _marcaRepositorioMock.Object);

            var resultado = await VehiculoInfraestructura.EliminarVehiculoAsync(PLACA);
            Assert.Equal(EConstante.VEHICULO_ELIMINADO, resultado);
        }
        #endregion

    }
}
