using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public interface IProductService
    {
        Task<PagedList<Product>> ListProducts(int page);
    }
    public class ProductService : PagedReadService<Product>, IProductService
    {
        public ProductService(TestDbContext ctx) : base(ctx){ }

        public Task<PagedList<Product>> ListProducts(int page)
        => ListAsync(page, 10);

    }
}
