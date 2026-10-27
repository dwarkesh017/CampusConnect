using CampusConnect.Data;
using CampusConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Controllers;

public class ReportsController : Controller
{
    private readonly ApplicationDbContext _db;
    public ReportsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search, string? category, ReportType? type, ItemStatus? status)
    {
        var query = _db.ItemReports.Include(r => r.User).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(r => r.Title.Contains(term) || r.Description.Contains(term) || r.Location.Contains(term));
        }
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(r => r.Category == category);
        if (type.HasValue) query = query.Where(r => r.ReportType == type.Value);
        if (status.HasValue) query = query.Where(r => r.Status == status.Value);
        ViewBag.Search = search;
        ViewBag.Category = category;
        ViewBag.Type = type;
        ViewBag.Status = status;
        ViewBag.Categories = await _db.ItemReports.Select(r => r.Category).Distinct().OrderBy(x => x).ToListAsync();
        return View(await query.OrderByDescending(r => r.CreatedAt).ToListAsync());
    }

    [Authorize]
    public IActionResult Create() => View(new ItemReport { EventDate = DateTime.Today });

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ItemReport report)
    {
        report.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        report.CreatedAt = DateTime.UtcNow;
        report.Status = ItemStatus.Pending;
        if (!ModelState.IsValid) return View(report);
        _db.ItemReports.Add(report);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Your report was submitted.";
        return RedirectToAction(nameof(Details), new { id = report.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var report = await _db.ItemReports.Include(r => r.User).Include(r => r.Claims)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (report is null) return NotFound();
        return View(report);
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var report = await _db.ItemReports.FindAsync(id);
        if (report is null) return NotFound();
        if (!await CanManage(report)) return Forbid();
        return View(report);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ItemReport input)
    {
        if (id != input.Id) return BadRequest();
        var report = await _db.ItemReports.FindAsync(id);
        if (report is null) return NotFound();
        if (!await CanManage(report)) return Forbid();
        if (!ModelState.IsValid) return View(input);
        report.Title = input.Title; report.Description = input.Description; report.Category = input.Category;
        report.ReportType = input.ReportType; report.Location = input.Location; report.EventDate = input.EventDate;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Report updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var report = await _db.ItemReports.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
        if (report is null) return NotFound();
        if (!await CanManage(report)) return Forbid();
        return View(report);
    }

    [HttpPost, ActionName("Delete"), Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var report = await _db.ItemReports.Include(r => r.Claims).FirstOrDefaultAsync(r => r.Id == id);
        if (report is null) return NotFound();
        if (!await CanManage(report)) return Forbid();
        _db.ItemReports.Remove(report);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Report deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> CanManage(ItemReport report) =>
        report.UserId == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        || User.IsInRole("Admin");
}
