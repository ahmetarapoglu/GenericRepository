using GenericRepository.Abstract;
using GenericRepository.DbContext;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;

namespace GenericRepository.Concreate
{
    public class GenericRepository<TEntity>(AppDbContext context) : IRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context = context 
                                               ?? throw new ArgumentNullException(nameof(context));
        private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

        #region GetList

        public async Task<(List<TModel> Data, int Total)> GetListAndTotalAsync<TModel>(
            Func<IQueryable<TEntity>, IQueryable<TModel>> select,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? skip = null, int? take = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking().AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            int total = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            IQueryable<TModel> model = select(query);

            var data = await model.ToListAsync();

            return (data, total);
        }

        public async Task<(List<TEntity> Data, int Total)> GetListAndTotalAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? skip = null, int? take = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking().AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            int total = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            var data = await query.ToListAsync();

            return (data, total);
        }

        public async Task<List<TModel>> GetListAsync<TModel>(
            Func<IQueryable<TEntity>, IQueryable<TModel>> select,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? skip = null, int? take = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking().AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            IQueryable<TModel> model = select(query);

            var data = await model.ToListAsync();

            return data;
        }

        public async Task<List<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? skip = null, int? take = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking().AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            var data = await query.ToListAsync();

            return data;
        }


        #endregion

        #region GetOne

        public async Task<TEntity?> FindAsync(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            try
            {
                IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking().AsQueryable();

                if (include != null)
                {
                    query = include(query);
                }

                var data = await query.FirstOrDefaultAsync(filter);

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TModel?> FindAsync<TModel>(
            Func<IQueryable<TEntity>, IQueryable<TModel>> select,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            try
            {
                IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking().AsQueryable();

                if (include != null)
                {
                    query = include(query);
                }

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                var data = select(query);

                return await data.FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Add

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            try
            {
                await _dbSet.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Update
        public async Task UpdateAsync(
            Action<TEntity> action,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            IQueryable<TEntity> query = _dbSet.Where(filter).AsNoTracking().AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            var entity = await query.FirstOrDefaultAsync();

            action(entity!);

            _dbSet.Update(entity!);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<TEntity> entities)
        {
            try
            {
                _dbSet.UpdateRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Delete
        public async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                TEntity entity = _dbSet.FirstOrDefault(filter)!;
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteRangeAsync(List<TEntity> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteRangeAsync(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                List<TEntity> entities = await _dbSet.Where(filter).ToListAsync();

                if (entities.Any())
                {
                    _dbSet.RemoveRange(entities);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
