using FluentValidation;
using SGE.Application.DTOs.Employees;
using SGE.Application.Interfaces.Repositories;

namespace SGE.Application.Validators;

public class EmployeeImportValidator : AbstractValidator<EmployeeImportDto>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeImportValidator(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Le prénom est obligatoire")
            .MaximumLength(50).WithMessage("Le prénom ne doit pas dépasser 50 caractères");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Le nom est obligatoire")
            .MaximumLength(50).WithMessage("Le nom ne doit pas dépasser 100 caractères");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email est obligatoire")
            .EmailAddress().WithMessage("Format d'email invalide")
            .MustAsync(BeUniqueEmail).WithMessage("Cet email existe déjà")
            .MaximumLength(100).WithMessage("L'email doit contenir un maximum de 100 caractères");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Le numéro de téléphone est obligatoire")
            .Matches(@"^0[1-9]\d{8}$").WithMessage("Format de téléphone invalide (ex: 0612345678)");

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Le poste est obligatoire");

        RuleFor(x => x.Salary)
            .NotEmpty().WithMessage("Le salaire est obligatoire")
            .GreaterThan(0).WithMessage("Le salaire doit être supérieur à 0");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("La date d'embauche est obligatoire")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("La date d'embauche ne peut pas être dans le futur");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Le département est obligatoire")
            .MustAsync(DepartmentExists).WithMessage("Le département n'existe pas");
    }

    // Fonction async permettant la vérification d'un email si il est unique
    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var existing = await _employeeRepository.GetByEmailAsync(email, cancellationToken);
        return existing == null;
    }

    // Fonction async permettant la vérification d'un département si il existe
    private async Task<bool> DepartmentExists(int id, CancellationToken cancellationToken)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken);
        return departments.Any(d => d.Id == id);
    }
}