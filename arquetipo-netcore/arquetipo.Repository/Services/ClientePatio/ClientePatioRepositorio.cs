using arquetipo.Domain.Interfaces.Services.ClientePatio;
using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Repository.Services.ClientePatio
{
    public class ClientePatioRepositorio : GenericRepositorio<EClientePatio, Guid>, IClientePatioRepositorio
    {
        private readonly CrAutoDbContext _context;

        public ClientePatioRepositorio(CrAutoDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<EClientePatio?> ObtenerPorParametrosAsync(Guid clienteId, Guid patioId)
        {
            return await _context.ClientePatios.FirstOrDefaultAsync(cp => cp.ClienteId == clienteId && cp.PatioId == patioId);
        }
    }
}
