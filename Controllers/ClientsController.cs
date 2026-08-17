using DevTrack.Data;
using DevTrack.Models;
using DevTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Controllers;

[Authorize(Roles = "Administrator,Project Manager")]
public class ClientsController(ApplicationDbContext context, IActivityLogService activityLog) : Controller
{
    public async Task<IActionResult> Index(string? search, ClientStatus? status, string sort = "name", int page = 1)
    {
        const int pageSize = 10;
        var query = context.Clients.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(c => c.CompanyName.Contains(search) || c.ContactPerson.Contains(search) || c.Email.Contains(search));
        if (status.HasValue) query = query.Where(c => c.Status == status);
        query = sort switch { "created" => query.OrderByDescending(c => c.CreatedAtUtc), "industry" => query.OrderBy(c => c.Industry), _ => query.OrderBy(c => c.CompanyName) };
        var count = await query.CountAsync();
        ViewBag.Search = search; ViewBag.Status = status; ViewBag.Sort = sort; ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        return View(await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
    }

    public IActionResult Create() => View(new Client());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Client client)
    {
        if (await context.Clients.AnyAsync(c => c.CompanyName == client.CompanyName)) ModelState.AddModelError(nameof(client.CompanyName), "A client with this company name already exists.");
        if (!ModelState.IsValid) return View(client);
        context.Clients.Add(client); await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Client Created", "Client", client.Id.ToString(), $"Created client {client.CompanyName}.");
        TempData["Success"] = "Client created."; return RedirectToAction(nameof(Details), new { id = client.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var client = await context.Clients.Include(c => c.Projects).Include(c => c.Requirements).Include(c => c.Communications).FirstOrDefaultAsync(c => c.Id == id);
        return client is null ? NotFound() : View(client);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var client = await context.Clients.FindAsync(id); return client is null ? NotFound() : View(client);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Client client)
    {
        if (id != client.Id) return BadRequest();
        if (!ModelState.IsValid) return View(client);
        context.Update(client); await context.SaveChangesAsync();
        await activityLog.RecordAsync(User.Identity?.Name ?? "System", "Client Updated", "Client", client.Id.ToString(), $"Updated client {client.CompanyName}.");
        TempData["Success"] = "Client updated."; return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCommunication(ClientCommunication communication)
    {
        if (!ModelState.IsValid) return RedirectToAction(nameof(Details), new { id = communication.ClientId });
        communication.Employee = User.Identity?.Name; communication.OccurredAtUtc = DateTime.UtcNow;
        context.ClientCommunications.Add(communication); await context.SaveChangesAsync();
        await activityLog.RecordAsync(communication.Employee ?? "System", "Communication Logged", "Client", communication.ClientId.ToString(), communication.Subject);
        TempData["Success"] = "Communication recorded."; return RedirectToAction(nameof(Details), new { id = communication.ClientId });
    }
}
