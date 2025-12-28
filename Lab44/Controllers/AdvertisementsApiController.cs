using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvertisementServiceMVC2.Models;

namespace AdvertisementServiceMVC2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementsApiController : ControllerBase
    {
        private readonly AdvertisementServiceContext _context;

        public AdvertisementsApiController(AdvertisementServiceContext context)
        {
            _context = context;
        }

        // GET: api/AdvertisementsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAdvertisements()
        {
            // Используем .Select, чтобы вернуть только нужные данные и избежать циклических ссылок JSON
            return await _context.Advertisements
                .Include(a => a.Category)
                .Include(a => a.User)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Price,
                    a.Category.CategoryName, // Смысловое значение
                    a.User.UserName,             // Смысловое значение
                    a.Status
                })
                .ToListAsync();
        }

        // GET: api/AdvertisementsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Advertisement>> GetAdvertisement(int id)
        {
            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement == null) return NotFound();
            return advertisement;
        }

        // POST: api/AdvertisementsApi
        [HttpPost]
        public async Task<ActionResult<Advertisement>> PostAdvertisement(Advertisement advertisement)
        {
            _context.Advertisements.Add(advertisement);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetAdvertisement", new { id = advertisement.Id }, advertisement);
        }

        // PUT: api/AdvertisementsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdvertisement(int id, Advertisement advertisement)
        {
            if (id != advertisement.Id) return BadRequest();
            _context.Entry(advertisement).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/AdvertisementsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvertisement(int id)
        {
            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement == null) return NotFound();
            _context.Advertisements.Remove(advertisement);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}