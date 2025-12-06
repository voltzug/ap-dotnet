using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace Lab4.Pages
{
    public class SingleModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string imagesDir;

        [BindProperty(SupportsGet = true)]
        public string? Image { get; set; }

        public SingleModel(IWebHostEnvironment environment)
        {
            _environment = environment;
            imagesDir = Path.Combine(_environment.WebRootPath ?? "wwwroot", "images");
        }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(Image))
            {
                return NotFound("Nie podano nazwy pliku.");
            }

            // Decode URL-encoded file name
            var decodedImage = WebUtility.UrlDecode(Image);
            var filePath = Path.Combine(imagesDir, decodedImage);
            if (System.IO.File.Exists(filePath))
            {
                Image = decodedImage;
                return Page();
            }
            else
            {
                return NotFound("Obrazek nie istnieje.");
            }
        }

        public IActionResult OnPostDelete()
        {
            if (string.IsNullOrEmpty(Image))
            {
                return NotFound("Nie podano nazwy pliku.");
            }

            var decodedImage = WebUtility.UrlDecode(Image);
            var filePath = Path.Combine(imagesDir, decodedImage);

            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                }
                catch
                {
                    // Optionally, add error handling/logging here
                }
            }

            return RedirectToPage("Index");
        }
    }
}
