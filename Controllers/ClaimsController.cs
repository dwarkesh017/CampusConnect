using CampusConnect.Data;
using CampusConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Controllers;

[Authorize]
public class ClaimsController : Controller
{
    private readonly ApplicationDbContext _db;
    public ClaimsController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Create(int itemReportId)
    {
        var item = await _db.ItemReports.FindAsync(itemReportId);
        if (item is null) return NotFound();
        if (item.UserId == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
        {
            TempData["Error"] = "You cannot claim your own report.";
            return RedirectToAction("Details", "Reports", new { id = itemReportId });
        }
        ViewBag.Item = item;
        return View(new ClaimRequest { ItemReportId = itemReportId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClaimRequest claim)
    {
        var item = await _db.ItemReports.FindAsync(claim.ItemReportId);
        if (item is null) return NotFound();
        if (item.UserId == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
            return Forbid();
        if (item.Status == ItemStatus.Returned)
        {
            ModelState.AddModelError("", "This item has already been returned.");
        }
        if (!ModelState.IsValid)
        {
            ViewBag.Item = item;
            return View(claim);
        }
        claim.ClaimantUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        claim.CreatedAt = DateTime.UtcNow;
        claim.Status = ClaimStatus.Pending;
        _db.Claims.Add(claim);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Claim submitted for administrator review.";
        return RedirectToAction("Details", "Reports", new { id = claim.ItemReportId });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var claims = await _db.Claims.Include(c => c.ItemReport).Include(c => c.Claimant)
            .OrderByDescending(c => c.CreatedAt).ToListAsync();
        return View(claims);
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(int id, ClaimStatus status)
    {
        var claim = await _db.Claims.Include(c => c.ItemReport).FirstOrDefaultAsync(c => c.Id == id);
        if (claim is null) return NotFound();
        if (status is not ClaimStatus.Approved and not ClaimStatus.Rejected) return BadRequest();
        claim.Status = status;
        if (status == ClaimStatus.Approved && claim.ItemReport is not null)
            claim.ItemReport.Status = ItemStatus.Returned;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Claim review saved.";
        return RedirectToAction(nameof(Index));
    }
}
