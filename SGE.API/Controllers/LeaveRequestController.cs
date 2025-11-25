using Microsoft.AspNetCore.Mvc;
using SGE.Application.DTOs.LeaveRequests;
using SGE.Application.Interfaces.Services;
using SGE.Core.Enums;
using SGE.Core.Exceptions;

namespace SGE.API.Controllers;

/// <summary>
///     API controller for managing leave requests in the system.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LeaveRequestController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeaveRequestController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    /// <summary>
    ///     Creates a new leave request.
    /// </summary>
    /// <param name="dto">The leave request creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created leave request.</returns>
    /// <response code="201">Returns the newly created leave request.</response>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="404">If the employee is not found.</response>
    /// <response code="409">If there is a conflicting leave request.</response>
    [HttpPost]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest(
        [FromBody] LeaveRequestCreateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _leaveRequestService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(
                nameof(GetLeaveRequestById),
                new { id = result.Id },
                result);
        }
        catch (EmployeeNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ConflictingLeaveRequestException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    ///     Retrieves a leave request by its ID.
    /// </summary>
    /// <param name="id">The leave request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The leave request details.</returns>
    /// <response code="200">Returns the leave request.</response>
    /// <response code="404">If the leave request is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequestById(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _leaveRequestService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (LeaveRequestNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    ///     Retrieves all leave requests for a specific employee.
    /// </summary>
    /// <param name="employeeId">The employee ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of leave requests for the employee.</returns>
    /// <response code="200">Returns the list of leave requests.</response>
    /// <response code="404">If the employee is not found.</response>
    [HttpGet("employee/{employeeId}")]
    [ProducesResponseType(typeof(IEnumerable<LeaveRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequestsByEmployee(
        int employeeId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _leaveRequestService.GetLeaveRequestsByEmployeeAsync(
                employeeId,
                cancellationToken);
            return Ok(result);
        }
        catch (EmployeeNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    ///     Retrieves all leave requests filtered by status.
    /// </summary>
    /// <param name="status">The leave status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of leave requests with the specified status.</returns>
    /// <response code="200">Returns the list of leave requests.</response>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequestsByStatus(
        LeaveStatus status,
        CancellationToken cancellationToken)
    {
        var result = await _leaveRequestService.GetLeaveRequestsByStatusAsync(
            status,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    ///     Retrieves all pending leave requests.
    /// </summary>
    /// <returns>A collection of pending leave requests.</returns>
    /// <response code="200">Returns the list of pending leave requests.</response>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(IEnumerable<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetPendingLeaveRequests()
    {
        var result = await _leaveRequestService.GetPendingLeaveRequestsAsync();
        return Ok(result);
    }

    /// <summary>
    ///     Updates the status of a leave request.
    /// </summary>
    /// <param name="id">The leave request ID.</param>
    /// <param name="dto">The status update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success indicator.</returns>
    /// <response code="200">If the status was updated successfully.</response>
    /// <response code="400">If the status transition is invalid.</response>
    /// <response code="404">If the leave request is not found.</response>
    /// <response code="409">If there are insufficient leave days.</response>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateLeaveRequestStatus(
        int id,
        [FromBody] LeaveRequestUpdateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _leaveRequestService.UpdateStatusAsync(id, dto, cancellationToken);
            return Ok(new { message = "Le statut de la demande de congé a été mis à jour avec succès." });
        }
        catch (LeaveRequestNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidLeaveStatusTransitionException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InsufficientLeaveDaysException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    ///     Retrieves the remaining leave days for an employee in a given year.
    /// </summary>
    /// <param name="employeeId">The employee ID.</param>
    /// <param name="year">The year to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of remaining leave days.</returns>
    /// <response code="200">Returns the remaining leave days.</response>
    /// <response code="404">If the employee is not found.</response>
    [HttpGet("employee/{employeeId}/remaining-days/{year}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<int>> GetRemainingLeaveDays(
        int employeeId,
        int year,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _leaveRequestService.GetRemainingLeaveDaysAsync(
                employeeId,
                year,
                cancellationToken);
            return Ok(result);
        }
        catch (EmployeeNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    ///     Checks if there are conflicting leave requests for an employee.
    /// </summary>
    /// <param name="employeeId">The employee ID.</param>
    /// <param name="startDate">The start date to check.</param>
    /// <param name="endDate">The end date to check.</param>
    /// <param name="excludeRequestId">Optional leave request ID to exclude from the check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if there is a conflict, false otherwise.</returns>
    /// <response code="200">Returns the conflict status.</response>
    [HttpGet("employee/{employeeId}/check-conflict")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> CheckConflictingLeave(
        int employeeId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int? excludeRequestId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _leaveRequestService.HasConflictingLeaveAsync(
            employeeId,
            startDate,
            endDate,
            excludeRequestId,
            cancellationToken);
        return Ok(result);
    }
}