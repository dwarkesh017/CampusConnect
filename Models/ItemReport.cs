using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Models;

public enum ReportType { Lost = 1, Found = 2 }
public enum ItemStatus { Pending = 1, Matched = 2, Returned = 3 }

public class ItemReport
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Item name")]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(1000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Report type")]
    public ReportType ReportType { get; set; } = ReportType.Lost;

    [Required, StringLength(150)]
    public string Location { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    [Display(Name = "Date lost/found")]
    public DateTime EventDate { get; set; } = DateTime.Today;

    public ItemStatus Status { get; set; } = ItemStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public List<ClaimRequest> Claims { get; set; } = new();
}
