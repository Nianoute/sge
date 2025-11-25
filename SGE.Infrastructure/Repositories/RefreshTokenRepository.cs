using Microsoft.EntityFrameworkCore;
using SGE.Core.Entities;
using SGE.Application.Interfaces.Repositories;
using SGE.Infrastructure.Data;

namespace SGE.Infrastructure.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return _dbSet.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
    }

    public Task<List<RefreshToken>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task UpdateRangeAsync(IEnumerable<RefreshToken> tokens)
    {
        _context.RefreshTokens.UpdateRange(tokens);
        await _context.SaveChangesAsync();
    }
}