using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ImageMagick;
using Microsoft.Extensions.Logging;

namespace Lab4.Pages
{
    public class UploadModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UploadModel> _logger;
        private readonly string imagesDir;

        public UploadModel(IWebHostEnvironment environment, ILogger<UploadModel> logger)
        {
            _environment = environment;
            _logger = logger;

            imagesDir = Path.Combine(environment.WebRootPath ?? "wwwroot", "images");
            try
            {
                Directory.CreateDirectory(imagesDir);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not ensure images directory exists at {Path}", imagesDir);
            }

            // Ensure watermark provider is initialized (loads watermark once if present)
            WatermarkProvider.Initialize(environment.WebRootPath ?? "wwwroot", logger);
        }

        // Bound file from the form
        [BindProperty]
        public IFormFile? Upload { get; set; }

        // If the file already exists, client may set this to true to confirm overwrite
        [BindProperty]
        public bool Overwrite { get; set; }

        // When a collision is detected, this property holds the name shown on the page
        public string? ExistingFileName { get; set; }

        public void OnGet()
        {
            // Intentionally empty
        }

        // Async upload handler. Uses in-memory processing, applies watermark if available,
        // and writes the final bytes to disk with a single async write.
        public async Task<IActionResult> OnPostAsync()
        {
            if (Upload == null || Upload.Length == 0)
            {
                return RedirectToPage("Index");
            }

            // Use original filename; detect extension from content type when possible
            var originalName = Path.GetFileName(Upload.FileName) ?? "uploaded";
            var extension = GetExtensionFromContentType(Upload.ContentType);
            if (string.IsNullOrEmpty(extension))
            {
                extension = Path.GetExtension(originalName);
                if (string.IsNullOrEmpty(extension))
                {
                    extension = ".jpg";
                }
            }

            var baseName = Path.GetFileNameWithoutExtension(originalName);
            var fileName = baseName + extension;
            var destPath = Path.Combine(imagesDir, fileName);

            // If file exists and user did not confirm overwrite, show confirmation state
            if (System.IO.File.Exists(destPath) && !Overwrite)
            {
                ExistingFileName = fileName;
                ModelState.AddModelError(string.Empty, "Plik o tej nazwie już istnieje. Potwierdź nadpisanie, zaznaczając opcję i wyślij ponownie.");
                return Page();
            }

            try
            {
                await using var ms = new MemoryStream();
                await Upload.CopyToAsync(ms);
                ms.Position = 0;

                var watermarkBytes = WatermarkProvider.Get();

                byte[] outputBytes;

                // Apply watermark (if available) using Magick.NET in-memory
                using (var image = new MagickImage(ms))
                {
                    if (watermarkBytes != null && watermarkBytes.Length > 0)
                    {
                        using var wm = new MagickImage(watermarkBytes);

                        // Scale watermark to a fraction of image width
                        int targetWidth = (int)(image.Width * 0.2);
                        if (targetWidth > 0 && wm.Width > targetWidth)
                        {
                            wm.Resize((uint)targetWidth, 0u);
                        }

                        try
                        {
                            wm.Evaluate(Channels.Alpha, EvaluateOperator.Multiply, 0.6);
                        }
                        catch
                        {
                            // If evaluate fails for some format, continue without adjusting alpha
                        }

                        image.Composite(wm, Gravity.Southeast, CompositeOperator.Over);
                    }

                    outputBytes = image.ToByteArray();
                }

                // Single asynchronous write to disk
                await System.IO.File.WriteAllBytesAsync(destPath, outputBytes);

                _logger.LogInformation("Uploaded file saved to {Path}", destPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing uploaded file {FileName}", fileName);
                ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas przesyłania pliku.");
                return Page();
            }

            return RedirectToPage("Index");
        }

        private static string GetExtensionFromContentType(string? contentType)
        {
            if (string.IsNullOrEmpty(contentType)) return string.Empty;
            return contentType.ToLower() switch
            {
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/tiff" => ".tiff",
                "image/bmp" => ".bmp",
                "image/jpeg" => ".jpg",
                _ => string.Empty
            };
        }
    }
}