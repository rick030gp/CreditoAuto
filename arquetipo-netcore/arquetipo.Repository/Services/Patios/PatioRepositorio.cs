using arquetipo.Domain.Interfaces.Services.Patios;
using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Repository.Services.Patios
{
    public class PatioRepositorio : GenericRepositorio<EPatio, Guid>, IPatioRepositorio
    {
        private readonly CrAutoDbContext _context;

        public PatioRepositorio(CrAutoDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<EPatio?> ObtenerPorPuntoVentaAsync(short numeroPuntoVenta)
        {
            return await _context.Patios.FirstOrDefaultAsync(p => p.NumeroPuntoVenta == numeroPuntoVenta);
        }

        public async Task<EPatio?> ObtenerPorPuntoVentaConEjecutivosAsync(short numeroPuntoVenta)
        {
            return await _context.Patios.Include(p => p.Ejecutivos)
                .FirstOrDefaultAsync(p => p.NumeroPuntoVenta == numeroPuntoVenta);
        }
    }
}
