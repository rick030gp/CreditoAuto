using arquetipo.Domain.Interfaces.Services;
using arquetipo.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Repository.Services
{
    public class GenericRepositorio<TEntity, TKey> : IGenericRepositorio<TEntity, TKey> where TEntity : class
    {
        private readonly CrAutoDbContext _context;

        public GenericRepositorio(CrAutoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> ObtenerTodoAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> ObtenerPorId(TKey id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity> InsertarAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task EliminarAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity> ActualizarAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
