﻿using arquetipo.Domain.Interfaces.Services.ClientePatio;
using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Entity.DTOs;
using arquetipo.Entity.Models;
using arquetipo.Infrastructure;
using arquetipo.Infrastructure.Exceptions;
using arquetipo.Infrastructure.Services.ClientePatio;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
        [Fact]
        public async Task Should_ThrowException_ClienteNoExiste_Al_AsociarClientePatio()
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
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.ClienteNoExisteError.Code, exception.Code);
        }

        [Fact]
        public async Task Should_ThrowException_PatioNoExiste_Al_AsociarClientePatio()
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
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.PatioNoExisteError.Code, exception.Code);
        }

        [Fact]
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
            _clientePatioRepositorioMock.Setup(cpr => cpr.InsertarAsync(clientePatioObject))
                .ReturnsAsync((EClientePatio cp) => cp);

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var clientePatio = await clientePatioInfraestructura.AsociarClientePatioAsync(input);
            
            Assert.Equal(cliente.Id, clientePatio.ClienteId);
            Assert.Equal(patio.Id, clientePatio.PatioId);
        }

        [Fact]
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

            Assert.Equal(cliente.Id, clientePatio.ClienteId);
            Assert.Equal(patio.Id, clientePatio.PatioId);
        }
        #endregion

        #region ActualizarAsociacionClientePatiosAsync
        [Fact]
        public async void Should_ThrowException_ActualizacionDatosVaciosExcepcion_Al_ActualizarAsociacionClientePatios()
        {
            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            JsonPatchDocument<EClientePatio> jsonPatch = new();
            Task act() => clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(
                Guid.NewGuid(), jsonPatch);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.NotNull(exception);
            Assert.Equal(exception.Code, CrAutoErrores.ActualizacionDatosVaciosError.Code);
        }

        [Fact]
        public async Task Should_ThrowException_AsociacionClientePatioNoExiste_Al_ActualizarAsociacionClientePatio()
        {
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorId(Guid.NewGuid()))
                .Returns(Task.FromResult<EClientePatio?>(null));
            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EClientePatio>();
            jsonObject.Replace(j => j.ClienteId, Guid.NewGuid());

            Task act() => clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(
                Guid.NewGuid(), jsonObject);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.AsociacionClientePatioNoExiste.Code, exception.Code);
        }

        [Fact]
        public async Task Should_ThrowException_ClienteNoExiste_Al_ActualizarAsociacionClientePatio()
        {
            var cliente = _clientesSeed.First(c => c.Identificacion == "CL01");
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == 1);
            EClientePatio CLIENTE_PATIO_ACTUALIZAR = new(Guid.NewGuid(), cliente.Id, patio.Id);

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorId(Guid.NewGuid()))
                .Returns(Task.FromResult<ECliente?>(null));
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorId(CLIENTE_PATIO_ACTUALIZAR.Id))
                .ReturnsAsync(CLIENTE_PATIO_ACTUALIZAR);

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EClientePatio>();
            jsonObject.Replace(j => j.ClienteId, Guid.NewGuid());

            Task act() => clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(
                CLIENTE_PATIO_ACTUALIZAR.Id, jsonObject);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.ClienteNoExisteError.Code, exception.Code);
        }

        [Fact]
        public async Task Should_ThrowException_PatioNoExiste_Al_ActualizarAsociacionClientePatio()
        {
            var cliente = _clientesSeed.First(c => c.Identificacion == "CL01");
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == 1);
            EClientePatio CLIENTE_PATIO_ACTUALIZAR = new(Guid.NewGuid(), cliente.Id, patio.Id);
            
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorId(Guid.NewGuid()))
                .Returns(Task.FromResult<EPatio?>(null));
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorId(CLIENTE_PATIO_ACTUALIZAR.Id))
                .ReturnsAsync(CLIENTE_PATIO_ACTUALIZAR);

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EClientePatio>();
            jsonObject.Replace(j => j.PatioId, Guid.NewGuid());

            Task act() => clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(
                CLIENTE_PATIO_ACTUALIZAR.Id, jsonObject);
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.PatioNoExisteError.Code, exception.Code);
        }

        [Fact]
        public async Task Should_ActualizarAsociacionClientePatio_Ok()
        {
            var cliente = _clientesSeed.First(c => c.Identificacion == "CL01");
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == 1);
            EClientePatio CLIENTE_PATIO_ACTUALIZAR = new(Guid.NewGuid(), cliente.Id, patio.Id);
            var clienteActualizar = new ECliente(Guid.NewGuid(), "CL05", "NOM05", "AP05", 22, Convert.ToDateTime("12/01/2000"), "D05", "TL05", "SOLTERO");
            var patioActualizar = new EPatio(Guid.NewGuid(), "Patio 5", "D05", "TL05", 5);

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorId(clienteActualizar.Id))
                .ReturnsAsync(clienteActualizar);
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorId(patioActualizar.Id))
                .ReturnsAsync(patioActualizar);
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorId(CLIENTE_PATIO_ACTUALIZAR.Id))
                .ReturnsAsync(CLIENTE_PATIO_ACTUALIZAR);

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var jsonObject = new JsonPatchDocument<EClientePatio>();
            jsonObject.Replace(j => j.ClienteId, clienteActualizar.Id);
            jsonObject.Replace(j => j.PatioId, patioActualizar.Id);

            var resultado = await clientePatioInfraestructura.ActualizarAsociacionClientePatioAsync(
                CLIENTE_PATIO_ACTUALIZAR.Id, jsonObject);
            
            Assert.Equal(clienteActualizar.Id, resultado.ClienteId);
            Assert.Equal(patioActualizar.Id, resultado.PatioId);
        }

        [Fact]
        public async Task Should_Not_ActualizarAsociacionClientePatio_Duplicado_Ok()
        {
            var cliente = _clientesSeed.First(c => c.Identificacion == "CL01");
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == 1);
            EClientePatio CLIENTE_PATIO_ACTUALIZAR = new(Guid.NewGuid(), cliente.Id, patio.Id);
            EClientePatio CLIENTE_PATIO_EXISTENTE = new(Guid.NewGuid(), cliente.Id, patio.Id);
            var clienteActualizar = new ECliente(Guid.NewGuid(), "CL05", "NOM05", "AP05", 22, Convert.ToDateTime("12/01/2000"), "D05", "TL05", "SOLTERO");
            var patioActualizar = new EPatio(Guid.NewGuid(), "Patio 5", "D05", "TL05", 5);

            _clienteRepositorioMock.Setup(cr => cr.ObtenerPorId(clienteActualizar.Id))
                .ReturnsAsync(clienteActualizar);
            _patioRepositorioMock.Setup(pr => pr.ObtenerPorId(patioActualizar.Id))
                .ReturnsAsync(patioActualizar);
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorId(CLIENTE_PATIO_ACTUALIZAR.Id))
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

            Assert.Equal(clienteActualizar.Id, resultado.ClienteId);
            Assert.Equal(patioActualizar.Id, resultado.PatioId);
        }
        #endregion

        #region EliminarAsociacionClientePatioAsync
        [Fact]
        public async Task Should_ThrowException_AsociacionClientePatioNoExiste_Al_EliminarAsociacionClientePatio()
        {
            _clientePatioRepositorioMock.Setup(cpr => cpr.ObtenerPorId(Guid.NewGuid()))
                .Returns(Task.FromResult<EClientePatio?>(null));
            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            Task act() => clientePatioInfraestructura.EliminarAsociacionClientePatioAsync(Guid.NewGuid());
            var exception = await Assert.ThrowsAsync<CrAutoExcepcion>(act);

            Assert.Equal(CrAutoErrores.AsociacionClientePatioNoExiste.Code, exception.Code);
        }
  

        [Fact]
        public async Task Should_EliminarAsociacionClientePatio_OK()
        {
            var cliente = _clientesSeed.First(c => c.Identificacion == "CL01");
            var patio = _patiosSeed.First(p => p.NumeroPuntoVenta == 1);
            var clientePatioObject = new EClientePatio(
                Guid.NewGuid(),
                cliente.Id,
                patio.Id);

            _clientePatioRepositorioMock.Setup(cpr => cpr.EliminarAsync(clientePatioObject));

            var clientePatioInfraestructura = new ClientePatioInfraestructura(
                _clienteRepositorioMock.Object,
                _patioRepositorioMock.Object,
                _clientePatioRepositorioMock.Object);

            var resultado = await clientePatioInfraestructura.EliminarAsociacionClientePatioAsync(clientePatioObject.Id);
            
            Assert.Equal(EConstante.CLIENTE_PATIO_ELIMINADO, resultado);
        }
        #endregion
    }
}
