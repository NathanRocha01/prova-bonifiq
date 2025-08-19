using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public interface ICustomerService
    {
        Task<PagedList<Customer>> ListCustomers(int page);
        Task<bool> CanPurchase(int customerId, decimal purchaseValue, DateTime? nowUtc = null);
    }
    public class CustomerService : PagedReadService<Customer>, ICustomerService
    {
        public CustomerService(TestDbContext ctx) : base(ctx) { }
        public Task<PagedList<Customer>> ListCustomers(int page)
            => ListAsync(page, 10);

        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue, DateTime? nowUtc = null)
        {
            if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));

            if (purchaseValue <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseValue));

            var now = nowUtc ?? DateTime.UtcNow;

            //Business Rule: Non registered Customers cannot purchase
            var customer = await _ctx.Customers.FindAsync(customerId);
            if (customer == null) throw new InvalidOperationException($"Customer Id {customerId} does not exists");

            //Business Rule: A customer can purchase only a single time per month
            var baseDate = DateTime.UtcNow.AddMonths(-1);
            var ordersInThisMonth = await _ctx.Orders.CountAsync(s => s.CustomerId == customerId && s.OrderDate >= baseDate);
            if (ordersInThisMonth > 0)
                return false;

            //Business Rule: A customer that never bought before can make a first purchase of maximum 100,00
            var haveBoughtBefore = await _ctx.Customers.CountAsync(s => s.Id == customerId && s.Orders.Any());
            if (haveBoughtBefore == 0 && purchaseValue > 100)
                return false;

            //Business Rule: A customer can purchases only during business hours and working days
            if (now.Hour < 8 || now.Hour > 18 || now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return false;


            return true;
        }

    }
}
