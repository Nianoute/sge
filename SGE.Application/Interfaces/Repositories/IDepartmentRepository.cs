using SGE.Core.Entities;

namespace SGE.Application.Interfaces.Repositories;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<Department> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}