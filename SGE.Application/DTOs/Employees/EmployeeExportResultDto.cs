namespace SGE.Application.DTOs.Employees;

/// <summary>
///     Résultat de l'import du fichier excel
/// </summary>
public class EmployeeExportResultDto
{
    public int TotalRows { get; set; } // Total de lignes récupérées
    public int SuccessCount { get; set; } // Nombre d'employées créé avec succès
    public int ErrorCount { get; set; } // Nombre d'erreurs
    public List<ExportError> Errors { get; set; } = new(); // Détail de l'erreur
}

public class ExportError
{
    public int RowNumber { get; set; } // Numéro de la ligne qui renvoie l'erreur
    public string Field { get; set; } = string.Empty; // Champ concerné par l'erreur
    public string Message { get; set; } = string.Empty; // Message contenant l'erreur (lisible)
}