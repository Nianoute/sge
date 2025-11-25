using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGE.Application.DTOs.Employees;
using SGE.Application.Interfaces.Services;

namespace SGE.API.Controllers;

/// <summary>
///     API controller responsible for managing employee-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController(IEmployeeService employeeService) :
    ControllerBase
{
    /// <summary>
    ///     Retrieves all employees.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An asynchronous task that returns an action result containing an enumerable collection of EmployeeDto objects.
    /// </returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>>
        GetAll(CancellationToken cancellationToken)
    {
        var employees = await
            employeeService.GetAllAsync(cancellationToken);
        return Ok(employees);
    }

    /// <summary>
    ///     Retrieves an employee by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An asynchronous task that returns an action result containing the employee data transfer object (EmployeeDto) if
    ///     found; otherwise, a not found result.
    /// </returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id,
        CancellationToken cancellationToken)
    {
        var employee = await employeeService.GetByIdAsync(id,
            cancellationToken);
        if (employee == null) return NotFound();
        return Ok(employee);
    }

    /// <summary>
    ///     Retrieves an employee by their email address.
    /// </summary>
    /// <param name="email">The email address of the employee to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An asynchronous task that returns an action result containing the employee's data transfer object (EmployeeDto) if
    ///     found, otherwise a NotFound result.
    /// </returns>
    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<EmployeeDto>> GetByEmail(string
        email, CancellationToken cancellationToken)
    {
        var employee = await employeeService.GetByEmailAsync(email,
            cancellationToken);
        if (employee == null) return NotFound();
        return Ok(employee);
    }

    /// <summary>
    ///     Retrieves employees associated with a specific department.
    /// </summary>
    /// <param name="departmentId">The identifier of the department to retrieve employees for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An asynchronous task that returns an action result containing an enumerable collection of EmployeeDto objects.
    /// </returns>
    [HttpGet("by-department/{departmentId:int}")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>>
        GetByDepartment(int departmentId, CancellationToken cancellationToken)
    {
        var employees = await
            employeeService.GetByDepartmentAsync(departmentId, cancellationToken);
        return Ok(employees);
    }

    /// <summary>
    ///     Creates a new employee record.
    /// </summary>
    /// <param name="dto">The data transfer object containing the details of the employee to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An asynchronous task that returns an action result containing the created EmployeeDto object.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>>
        Create(EmployeeCreateDto dto, CancellationToken cancellationToken)
    {
        var created = await employeeService.CreateAsync(dto,
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            created);
    }

    /// <summary>
    ///     Updates an existing employee with the provided details.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to be updated.</param>
    /// <param name="dto">The data transfer object containing the updated employee details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An action result indicating the outcome of the update operation. Returns <c>NoContent</c> if the update is
    ///     successful or <c>NotFound</c> if the employee is not found.
    /// </returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(int id, EmployeeUpdateDto dto, CancellationToken cancellationToken)
    {
        var ok = await employeeService.UpdateAsync(id, dto, cancellationToken);
        if (!ok) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Deletes an employee by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An asynchronous task that returns an action result. Returns NoContent when the deletion is successful, or NotFound
    ///     if the employee does not exist.
    /// </returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return NoContent();
    }

    [HttpPost("import")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(EmployeeImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeImportResultDto>> Import(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        // Vérifier que le fichier existe
        if (file == null || file.Length == 0)
            return BadRequest("Aucun fichier fourni");

        // Vérifier l'extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx")
            return BadRequest("Seuls les fichiers Excel (.xlsx) sont acceptés");

        // Vérifier le type MIME
        if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            return BadRequest("Type de fichier invalide");

        // Vérifier la taille (par exemple, max 10 MB)
        if (file.Length > 10 * 1024 * 1024)
            return BadRequest("Le fichier est trop volumineux (max 10 MB)");

        using var stream = file.OpenReadStream();
        var result = await employeeService.ImportFromExcelAsync(stream, cancellationToken);

        return Ok(result);
    }

    [HttpGet("export")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Export(CancellationToken cancellationToken)
    {
        // var fileBytes = await employeeService.ExportToExcelAsync(cancellationToken);
        // var fileName = $"Employees_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        //
        // return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        return Ok();
    }
}