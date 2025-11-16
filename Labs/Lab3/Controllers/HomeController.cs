using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Lab3.Models;

namespace Lab3.Controllers;

public class HomeController : Controller
{
    private readonly PhoneBookService _phoneBook;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, PhoneBookService phoneBook)
    {
        _logger = logger;
        _phoneBook = phoneBook;
    }

    public IActionResult Index()
    {
        Random r = new Random();
        ViewData["random"] = r.NextDouble();
        return View();
    }

    public IActionResult Index2()
    {
        return View(_phoneBook.GetContacts());
    }

    public IActionResult Delete(int id)
    {
        if (_phoneBook.Remove(id))
        {
            return RedirectToAction("Index2");
        }
        return NotFound();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Contact contact)
    {
        if (ModelState.IsValid)
        {
            _phoneBook.Add(contact);
            return RedirectToAction("Index2");
        }
        return View();
    }
}