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
    public async Task<LeaveRequestDto> CreateAsync(LeaveRequestCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(dto.EmployeeId, cancellationToken);
        if (employee is null)
            throw new EmployeeNotFoundException(dto.EmployeeId);

        if (dto.EndDate < dto.StartDate)
            throw new ValidationException("EndDate", "La date de fin doit être supérieure à la date de début.");

        if (dto.StartDate < DateTime.Today)
            throw new ValidationException("StartDate",
                "La date de début doit être supérieure ou égale à la date de jour.");

        var daysRequested = CalculateBusinessDays(dto.StartDate, dto.EndDate);

        var hasConflict = await HasConflictingLeaveAsync(dto.EmployeeId,
            dto.StartDate, dto.EndDate, cancellationToken: cancellationToken);
        if (hasConflict)
            throw new ConflictingLeaveRequestException(dto.StartDate, dto.EndDate);

        var entity = mapper.Map<LeaveRequest>(dto);
        entity.DaysRequested = daysRequested;

        await leaveRequestRepository.AddAsync(entity, cancellationToken);

        return mapper.Map<LeaveRequestDto>(entity);
    }

    /// <summary>
    ///     Retrieves the details of a leave request by its unique identifier asynchronously.
    /// </summary>
    public async Task<LeaveRequestDto?> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        var leaveRequest = await leaveRequestRepository.GetByIdAsync(id, cancellationToken);

        if (leaveRequest is null)
            throw new LeaveRequestNotFoundException(id);

        return mapper.Map<LeaveRequestDto>(leaveRequest);
    }

    /// <summary>
    ///     Retrieves the leave requests associated with a specific employee asynchronously.
    /// </summary>
    public async Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByEmployeeAsync(int employeeId,
        CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
            throw new EmployeeNotFoundException(employeeId);

        var leaveRequests = await leaveRequestRepository.GetByEmployeeIdAsync(employeeId, cancellationToken);

        return mapper.Map<IEnumerable<LeaveRequestDto>>(leaveRequests);
    }

    /// <summary>
    ///     Retrieves a collection of leave requests based on the specified status asynchronously.
    /// </summary>
    public async Task<IEnumerable<LeaveRequestDto>> GetLeaveRequestsByStatusAsync(LeaveStatus status,
        CancellationToken cancellationToken = default)
    {
        var leaveRequests = await leaveRequestRepository.GetByStatusAsync(status, cancellationToken);

        return mapper.Map<IEnumerable<LeaveRequestDto>>(leaveRequests);
    }

    /// <summary>
    ///     Retrieves all leave requests with a status of pending asynchronously.
    /// </summary>
    public async Task<IEnumerable<LeaveRequestDto>> GetPendingLeaveRequestsAsync()
    {
        var leaveRequests = await leaveRequestRepository.GetByStatusAsync(LeaveStatus.Pending);

        return mapper.Map<IEnumerable<LeaveRequestDto>>(leaveRequests);
    }

    /// <summary>
    ///     Updates the status of an existing leave request asynchronously.
    /// </summary>
    public async Task<bool> UpdateStatusAsync(int id,
        LeaveRequestUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var leaveRequest = await leaveRequestRepository.GetByIdAsync(id, cancellationToken);

        if (leaveRequest is null)
            throw new LeaveRequestNotFoundException(id);

        // Vérifier la validité de la transition de statut
        ValidateStatusTransition(leaveRequest.Status, dto.Status);

        // Si le statut est approuvé, vérifier les jours de congé disponibles
        if (dto.Status == LeaveStatus.Approved)
        {
            var year = leaveRequest.StartDate.Year;
            var remainingDays = await GetRemainingLeaveDaysAsync(leaveRequest.EmployeeId, year, cancellationToken);

            if (leaveRequest.DaysRequested > remainingDays)
                throw new InsufficientLeaveDaysException(leaveRequest.DaysRequested, remainingDays);
        }

        // Mettre à jour le statut
        leaveRequest.Status = dto.Status;
        leaveRequest.ManagerComments = dto.ManagerComments;
        leaveRequest.ReviewedAt = DateTime.UtcNow;

        await leaveRequestRepository.UpdateAsync(leaveRequest, cancellationToken);

        return true;
    }

    /// <summary>
    ///     Retrieves the remaining leave days for a specific employee in a given year asynchronously.
    /// </summary>
    public async Task<int> GetRemainingLeaveDaysAsync(int employeeId, int year,
        CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
            throw new EmployeeNotFoundException(employeeId);

        // Récupérer toutes les demandes approuvées pour l'année donnée
        var approvedLeaves = await leaveRequestRepository.GetApprovedLeavesByEmployeeAndYearAsync(
            employeeId, year, cancellationToken);

        // Calculer le total des jours utilisés
        var usedDays = approvedLeaves.Sum(l => l.DaysRequested);

        // Supposons que chaque employé a 25 jours de congé par an (à adapter selon vos règles métier)
        const int annualLeaveDays = 25;
        var remainingDays = annualLeaveDays - usedDays;

        return Math.Max(0, remainingDays); // Ne peut pas être négatif
    }

    /// <summary>
    ///     Checks if there are any conflicting leave requests for an employee within the specified date range.
    /// </summary>
    public async Task<bool> HasConflictingLeaveAsync(int employeeId,
        DateTime startDate, DateTime endDate,
        int? excludeRequestId = null,
        CancellationToken cancellationToken = default)
    {
        var existingLeaves = await leaveRequestRepository.GetByEmployeeIdAsync(employeeId, cancellationToken);

        // Filtrer les demandes qui ne sont pas annulées ou rejetées
        var activeLeaves = existingLeaves.Where(l =>
            l.Status != LeaveStatus.Rejected &&
            l.Status != LeaveStatus.Cancelled &&
            (!excludeRequestId.HasValue || l.Id != excludeRequestId.Value));

        // Vérifier s'il y a un chevauchement de dates
        foreach (var leave in activeLeaves)
            // Il y a conflit si les périodes se chevauchent
            if (startDate <= leave.EndDate && endDate >= leave.StartDate)
                return true;

        return false;
    }

    /// <summary>
    ///     Valide la transition de statut d'une demande de congé.
    /// </summary>
    private void ValidateStatusTransition(LeaveStatus currentStatus, LeaveStatus newStatus)
    {
        // Définir les transitions valides
        var validTransitions = new Dictionary<LeaveStatus, List<LeaveStatus>>
        {
            {
                LeaveStatus.Pending,
                new List<LeaveStatus> { LeaveStatus.Approved, LeaveStatus.Rejected, LeaveStatus.Cancelled }
            },
            { LeaveStatus.Approved, new List<LeaveStatus> { LeaveStatus.Cancelled } },
            { LeaveStatus.Rejected, new List<LeaveStatus>() }, // Aucune transition possible
            { LeaveStatus.Cancelled, new List<LeaveStatus>() } // Aucune transition possible
        };

        if (!validTransitions.ContainsKey(currentStatus) ||
            !validTransitions[currentStatus].Contains(newStatus))
            throw new InvalidLeaveStatusTransitionException(currentStatus.ToString(), newStatus.ToString());
    }

    /// <summary>
    ///     Calcule le nombre de jours ouvrables entre deux dates.
    /// </summary>
    private int CalculateBusinessDays(DateTime startDate, DateTime endDate)
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