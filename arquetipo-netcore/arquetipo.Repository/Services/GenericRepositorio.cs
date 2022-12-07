using arquetipo.Domain.Interfaces.Services;
using arquetipo.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace arquetipo.Repository.Services
{
    public class GenericRepositorio<TEntity, TKey> : IGenericRepositorio<TEntity, TKey> where TEntity : class
    {
        private readonly CrAutoDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepositorio(CrAutoDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> ObtenerTodoAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity?> ObtenerPorIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity> InsertarAsync(TEntity entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task EliminarAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity> ActualizarAsync(TEntity entity)
        {
            _dbSet.Update(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
