using SGE.Core.Entities;

namespace SGE.Application.Interfaces.Repositories;

/// <summary>
///     Repository interface for managing attendance records in the data store.
/// </summary>
public interface IAttendanceRepository
{
    /// <summary>
    ///     Retrieves an attendance record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the attendance record.</param>
    /// <param name="cancellationToken">A token to observe during the asynchronous operation for cancellation.</param>
    /// <returns>The attendance record if found; otherwise, null.</returns>
    Task<Attendance?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves the attendance record for a specific employee on a specific date.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="date">The date for which to retrieve the attendance record.</param>
    /// <param name="cancellationToken">A token to observe during the asynchronous operation for cancellation.</param>
    /// <returns>The attendance record if found; otherwise, null.</returns>
    Task<Attendance?> GetByEmployeeAndDateAsync(int employeeId, DateTime date,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all attendance records for a specific employee within an optional date range.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="startDate">The start date of the range (optional).</param>
    /// <param name="endDate">The end date of the range (optional).</param>
    /// <param name="cancellationToken">A token to observe during the asynchronous operation for cancellation.</param>
    /// <returns>A collection of attendance records for the specified employee.</returns>
    Task<IEnumerable<Attendance>> GetByEmployeeIdAsync(int employeeId, DateTime? startDate = null,
        DateTime? endDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all attendance records for a specific date.
    /// </summary>
    /// <param name="date">The date for which to retrieve attendance records.</param>
    /// <param name="cancellationToken">A token to observe during the asynchronous operation for cancellation.</param>
    /// <returns>A collection of attendance records for the specified date.</returns>
    Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds a new attendance record to the data store.
    /// </summary>
    /// <param name="attendance">The attendance record to add.</param>
    /// <param name="cancellationToken">A token to observe during the asynchronous operation for cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Attendance attendance, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing attendance record in the data store.
    /// </summary>
    /// <param name="attendance">The attendance record to update.</param>
    /// <param name="cancellationToken">A token to observe during the asynchronous operation for cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Attendance attendance, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an attendance record from the data store.
    /// </summary>
    /// <param name="attendance">The attendance record to delete.</param>
    /// <param name="cancellationToken">A token to observe during the asynchronous operation for cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Attendance attendance, CancellationToken cancellationToken = default);
}