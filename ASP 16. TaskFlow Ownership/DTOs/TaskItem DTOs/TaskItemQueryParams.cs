using System.ComponentModel.DataAnnotations;

namespace ASP_16._TaskFlow_Ownership.DTOs.TaskItem_DTOs;

public class TaskItemQueryParams
{
    /// <summary>
    /// Page number for pagination (starts from 1).
    /// </summary>
    //[Required]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page.
    /// </summary>

    //[Range(1, 100)]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Field name used for sorting (e.g. CreatedAt, Priority, Title).
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Sorting direction: asc or desc.
    /// </summary>
    public string? SortDirection { get; set; }

    /// <summary>
    /// Filter tasks by status (e.g. ToDo, InProgress, Done).
    /// </summary>
    //[EmailAddress]
    //[StatusValidation]
    public string? Status { get; set; }

    /// <summary>
    /// Filter tasks by priority (e.g. Low, Medium, High).
    /// </summary>
    public string? Priority { get; set; }

    /// <summary>
    /// Search text applied to task title and description.
    /// </summary>
    //[MinLength(3)]
    public string? Search { get; set; }

    /// <summary>
    /// Filter tasks by project identifier.
    /// </summary>
    public int? ProjectId { get; set; }


    public void Validate()
    {
        if (Page < 1) Page = 1;

        if (PageSize < 1) PageSize = 10;

        if (PageSize > 100) PageSize = 100;

        if (string.IsNullOrWhiteSpace(SortDirection)) 
            SortDirection = "asc";
        
        SortDirection = SortDirection.ToLower();

        if (SortDirection != "asc" && SortDirection != "desc") 
            SortDirection = "asc";
    }
}

//public class StatusValidationAttribute: ValidationAttribute
//{
//    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
//    {
//        if(value is not string status)
//        {
//            return new ValidationResult("Status must be a string");
//        }

//        if (status != "ToDo" && status != "InProgress" && status != "Done")
//        {
//            return new ValidationResult("Status must be ToDo, InProgress or Done");
//        }

//        return ValidationResult.Success;
//    }
//}