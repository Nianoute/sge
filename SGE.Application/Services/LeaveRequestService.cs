using AutoMapper;
using SGE.Application.DTOs.LeaveRequests;
using SGE.Application.Interfaces.Repositories;
using SGE.Application.Interfaces.Services;
using SGE.Core.Entities;
using SGE.Core.Enums;
using SGE.Core.Exceptions;

namespace SGE.Application.Services;

public class LeaveRequestService(
    IEmployeeRepository employeeRepository,
    ILeaveRequestRepository leaveRequestRepository,
    IMapper mapper) : ILeaveRequestService
{
    /// <summary>
    ///     Creates a new leave request in the system asynchronously.
    /// </summary>
    /// <param name="dto">
    ///     The data required to create a new leave request, including employee ID, leave type, start date, end date, and
    ///     reason.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     The details of the created leave request wrapped in a LeaveRequestDto.
    /// </returns>
    /// <exception cref="NotImplementedException">
    ///     Thrown if the method is not implemented.
    /// </exception>
    public async Task<LeaveRequestDto> CreateAsync(LeaveRequestCreateDto
            dto,
        CancellationToken cancellationToken = default)
    {
        var employee = await
            employeeRepository.GetByIdAsync(dto.EmployeeId, cancellationToken);
        if (employee is null)
            throw new EmployeeNotFoundException(dto.EmployeeId);
        if (dto.EndDate < dto.StartDate)
            throw new ValidationException("EndDate", "La date de fin doit être supérieure à la date de début.");
        if (dto.StartDate < DateTime.Today)
            throw new ValidationException("StartDate",
                "La date de début doit être supérieure ou égale à la date de jour.");
        var daysRequested = CalculateBusinessDays(dto.StartDate,
            dto.EndDate);
        var hasConflict = await HasConflictingLeaveAsync(dto.EmployeeId,
            dto.StartDate, dto.EndDate, cancellationToken: cancellationToken);
        if (hasConflict)
            throw new ConflictingLeaveRequestException(dto.StartDate, dto.EndDate);
        var entity = mapper.Map<LeaveRequest>(dto);
        entity.DaysRequested = daysRequested;
        await leaveRequestRepository.AddAsync(entity,
            cancellationToken);
        return mapper.Map<LeaveRequestDto>(entity);
    }

    /// <summary>
    ///     Retrieves the details of a leave request by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">
    ///     The unique identifier of the leave request to be retrieved.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     The details of the leave request wrapped in a LeaveRequestDto, or null if no leave request with the specified ID
    ///     exists.
    /// </returns>
    /// <exception cref="NotImplementedException">
    ///     Thrown if the method is not implemented.
    /// </exception>
    public async Task<LeaveRequestDto?> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Retrieves the leave requests associated with a specific employee asynchronously.
    /// </summary>
    /// <param name="employeeId">
    ///     The unique identifier of the employee whose leave requests are to be retrieved.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A collection of leave request details wrapped in LeaveRequestDto objects.
    /// </returns>
    /// <exception cref="NotImplementedException">
    ///     Thrown if the method is not implemented.
    /// </exception>
    public async Task<IEnumerable<LeaveRequestDto>>
        GetLeaveRequestsByEmployeeAsync(int employeeId,
            CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Retrieves a collection of leave requests based on the specified status asynchronously.
    /// </summary>
    /// <param name="status">
    ///     The status of the leave requests to filter by, such as Pending, Approved, Rejected, or Cancelled.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A collection of leave requests matching the specified status, wrapped in LeaveRequestDto objects.
    /// </returns>
    /// <exception cref="NotImplementedException">
    ///     Thrown if the method is not implemented.
    /// </exception>
    public async Task<IEnumerable<LeaveRequestDto>>
        GetLeaveRequestsByStatusAsync(LeaveStatus status,
            CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Retrieves all leave requests with a status of pending asynchronously.
    /// </summary>
    /// <returns>
    ///     A collection of leave requests that are currently pending, wrapped in LeaveRequestDto objects.
    /// </returns>
    /// <exception cref="NotImplementedException">
    ///     Thrown if the method is not implemented.
    /// </exception>
    public async Task<IEnumerable<LeaveRequestDto>>
        GetPendingLeaveRequestsAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Updates the status of an existing leave request asynchronously.
    /// </summary>
    /// <param name="id">
    ///     The unique identifier of the leave request to be updated.
    /// </param>
    /// <param name="dto">
    ///     An object containing the updated status and optional manager comments for the leave request.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A boolean value indicating whether the operation was successful.
    /// </returns>
    /// <exception cref="NotImplementedException">
    ///     Thrown if the method is not implemented.
    /// </exception>
    public async Task<bool> UpdateStatusAsync(int id,
        LeaveRequestUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Retrieves the remaining leave days for a specific employee in a given year asynchronously.
    /// </summary>
    /// <param name="employeeId">
    ///     The unique identifier of the employee for whom the remaining leave days are being retrieved.
    /// </param>
    /// <param name="year">
    ///     The year for which the remaining leave days are being calculated.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     The total number of remaining leave days for the specified employee and year.
    /// </returns>
    /// <exception cref="NotImplementedException">
    ///     Thrown if the method is not implemented.
    /// </exception>
    public async Task<int> GetRemainingLeaveDaysAsync(int employeeId, int
            year,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Checks if there are any conflicting leave requests for an
    ///     employee within the specified date range.
    /// </summary>
    /// <param name="employeeId">
    ///     The ID of the employee for whom the check is being performed.
    /// </param>
    /// <param name="startDate">
    ///     The start date of the leave period to verify for conflicts.
    /// </param>
    /// <param name="endDate">
    ///     The end date of the leave period to verify for conflicts.
    /// </param>
    /// <param name="excludeRequestId">
    ///     An optional leave request ID to exclude from the conflict check, typically used when updating an existing leave
    ///     request.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests during the operation.
    /// </param>
    /// <returns>
    ///     A boolean value indicating whether any conflicting leave requests exist.
    /// </returns>
    /// <exception cref="NotImplementedException">
    ///     Thrown if the method is not implemented.
    /// </exception>
    public async Task<bool> HasConflictingLeaveAsync(int employeeId,
        DateTime startDate, DateTime endDate,
        int? excludeRequestId = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private int CalculateBusinessDays(DateTime startDate, DateTime
        endDate)
    {
        var businessDays = 0;
        var current = startDate;
        while (current <= endDate)
        {
            if (current.DayOfWeek != DayOfWeek.Saturday &&
                current.DayOfWeek != DayOfWeek.Sunday)
                businessDays++;
            current = current.AddDays(1);
        }

        return businessDays;
    }
}