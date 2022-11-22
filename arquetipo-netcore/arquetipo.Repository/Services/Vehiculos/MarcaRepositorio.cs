using arquetipo.Domain.Interfaces.Services.Vehiculos;
using arquetipo.Entity.Models;
using arquetipo.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Repository.Services.Vehiculos
{
    public class MarcaRepositorio : GenericRepositorio<EMarca, Guid>, IMarcaRepositorio
    {
        private readonly CrAutoDbContext _context;

        public MarcaRepositorio(CrAutoDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<EMarca?> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.Marcas.FirstOrDefaultAsync(m => m.Nombre == nombre);
        }
    }
}
