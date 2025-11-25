using Microsoft.EntityFrameworkCore;
using SGE.Application.Interfaces.Repositories;
using SGE.Core.Entities;
using SGE.Infrastructure.Data;

namespace SGE.Infrastructure.Repositories;

/// <summary>
///     Implementation of the attendance repository using Entity Framework Core.
/// </summary>
public class AttendanceRepository : IAttendanceRepository
{
    private readonly ApplicationDbContext _context;

    public AttendanceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Retrieves an attendance record by its unique identifier.
    /// </summary>
    public async Task<Attendance?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Attendances
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    /// <summary>
    ///     Retrieves the attendance record for a specific employee on a specific date.
    /// </summary>
    public async Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateTime date,
        CancellationToken cancellationToken = default)
    {
        return await _context.Attendances
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == date.Date, cancellationToken);
    }

    /// <summary>
    ///     Retrieves all attendance records for a specific employee within an optional date range.
    /// </summary>
    public async Task<IEnumerable<Attendance>> GetByEmployeeIdAsync(int employeeId, DateTime? startDate = null,
        DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Attendances
            .Include(a => a.Employee)
            .Where(a => a.EmployeeId == employeeId);

        if (startDate.HasValue)
            query = query.Where(a => a.Date >= startDate.Value.Date);

        if (endDate.HasValue)
            query = query.Where(a => a.Date <= endDate.Value.Date);

        return await query
            .OrderBy(a => a.Date)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Retrieves all attendance records for a specific date.
    /// </summary>
    public async Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date,
        CancellationToken cancellationToken = default)
    {
        return await _context.Attendances
            .Include(a => a.Employee)
            .Where(a => a.Date == date.Date)
            .OrderBy(a => a.EmployeeId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    ///     Adds a new attendance record to the data store.
    /// </summary>
    public async Task AddAsync(Attendance attendance, CancellationToken cancellationToken = default)
    {
        await _context.Attendances.AddAsync(attendance, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    ///     Updates an existing attendance record in the data store.
    /// </summary>
    public async Task UpdateAsync(Attendance attendance, CancellationToken cancellationToken = default)
    {
        _context.Attendances.Update(attendance);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    ///     Deletes an attendance record from the data store.
    /// </summary>
    public async Task DeleteAsync(Attendance attendance, CancellationToken cancellationToken = default)
    {
        _context.Attendances.Remove(attendance);
        await _context.SaveChangesAsync(cancellationToken);
    }
}