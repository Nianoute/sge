using SGE.Core.Entities;
using SGE.Core.Enums;

namespace SGE.Application.Interfaces.Repositories;

/// <summary>
///     Represents a repository interface for handling data access operations specific to LeaveRequest entities.
///     Provides methods for retrieving LeaveRequest data with additional customization for employee-specific requests.
/// </summary>
public interface ILeaveRequestRepository : IRepository<LeaveRequest>
{
    /// <summary>
    ///     Asynchronously retrieves a collection of leave requests associated with a specific employee.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee whose leave requests are to be fetched.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to observe the cancellation request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains an enumerable collection of leave
    ///     requests associated with the specified employee.
    /// </returns>
    Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Asynchronously retrieves a collection of leave requests filtered by a specific status.
    /// </summary>
    /// <param name="status">The status to filter leave requests by (e.g., Pending, Approved, Rejected, Cancelled).</param>
    /// <param name="cancellationToken">A cancellation token that can be used to observe the cancellation request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains an enumerable collection of leave
    ///     requests with the specified status.
    /// </returns>
    Task<IEnumerable<LeaveRequest>> GetByStatusAsync(LeaveStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Asynchronously retrieves all approved leave requests for a specific employee within a given year.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="year">The year for which to retrieve approved leave requests.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to observe the cancellation request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains an enumerable collection of approved
    ///     leave requests for the specified employee and year.
    /// </returns>
    Task<IEnumerable<LeaveRequest>> GetApprovedLeavesByEmployeeAndYearAsync(int employeeId, int year,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Asynchronously retrieves leave requests that overlap with a specified date range for a given employee.
    /// </summary>
    /// <param name="employeeId">The unique identifier of the employee.</param>
    /// <param name="startDate">The start date of the range to check for overlapping leave requests.</param>
    /// <param name="endDate">The end date of the range to check for overlapping leave requests.</param>
    /// <param name="excludeRequestId">Optional. The ID of a leave request to exclude from the search (useful for updates).</param>
    /// <param name="cancellationToken">A cancellation token that can be used to observe the cancellation request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains an enumerable collection of leave
    ///     requests that overlap with the specified date range.
    /// </returns>
    Task<IEnumerable<LeaveRequest>> GetOverlappingLeavesAsync(int employeeId, DateTime startDate, DateTime endDate,
        int? excludeRequestId = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Asynchronously retrieves all pending leave requests across all employees.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to observe the cancellation request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains an enumerable collection of all
    ///     pending leave requests.
    /// </returns>
    Task<IEnumerable<LeaveRequest>> GetPendingLeavesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Asynchronously retrieves leave requests for a specific department within a given date range.
    /// </summary>
    /// <param name="departmentId">The unique identifier of the department.</param>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to observe the cancellation request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains an enumerable collection of leave
    ///     requests for the specified department and date range.
    /// </returns>
    Task<IEnumerable<LeaveRequest>> GetLeavesByDepartmentAndDateRangeAsync(int departmentId, DateTime startDate,
        DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Asynchronously retrieves a leave request by its ID with all related entities.
    /// </summary>
    /// <param name="id">The unique identifier of the leave request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to observe the cancellation request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains the leave request if found, otherwise
    ///     null.
    /// </returns>
    Task<LeaveRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}