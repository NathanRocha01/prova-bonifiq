using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public interface IOrderService
    {
        Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId);
    }
    public class OrderService : IOrderService
    {
        private readonly TestDbContext _ctx;
        private readonly IReadOnlyDictionary<string, IPaymentProcessor> _processors;

        public OrderService(TestDbContext ctx, IEnumerable<IPaymentProcessor> processors)
        {
            _ctx = ctx;
            _processors = processors.ToDictionary(p => p.Method, p => p, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
		{

            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new ArgumentException("paymentMethod obrigatório.", nameof(paymentMethod));

            if (!_processors.TryGetValue(paymentMethod.Trim(), out var processor))
                throw new NotSupportedException($"Método de pagamento '{paymentMethod}' não suportado.");

            await processor.ProcessAsync(paymentValue, customerId);

            var order = new Order
            {
                CustomerId = customerId,
                Value = paymentValue, 
                OrderDate = DateTime.UtcNow     
            };

            _ctx.Orders.Add(order);
            await _ctx.SaveChangesAsync();

            return order;
        }
	}
}
