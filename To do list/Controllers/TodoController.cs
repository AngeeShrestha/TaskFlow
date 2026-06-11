using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers;

[Authorize]
public class TodoController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TodoController(AppDbContext db, UserManager<ApplicationUser> u)
    { _db = db; _userManager = u; }

    private string UserId => _userManager.GetUserId(User)!;

    public async Task<IActionResult> Index(string? filter, string? category, string? search)
    {
        var query = _db.TodoItems.Where(t => t.UserId == UserId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Title.Contains(search) || (t.Description != null && t.Description.Contains(search)));

        query = filter switch
        {
            "active" => query.Where(t => !t.IsCompleted),
            "done" => query.Where(t => t.IsCompleted),
            "overdue" => query.Where(t => !t.IsCompleted && t.DueDate < DateTime.Today),
            "today" => query.Where(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value.Date == DateTime.Today),
            _ => query
        };

        if (!string.IsNullOrEmpty(category) && Enum.TryParse<Category>(category, out var cat))
            query = query.Where(t => t.Category == cat);

        var items = await query.OrderBy(t => t.SortOrder).ThenBy(t => t.DueDate).ToListAsync();

        ViewBag.Filter = filter ?? "all";
        ViewBag.Category = category ?? "";
        ViewBag.Search = search ?? "";
        return View(items);
    }

    [HttpGet] public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoItem item)
    {
        if (!ModelState.IsValid) return View(item);
        item.UserId = UserId;
        item.SortOrder = await _db.TodoItems.Where(t => t.UserId == UserId).CountAsync();
        _db.TodoItems.Add(item);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoItem item)
    {
        if (id != item.Id) return BadRequest();
        var existing = await _db.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        if (existing == null) return NotFound();
        if (!ModelState.IsValid) return View(item);

        existing.Title = item.Title;
        existing.Description = item.Description;
        existing.Priority = item.Priority;
        existing.Category = item.Category;
        existing.DueDate = item.DueDate;
        if (item.IsCompleted && !existing.IsCompleted) existing.CompletedAt = DateTime.UtcNow;
        existing.IsCompleted = item.IsCompleted;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleComplete(int id)
    {
        var item = await _db.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        if (item != null)
        {
            item.IsCompleted = !item.IsCompleted;
            item.CompletedAt = item.IsCompleted ? DateTime.UtcNow : null;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        if (item != null) { _db.TodoItems.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    // AJAX endpoint for drag-to-reorder
    [HttpPost]
    public async Task<IActionResult> Reorder([FromBody] List<int> orderedIds)
    {
        var items = await _db.TodoItems.Where(t => t.UserId == UserId && orderedIds.Contains(t.Id)).ToListAsync();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            var item = items.FirstOrDefault(t => t.Id == orderedIds[i]);
            if (item != null) item.SortOrder = i;
        }
        await _db.SaveChangesAsync();
        return Ok();
    }
}