using CampusConnect.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalReports = await _db.ItemReports.CountAsync();
        ViewBag.LostCount = await _db.ItemReports.CountAsync(x => x.ReportType == Models.ReportType.Lost);
        ViewBag.FoundCount = await _db.ItemReports.CountAsync(x => x.ReportType == Models.ReportType.Found);
        var recent = await _db.ItemReports.Include(x => x.User)
            .OrderByDescending(x => x.CreatedAt).Take(6).ToListAsync();
        return View(recent);
    }

    public IActionResult Privacy() => View();
    public IActionResult Error() => View();
}
