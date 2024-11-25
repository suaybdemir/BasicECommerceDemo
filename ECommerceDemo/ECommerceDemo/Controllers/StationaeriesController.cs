using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerceDemo.Data;
using ECommerceDemo.Models.Concrete;

namespace ECommerceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationaeriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StationaeriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Stationaeries
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var stationaeries = _context.Stationaery.Include(s => s.Category);
            return View(await stationaeries.ToListAsync());
        }

        // GET: Stationaeries/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest(new { message = "Invalid ID" });

            var stationaery = await _context.Stationaery
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (stationaery == null)
                return NotFound(new { message = "Stationery not found" });

            return View(stationaery);
        }

        // GET: Stationaeries/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Stock,CategoryId")] Stationaery stationaery, IFormFile image, IFormFile video)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", stationaery.CategoryId);
                return View(stationaery);
            }

            try
            {
                // Upload files and assign URLs
                if (image != null)
                    stationaery.ImageUrl = await UploadFileAsync(image, "images");

                if (video != null)
                    stationaery.VideoUrl = await UploadFileAsync(video, "videos");

                // Save to database
                _context.Add(stationaery);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", stationaery.CategoryId);
                return View(stationaery);
            }
        }

        // GET: Stationaeries/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest(new { message = "Invalid ID" });

            var stationaery = await _context.Stationaery.FindAsync(id);
            if (stationaery == null)
                return NotFound(new { message = "Stationery not found" });

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", stationaery.CategoryId);
            return View(stationaery);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Stock,CategoryId")] Stationaery stationaery, IFormFile image, IFormFile video)
        {
            if (id != stationaery.Id)
                return BadRequest(new { message = "ID mismatch" });

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", stationaery.CategoryId);
                return View(stationaery);
            }

            try
            {
                // Upload files and update URLs if new files are provided
                if (image != null)
                    stationaery.ImageUrl = await UploadFileAsync(image, "images");
                if (video != null)
                    stationaery.VideoUrl = await UploadFileAsync(video, "videos");

                // Update database
                _context.Update(stationaery);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationaeryExists(stationaery.Id))
                    return NotFound(new { message = "Stationery not found" });

                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", stationaery.CategoryId);
                return View(stationaery);
            }
        }

        // Helper: File Upload
        private async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length <= 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".mp4" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException($"File type {extension} is not allowed");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(path)); // Ensure folder exists

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folder}/{fileName}";
        }

        private bool StationaeryExists(int id)
        {
            return _context.Stationaery.Any(e => e.Id == id);
        }
    }
}
