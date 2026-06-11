using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(AppDbContext db, UserManager<ApplicationUser> u)
    { _db = db; _userManager = u; }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var tasks = await _db.TodoItems.Where(t => t.UserId == userId).ToListAsync();

        ViewBag.Total = tasks.Count;
        ViewBag.Completed = tasks.Count(t => t.IsCompleted);
        ViewBag.Active = tasks.Count(t => !t.IsCompleted);
        ViewBag.Overdue = tasks.Count(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate < DateTime.Today);
        ViewBag.HighPrio = tasks.Count(t => !t.IsCompleted && t.Priority == Priority.High);
        ViewBag.DueToday = tasks.Count(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value.Date == DateTime.Today);

        ViewBag.ByCategory = tasks
            .GroupBy(t => t.Category.ToString())
            .Select(g => new { Category = g.Key, Count = g.Count(), Done = g.Count(t => t.IsCompleted) })
            .ToList();

        ViewBag.RecentTasks = tasks
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .ToList();

        return View();
    }
}