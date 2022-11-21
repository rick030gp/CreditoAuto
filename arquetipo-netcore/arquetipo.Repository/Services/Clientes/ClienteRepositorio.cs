using arquetipo.Domain.Interfaces.Services.Clientes;
using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Repository.Services.Clientes
{
    public class ClienteRepositorio : GenericRepositorio<ECliente, Guid>, IClienteRepositorio
    {
        private CrAutoDbContext _context;

        public ClienteRepositorio(CrAutoDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ECliente> ObtenerPorIdentificacionAsync(string identificacion)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Identificacion == identificacion);
        }
    }
}
