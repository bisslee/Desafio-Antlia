﻿using ManualMovements.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

namespace ManualMovements.Infrastructure
{
    public class ReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext Context;
        public ReadRepository(
            AppDbContext context
            )
        {
            Context = context;
        }

        public async Task<List<TEntity>> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async Task<(List<TEntity>, int)> FindWithPagination
        (
            Expression<Func<TEntity, bool>> predicate,
            int page,
            int pageSize,
            string? fieldName = null,
            string? order = null
        )
        {
            var query = Context.Set<TEntity>().Where(predicate);
            var total = await query.CountAsync();

            if (!string.IsNullOrEmpty(fieldName))
            {
                var ordering = fieldName +
                    (string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase)
                        ? " descending" : "");
                query = query.OrderBy(ordering);
            }

            var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (data, total);
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await Context.Set<TEntity>().FindAsync(id);
        }

        public async Task<List<TEntity>> ExecuteGetEntitiesSql(string sql, params object[] parameters) 
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }

            return await Context.Set<TEntity>().FromSqlRaw(sql, parameters).ToListAsync();
        }


    }
}
