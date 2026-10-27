using CampusConnect.Data;
using CampusConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    public AdminController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.Reports = await _db.ItemReports.CountAsync();
        ViewBag.PendingClaims = await _db.Claims.CountAsync(c => c.Status == ClaimStatus.Pending);
        ViewBag.Users = await _db.Users.CountAsync();
        return View(await _db.ItemReports.Include(r => r.User).OrderByDescending(r => r.CreatedAt).Take(20).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, ItemStatus status)
    {
        var report = await _db.ItemReports.FindAsync(id);
        if (report is null) return NotFound();
        report.Status = status;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Item status updated.";
        return RedirectToAction(nameof(Index));
    }
}
