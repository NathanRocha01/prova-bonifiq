using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using System.Linq.Expressions;

namespace ProvaPub.Services
{
    public abstract class PagedReadService<TEntity> where TEntity : class, IHasId
    {
        protected readonly TestDbContext _ctx;
        protected PagedReadService(TestDbContext ctx) => _ctx = ctx;

        protected virtual IQueryable<TEntity> Query()
            => _ctx.Set<TEntity>().AsNoTracking();

        public async Task<PagedList<TEntity>> ListAsync(int page, int pageSize = 10)
        {
            if (page < 1) page = 1;

            var query = Query().OrderBy(e => e.Id);

            var total = await query.CountAsync();
            var skip = (page - 1) * pageSize;

            var items = (skip >= total)
                ? new List<TEntity>()
                : await query.Skip(skip).Take(pageSize).ToListAsync();

            return new PagedList<TEntity>
            {
                TotalCount = total,
                HasNext = page * pageSize < total,
                Items = items
            };
        }
    }
}
