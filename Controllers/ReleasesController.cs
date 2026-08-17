using DevTrack.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

public class ReleasesController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await db.Releases.Include(x => x.Project).AsNoTracking().OrderBy(x => x.ReleaseDate).ToListAsync());
    }
}
