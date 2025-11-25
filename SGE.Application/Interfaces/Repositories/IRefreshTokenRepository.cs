using SGE.Core.Entities;
namespace SGE.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<List<RefreshToken>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<RefreshToken> tokens);
}
