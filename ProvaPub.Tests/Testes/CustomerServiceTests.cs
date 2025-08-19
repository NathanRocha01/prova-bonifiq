using ProvaPub.Models;
using ProvaPub.Services;
using ProvaPub.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using MockQueryable.Moq;
using Microsoft.EntityFrameworkCore;

namespace ProvaPub.Tests.Testes
{
    public class CustomerServiceMockTests
    {
        [Fact]
        public async Task Deve_retornar_false_quando_ja_existir_compra_no_ultimo_mes()
        {
            // Arrange
            var customers = new List<Customer> {
            new Customer { Id = 1, Name = "Alice", Orders = new List<Order>() }
            }.AsQueryable();

            var orders = new List<Order> {
            new Order { CustomerId = 1, OrderDate = new DateTime(2025, 8, 10, 12, 0, 0, DateTimeKind.Utc) }
            }.AsQueryable();

            var customersSet = customers.BuildMockDbSet();

            var ctxMock = DbContextMockHelper.Create(customers, orders);
            var svc = new CustomerService(ctxMock.Object);

            var now = new DateTime(2025, 8, 18, 12, 0, 0, DateTimeKind.Utc);

            // Act
            var ok = await svc.CanPurchase(1, 50m, now);

            // Assert
            ok.Should().BeFalse();
        }

        [Fact]
        public async Task Primeira_compra_maior_que_100_deve_retornar_false()
        {
            var customers = new List<Customer> {
            new Customer { Id = 2, Name = "Bob", Orders = new List<Order>() } // nunca comprou
        }.AsQueryable();

            var orders = new List<Order>().AsQueryable();

            var ctxMock = DbContextMockHelper.Create(customers, orders);
            var svc = new CustomerService(ctxMock.Object);

            var now = new DateTime(2025, 8, 18, 14, 0, 0, DateTimeKind.Utc); // dia útil e horário comercial

            var ok = await svc.CanPurchase(2, 150m, now);

            ok.Should().BeFalse();
        }

        [Fact]
        public async Task Fora_do_horario_comercial_ou_fim_de_semana_deve_retornar_false()
        {
            var customers = new List<Customer> {
            new Customer { Id = 3, Name = "Carol", Orders = new List<Order>() }
            }.AsQueryable();

            var orders = new List<Order>().AsQueryable();

            var ctxMock = DbContextMockHelper.Create(customers, orders);
            var svc = new CustomerService(ctxMock.Object);

            var sabado = new DateTime(2025, 8, 16, 10, 0, 0, DateTimeKind.Utc); // sábado
            (await svc.CanPurchase(3, 50m, sabado)).Should().BeFalse();

            var cedo = new DateTime(2025, 8, 18, 5, 0, 0, DateTimeKind.Utc); // fora de 8..18
            (await svc.CanPurchase(3, 50m, cedo)).Should().BeFalse();
        }

        [Fact]
        public async Task Todas_as_regras_ok_deve_retornar_true()
        {
            var customers = new List<Customer> {
            new Customer { Id = 4, Name = "Dave", Orders = new List<Order>() }
        }.AsQueryable();

            var orders = new List<Order>().AsQueryable();

            var ctxMock = DbContextMockHelper.Create(customers, orders);
            var svc = new CustomerService(ctxMock.Object);

            var now = new DateTime(2025, 8, 18, 14, 0, 0, DateTimeKind.Utc);

            (await svc.CanPurchase(4, 100m, now)).Should().BeTrue();
        }

        [Fact]
        public async Task Dispara_excecoes_para_parametros_invalidos_e_cliente_inexistente()
        {
            var customers = new List<Customer> {
            new Customer { Id = 10, Name = "Zoe", Orders = new List<Order>() }
        }.AsQueryable();

            var orders = new List<Order>().AsQueryable();

            var ctxMock = DbContextMockHelper.Create(customers, orders);
            var svc = new CustomerService(ctxMock.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => svc.CanPurchase(0, 10m, DateTime.UtcNow));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => svc.CanPurchase(1, 0m, DateTime.UtcNow));
            await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CanPurchase(999, 10m, DateTime.UtcNow));
        }
    }
}
