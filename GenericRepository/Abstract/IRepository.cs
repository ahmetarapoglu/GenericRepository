using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace GenericRepository.Abstract
{
    public interface IRepository<TEntity> where TEntity : class
    {

        Task<(List<TModel> Data, int Total)> GetListAndTotalAsync<TModel>(
            Func<IQueryable<TEntity>, IQueryable<TModel>> select,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? skip = null, int? take = null);

        Task<(List<TEntity> Data, int Total)> GetListAndTotalAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? skip = null, int? take = null);

        Task<List<TModel>> GetListAsync<TModel>(
            Func<IQueryable<TEntity>, IQueryable<TModel>> select,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? skip = null, int? take = null);

        Task<List<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? skip = null, int? take = null);


        Task<TEntity?> FindAsync(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

        Task<TModel?> FindAsync<TModel>(
            Func<IQueryable<TEntity>, IQueryable<TModel>> select,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);



        Task<TEntity> AddAsync(TEntity entity);
        Task AddRangeAsync(List<TEntity> entities);


        Task UpdateAsync(Action<TEntity> action, Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
        Task UpdateRangeAsync(List<TEntity> entities);


        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(Expression<Func<TEntity, bool>> filter);
        Task DeleteRangeAsync(List<TEntity> entities);
        Task DeleteRangeAsync(Expression<Func<TEntity, bool>> filter);
    }
}
