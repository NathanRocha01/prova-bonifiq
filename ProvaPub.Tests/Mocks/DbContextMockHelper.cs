using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using ProvaPub.Models;
using ProvaPub.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvaPub.Tests.Mocks
{
    public static class DbContextMockHelper
    {
        public static Mock<TestDbContext> Create(
            IQueryable<Customer> customers,
            IQueryable<Order> orders)
        {
            var customersSet = customers.BuildMockDbSet();

            customersSet
                .Setup(s => s.FindAsync(It.IsAny<object[]>()))
                .Returns((object[] ids) =>
                {
                    var id = (int)ids[0];
                    var c = customers.FirstOrDefault(x => x.Id == id);
                    return new ValueTask<Customer?>(c);
                });

            var ordersSet = orders.BuildMockDbSet();

            var ctx = new Mock<TestDbContext>(new DbContextOptions<TestDbContext>());
            ctx.Setup(c => c.Customers).Returns(customersSet.Object);
            ctx.Setup(c => c.Orders).Returns(ordersSet.Object);

            return ctx;
        }
    }
}
