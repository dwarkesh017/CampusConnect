using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Models;

public enum ClaimStatus { Pending = 1, Approved = 2, Rejected = 3 }

public class ClaimRequest
{
    public int Id { get; set; }
    public int ItemReportId { get; set; }
    public ItemReport? ItemReport { get; set; }
    [Required]
    public string ClaimantUserId { get; set; } = string.Empty;
    public ApplicationUser? Claimant { get; set; }

    [Required, StringLength(800, MinimumLength = 10)]
    [Display(Name = "Why do you believe this item is yours?")]
    public string ClaimDescription { get; set; } = string.Empty;

    public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
