﻿using arquetipo.API.Controllers.Clients;
using arquetipo.Domain.Interfaces.Services.Clientes;
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

namespace arquetipo.Test.API
{
    public class ClienteControllerTest
    {
        private readonly List<ECliente> _clientesSeed;

        public ClienteControllerTest()
        {
            _clientesSeed = new List<ECliente>
            {
                new ECliente(Guid.NewGuid(), "CL01", "NOM01", "AP01", 22, Convert.ToDateTime("12/01/2000"), "D01", "TL01", "SOLTERO", null, null, true),
                new ECliente(Guid.NewGuid(), "CL02", "NOM02", "AP02", 20, Convert.ToDateTime("12/01/2002"), "D02", "TL02", "SOLTERO", null, null, false),
            };
        }

        #region ConsultarClientesAsync
        [Fact]
        public async Task Should_Returns_ListaDeClientes_Ok()
        {
            var clienteInfraestructuraMock = new Mock<IClienteInfraestructura>();
            clienteInfraestructuraMock.Setup(ci => ci.ConsultarClientesAsync())
                .ReturnsAsync(_clientesSeed);
            var clienteController = new ClienteController(clienteInfraestructuraMock.Object);

            var result = await clienteController.ConsultarClientesAsync();

            Assert.Equal(2, result.Count());
        }
        #endregion

        #region ConsultarClientePorIdentificacionAsync
        [Fact]
        public async Task Should_Returns_ConsultarClientePorIdentificacion_Ok()
        {
            const string IDENTIFICACION = "CL01";
            var clienteInfraestructuraMock = new Mock<IClienteInfraestructura>();
            clienteInfraestructuraMock.Setup(ci => ci.ConsultarClientePorIdentificacionAsync(IDENTIFICACION))
                .ReturnsAsync(_clientesSeed.First(c => c.Identificacion == IDENTIFICACION));
            var clienteController = new ClienteController(clienteInfraestructuraMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await clienteController.ConsultarClientePorIdentificacionAsync(IDENTIFICACION);

            var response = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ECliente>(
                response.Value);
            Assert.Equal(IDENTIFICACION, model.Identificacion);
        }

        [Fact]
        public async Task Should_Returns_AcceptedResponse_When_ConsultarClientePorIdentificacion_NotFound()
        {
            const string IDENTIFICACION = "CL99";
            var clienteInfraestructuraMock = new Mock<IClienteInfraestructura>();
            clienteInfraestructuraMock.Setup(ci => ci.ConsultarClientePorIdentificacionAsync(IDENTIFICACION))
                .ThrowsAsync(new CrAutoExcepcion(CrAutoErrores.ClienteNoExisteError));

            var clienteController = new ClienteController(clienteInfraestructuraMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await clienteController.ConsultarClientePorIdentificacionAsync(IDENTIFICACION);

            Assert.IsType<ContentResult>(result);
        }
        #endregion

    }
}
