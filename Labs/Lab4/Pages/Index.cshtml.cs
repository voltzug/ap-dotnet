using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lab4.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IWebHostEnvironment _environment;

    private string imagesDir;

    [BindProperty]
    public IFormFile? Upload { get; set; }

    public List<string> Images { get; set; } = new();

    public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;

        imagesDir = Path.Combine(environment.WebRootPath ?? "wwwroot", "images");
        try
        {
            Directory.CreateDirectory(imagesDir);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not create images directory at {ImagesDir}", imagesDir);
        }

        // Initialize shared watermark provider (loads watermark once)
        WatermarkProvider.Initialize(environment.WebRootPath ?? "wwwroot", _logger);
    }

    public void OnGet()
    {
        UpdateFileList();
    }

    private void UpdateFileList()
    {
        Images = new List<string>();
        try
        {
            if (!Directory.Exists(imagesDir))
            {
                return;
            }

            foreach (var item in Directory.EnumerateFiles(imagesDir).ToList())
            {
                Images.Add(Path.GetFileName(item));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enumerate images in {ImagesDir}", imagesDir);
        }
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
        if (Upload == null || Upload.Length == 0)
        {
            return RedirectToPage();
        }

        // Use original filename for upload (preserve user file name)
        var fileName = Path.GetFileName(Upload.FileName);
        var destPath = Path.Combine(imagesDir, fileName);

        // Check if file exists and if so, delete it before writing new file (to ensure replacement)
        if (System.IO.File.Exists(destPath))
        {
            try
            {
                System.IO.File.Delete(destPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nie można usunąć istniejącego pliku przed nadpisaniem: {Path}", destPath);
                // Optionally, return error page or ModelState error here
            }
        }

        try
        {
            await using var ms = new MemoryStream();
            await Upload.CopyToAsync(ms);
            ms.Position = 0;

            var watermarkBytes = WatermarkProvider.Get();

            byte[] outputBytes;

            using (var image = new MagickImage(ms))
            {
                if (watermarkBytes != null && watermarkBytes.Length > 0)
                {
                    using var wm = new MagickImage(watermarkBytes);

                    int targetWidth = (int)(image.Width * 0.2);
                    if (wm.Width > targetWidth && targetWidth > 0)
                    {
                        wm.Resize((uint)targetWidth, 0u);
                    }

                    try
                    {
                        wm.Evaluate(Channels.Alpha, EvaluateOperator.Multiply, 0.6);
                    }
                    catch
                    {
                        // continue if not supported
                    }

                    image.Composite(wm, Gravity.Southeast, CompositeOperator.Over);
                }

                outputBytes = image.ToByteArray();
            }

            await System.IO.File.WriteAllBytesAsync(destPath, outputBytes);
            _logger.LogInformation("Saved uploaded image to {Path}", destPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing uploaded file");
        }

        UpdateFileList();
        return RedirectToPage();
    }

    private static string GetExtensionFromContentType(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType)) return ".jpg";
        return contentType.ToLower() switch
        {
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/tiff" => ".tiff",
            "image/bmp" => ".bmp",
            "image/jpeg" => ".jpg",
            _ => ".jpg"
        };
    }
}

// Shared provider loads watermark once and serves bytes to callers.
internal static class WatermarkProvider
{
    private static byte[]? watermark;
    private static readonly object sync = new();

    public static void Initialize(string webRootPath, ILogger logger)
    {
        if (watermark != null) return;

        var path = Path.Combine(webRootPath, "watermark.png");
        if (!File.Exists(path)) return;

        lock (sync)
        {
            if (watermark != null) return;
            try
            {
                watermark = File.ReadAllBytes(path);
                logger.LogInformation("Watermark loaded from {Path}", path);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load watermark from {Path}", path);
                watermark = null;
            }
        }
    }

    public static byte[]? Get()
    {
        return watermark;
    }
}
