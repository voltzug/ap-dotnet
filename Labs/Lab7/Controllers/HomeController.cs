using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab7.Data;
using Lab7.Models;

namespace Lab7.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ChinookDbContext _chinook;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ChinookDbContext chinook, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _chinook = chinook ?? throw new ArgumentNullException(nameof(chinook));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    // Displays a list of Chinook customers (email list). Null-safe.
    public IActionResult Index()
    {
        var customers = _chinook?.Customers?.ToList() ?? new List<Customer>();
        return View(customers);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Shows invoices for the currently logged-in user.
    [Authorize]
    public async Task<IActionResult> MyOrders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.CustomerId == null) return Forbid();

        var customerId = user.CustomerId.Value;
        var invoices = await _chinook.Invoices
            .Where(i => i.CustomerId == customerId)
            .ToListAsync();

        return View(invoices);
    }

    // Order details with ownership check and related data loaded.
    [Authorize]
    public async Task<IActionResult> OrderDetails(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.CustomerId == null) return Forbid();

        var invoice = await _chinook.Invoices
            .Include(i => i.InvoiceLines)
                .ThenInclude(il => il.Track)
            .FirstOrDefaultAsync(i => i.InvoiceId == id);

        if (invoice == null) return NotFound();
        if (invoice.CustomerId != user.CustomerId) return Forbid();

        return View(invoice);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
