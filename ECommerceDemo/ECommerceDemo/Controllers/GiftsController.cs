using ECommerceDemo.Data;
using ECommerceDemo.Models.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // API controller attribute
    public class GiftsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GiftsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/Gifts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gift>>> GetGifts()
        {
            return await _context.Gift.Include(g => g.Category).ToListAsync();
        }

        // GET: api/Gifts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gift>> GetGift(int id)
        {
            var gift = await _context.Gift.FindAsync(id);

            if (gift == null)
            {
                return NotFound();
            }

            return gift;
        }

        // POST: api/Gifts
        [HttpPost]
        public async Task<ActionResult<Gift>> PostGift([FromBody] Gift gift, IFormFile imageFile, IFormFile videoFile, IFormFile gifFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file uploads
                var imageResult = await SaveFile(imageFile, "images", "image/");
                if (!string.IsNullOrEmpty(imageResult.error))
                {
                    ModelState.AddModelError("ImageFile", imageResult.error);
                    return BadRequest(ModelState);
                }

                var videoResult = await SaveFile(videoFile, "videos", "video/");
                if (!string.IsNullOrEmpty(videoResult.error))
                {
                    ModelState.AddModelError("VideoFile", videoResult.error);
                    return BadRequest(ModelState);
                }

                var gifResult = await SaveFile(gifFile, "gifs", "image/gif");
                if (!string.IsNullOrEmpty(gifResult.error))
                {
                    ModelState.AddModelError("GifFile", gifResult.error);
                    return BadRequest(ModelState);
                }

                // Assign file URLs
                gift.ImageUrl = imageResult.url;
                gift.VideoUrl = videoResult.url ?? gifResult.url;

                _context.Add(gift);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetGift), new { id = gift.Id }, gift);
            }

            return BadRequest(ModelState);
        }

        // PUT: api/Gifts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGift(int id, [FromBody] Gift gift, IFormFile imageFile, IFormFile videoFile, IFormFile gifFile)
        {
            if (id != gift.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file uploads if necessary
                    var imageResult = await SaveFile(imageFile, "images", "image/");
                    if (!string.IsNullOrEmpty(imageResult.error))
                    {
                        ModelState.AddModelError("ImageFile", imageResult.error);
                        return BadRequest(ModelState);
                    }

                    var videoResult = await SaveFile(videoFile, "videos", "video/");
                    if (!string.IsNullOrEmpty(videoResult.error))
                    {
                        ModelState.AddModelError("VideoFile", videoResult.error);
                        return BadRequest(ModelState);
                    }

                    var gifResult = await SaveFile(gifFile, "gifs", "image/gif");
                    if (!string.IsNullOrEmpty(gifResult.error))
                    {
                        ModelState.AddModelError("GifFile", gifResult.error);
                        return BadRequest(ModelState);
                    }

                    // Assign file URLs
                    if (!string.IsNullOrEmpty(imageResult.url)) gift.ImageUrl = imageResult.url;
                    if (!string.IsNullOrEmpty(videoResult.url) || !string.IsNullOrEmpty(gifResult.url))
                        gift.VideoUrl = videoResult.url ?? gifResult.url;

                    _context.Update(gift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiftExists(gift.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
            }

            return BadRequest(ModelState);
        }

        // DELETE: api/Gifts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGift(int id)
        {
            var gift = await _context.Gift.FindAsync(id);
            if (gift == null)
            {
                return NotFound();
            }

            _context.Gift.Remove(gift);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<(string url, string error)> SaveFile(IFormFile file, string folder, string contentTypePrefix)
        {
            if (file != null && file.Length > 0)
            {
                if (!file.ContentType.StartsWith(contentTypePrefix))
                {
                    return (null, $"Only {contentTypePrefix} files are allowed.");
                }

                var fileName = Path.GetFileName(file.FileName);
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folder, fileName);

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(uploadPath)); // Ensure folder exists
                    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                catch
                {
                    return (null, "An error occurred while uploading the file.");
                }

                return ($"/uploads/{folder}/{fileName}", null);
            }

            return (null, null);
        }

        private bool GiftExists(int id)
        {
            return _context.Gift.Any(e => e.Id == id);
        }
    }
}
