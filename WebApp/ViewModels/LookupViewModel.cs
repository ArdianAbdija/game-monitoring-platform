using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;


/// View model used by simple CRUD pages for lookup tables such as genres and game types.

public class LookupViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
}
