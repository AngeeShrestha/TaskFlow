using Microsoft.AspNetCore.Identity;
using To_do_list.Models;

namespace TodoApp.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}