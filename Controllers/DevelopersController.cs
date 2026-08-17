using DevTrack.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

public class DevelopersController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var developers = await db.Developers.Include(x => x.Tasks).AsNoTracking().OrderByDescending(x => x.CurrentWorkload).ToListAsync();
        return View(developers);
    }
}
