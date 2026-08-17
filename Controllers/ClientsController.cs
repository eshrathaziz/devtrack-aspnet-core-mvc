using DevTrack.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

public class ClientsController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var query = db.Clients.Include(x => x.Projects).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.CompanyName.Contains(search) || x.ContactPerson.Contains(search) || x.Industry.Contains(search));
        ViewBag.Search = search ?? "";
        return View(await query.OrderBy(x => x.CompanyName).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var client = await db.Clients.Include(x => x.Projects).Include(x => x.Requirements).Include(x => x.Projects).FirstOrDefaultAsync(x => x.Id == id);
        return client is null ? NotFound() : View(client);
    }
}
