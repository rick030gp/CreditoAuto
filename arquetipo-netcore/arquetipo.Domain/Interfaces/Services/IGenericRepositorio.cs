namespace arquetipo.Domain.Interfaces.Services
{
    public interface IGenericRepositorio<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> ObtenerTodoAsync();
        Task<TEntity?> ObtenerPorId(TKey id);
        Task<TEntity> InsertarAsync(TEntity entity);
        Task<TEntity> ActualizarAsync(TEntity entity);
        Task EliminarAsync(TEntity entity);
    }
}
