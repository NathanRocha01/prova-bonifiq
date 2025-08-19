using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public interface IRandomService
    {
        Task<int> GetRandom(CancellationToken ct = default);
    }
    public class RandomService : IRandomService
	{
        private readonly TestDbContext _ctx;
        public RandomService(TestDbContext ctx) => _ctx = ctx;

        public async Task<int> GetRandom(CancellationToken ct = default)
        {
            const int maxAttempts = 5;
            for (var i = 0; i < maxAttempts; i++)
            {
                var n = Random.Shared.Next(1, 1_000_001);
                try
                {
                    _ctx.Numbers.Add(new RandomNumber { Number = n });
                    await _ctx.SaveChangesAsync(ct);
                    return n;
                }
                catch (DbUpdateException ex) when (IsUniqueViolation(ex))
                {
                    _ctx.ChangeTracker.Clear();
                }
            }
            throw new InvalidOperationException("Falha ao gerar número único após várias tentativas.");
        }
        private static bool IsUniqueViolation(DbUpdateException ex)
       => ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);
    }
}
