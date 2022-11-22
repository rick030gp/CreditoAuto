using arquetipo.Domain.Interfaces.Services.Vehiculos;
using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Repository.Services.Vehiculos
{
    public class VehiculoRepositorio : GenericRepositorio<EVehiculo, Guid>, IVehiculoRepositorio
    {
        private readonly CrAutoDbContext _context;

        public VehiculoRepositorio(CrAutoDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<EVehiculo?> ObtenerPorPlacaAsync(string placa)
        {
            return await _context.Vehiculos.FirstOrDefaultAsync(v => v.Placa == placa);
        }

        public async Task<List<EVehiculo>> ObtenerPorParametrosAsync(Guid? marcaId = null, string? modelo = null)
        {
            var vehiculos = _context.Vehiculos;
            if (marcaId != null)
                await vehiculos.Where(v => v.MarcaId== marcaId).ToListAsync();

            if (string.IsNullOrEmpty(modelo))
                return await vehiculos.ToListAsync();

            return await vehiculos.Where(v => v.Modelo == modelo).ToListAsync();
        }
    }
}
