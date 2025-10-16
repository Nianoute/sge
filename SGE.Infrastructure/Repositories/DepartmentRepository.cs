using Microsoft.EntityFrameworkCore;
using SGE.Application.Interfaces.Repositories;
using SGE.Core.Entities;
using SGE.Infrastructure.Data;

namespace SGE.Infrastructure.Repositories;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Department> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Name == name, cancellationToken);
    }

    public async Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Code == code, cancellationToken);
    }
}