namespace SGE.Application.DTOs.Employees;

public class EmployeeImportDto
{
    /// <summary>
    ///     Données d'une ligne d'un fichier excel
    /// </summary>
    public int RowNumber { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
}