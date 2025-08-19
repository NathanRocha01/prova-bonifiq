using ProvaPub.DTOs;
using ProvaPub.Models;

namespace ProvaPub.Services.Mapping
{
    public interface IOrderDtoFactory
    {
        OrderDto FromEntity(Order order);
    }

    public class OrderDtoFactory : IOrderDtoFactory
    {
        private static readonly TimeZoneInfo TzBr = TimeZoneInfo.FindSystemTimeZoneById("America/Recife");

        public OrderDto FromEntity(Order order)
        {
            var local = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, TzBr);
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Value = order.Value,
                OrderDate = local
            };
        }
    }
}
