using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;

namespace ProvaPub.Controllers
{
	
	[ApiController]
	[Route("[controller]")]
	public class Parte2Controller :  ControllerBase
	{
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
		public Parte2Controller(IProductService productService, ICustomerService customerService)
		{
            _productService = productService;
            _customerService = customerService;
        }
	
		[HttpGet("products")]
		public Task<PagedList<Product>> ListProducts(int page)
		{
            return _productService.ListProducts(page);
        }

		[HttpGet("customers")]
		public Task<PagedList<Customer>> ListCustomers(int page)
		{
            return _customerService.ListCustomers(page);
        }
	}
}
