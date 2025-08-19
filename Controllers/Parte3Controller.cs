using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProvaPub.DTOs;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;
using ProvaPub.Services.Mapping;

namespace ProvaPub.Controllers
{
    [ApiController]
	[Route("[controller]")]
	public class Parte3Controller :  ControllerBase
	{
        private readonly IOrderService _orderService;
        private readonly IOrderDtoFactory _factory;

        public Parte3Controller(IOrderService orderService, IOrderDtoFactory factory)
        {
            _orderService = orderService;
            _factory = factory;
        }

        [HttpGet("orders")]
        public async Task<ActionResult<OrderDto>> PlaceOrder(string paymentMethod, decimal paymentValue, int customerId)
        => Ok(_factory.FromEntity(await _orderService.PayOrder(paymentMethod, paymentValue, customerId)));
    }
}
