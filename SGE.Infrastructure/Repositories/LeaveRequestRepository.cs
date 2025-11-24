using Microsoft.EntityFrameworkCore;
using SGE.Application.Interfaces.Repositories;
using SGE.Core.Entities;
using SGE.Core.Enums;
using SGE.Infrastructure.Data;

namespace SGE.Infrastructure.Repositories;

/// <summary>
///     Provides the implementation for leave request data access and management operations.
/// </summary>
/// <remarks>
///     This repository extends the base functionality provided by the generic <see cref="Repository{T}" /> class.
///     It focuses specifically on operations related to the <see cref="LeaveRequest" /> entity, including retrieving
///     leave requests associated with a specific employee.
/// </remarks>
public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
{
    /// <summary>
    ///     A repository class for managing leave request data in the application's database.
    /// </summary>
    /// <remarks>
    ///     Inherits from the generic <see cref="Repository{T}" /> class, providing common repository operations.
    ///     This class specializes in operations related to the <see cref="LeaveRequest" /> entity and
    ///     includes methods for specific use cases, such as retrieving leave requests by employee identifier.
    /// </remarks>
    public LeaveRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    ///     Retrieves a collection of leave requests associated with a specific employee.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee whose leave requests are to be retrieved.</param>
    /// <param name="cancellationToken">A token to observe for operation cancellation.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains an <see cref="IEnumerable{T}" /> of
    ///     <see cref="LeaveRequest" /> objects associated with the specified employee.
    /// </returns>
    public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(lr => lr.EmployeeId == employeeId)
            .Include(lr => lr.Employee)
            .OrderByDescending(lr => lr.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Asynchronously retrieves a collection of leave requests filtered by a specific status.
    /// </summary>
    public async Task<IEnumerable<LeaveRequest>> GetByStatusAsync(LeaveStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(lr => lr.Status == status)
            .Include(lr => lr.Employee)
            .ThenInclude(e => e.Department)
            .OrderByDescending(lr => lr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Asynchronously retrieves all approved leave requests for a specific employee within a given year.
    /// </summary>
    public async Task<IEnumerable<LeaveRequest>> GetApprovedLeavesByEmployeeAndYearAsync(int employeeId, int year,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(lr => lr.EmployeeId == employeeId &&
                         lr.Status == LeaveStatus.Approved &&
                         lr.StartDate.Year == year)
            .OrderBy(lr => lr.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Asynchronously retrieves leave requests that overlap with a specified date range for a given employee.
    /// </summary>
    public async Task<IEnumerable<LeaveRequest>> GetOverlappingLeavesAsync(int employeeId, DateTime startDate,
        DateTime endDate, int? excludeRequestId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(lr => lr.EmployeeId == employeeId &&
                         lr.Status != LeaveStatus.Rejected &&
                         lr.Status != LeaveStatus.Cancelled &&
                         lr.StartDate <= endDate &&
                         lr.EndDate >= startDate);

        if (excludeRequestId.HasValue) query = query.Where(lr => lr.Id != excludeRequestId.Value);

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Asynchronously retrieves all pending leave requests across all employees.
    /// </summary>
    public async Task<IEnumerable<LeaveRequest>> GetPendingLeavesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(lr => lr.Status == LeaveStatus.Pending)
            .Include(lr => lr.Employee)
            .ThenInclude(e => e.Department)
            .OrderBy(lr => lr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Asynchronously retrieves leave requests for a specific department within a given date range.
    /// </summary>
    public async Task<IEnumerable<LeaveRequest>> GetLeavesByDepartmentAndDateRangeAsync(int departmentId,
        DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(lr => lr.Employee.DepartmentId == departmentId &&
                         lr.Status == LeaveStatus.Approved &&
                         lr.StartDate <= endDate &&
                         lr.EndDate >= startDate)
            .Include(lr => lr.Employee)
            .ThenInclude(e => e.Department)
            .OrderBy(lr => lr.StartDate)
            .ThenBy(lr => lr.Employee.LastName)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Asynchronously retrieves a leave request by its ID with all related entities.
    /// </summary>
    public async Task<LeaveRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(lr => lr.Employee)
            .ThenInclude(e => e.Department)
            .FirstOrDefaultAsync(lr => lr.Id == id, cancellationToken);
    }
}