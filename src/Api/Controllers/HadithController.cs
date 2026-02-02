using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using The9Books.Models;

namespace The9Books.Controllers
{
    [ApiController]
    [Route("")]
    public class HadithController : ControllerBase
    {
        private readonly IDBContext _dbContext;
        private readonly IRandom _random;
        private readonly ILogger<HadithController> _logger;
        private readonly ApiSettings _apiSettings;
        private static readonly Book[] _books;

        static HadithController()
        {
            _books = new[]
            {
                new Book("bukhari", "صحيح البخاري", "Sahih Bukhari", 7008),
                new Book("muslim", "صحيح مسلم", "Sahih Muslim", 5362),
                new Book("muwataa", "موطأ مالك", "Al Muwatta", 1594),
                new Book("abidawud", "سنن أبي داود", "Sunan Abu Dawud", 4590),
                new Book("ibnmaja", "سنن ابن ماجة", "Sunan Ibn Maja", 4332),
                new Book("musnad", "مسند أحمد بن حنبل", "Musnad Ahmad ibn Hanbal", 26363),
                new Book("tirmidhi", "سنن الترمذي", "Sunan al Tirmidhi", 3891),
                new Book("nasai", "سنن النسائي", "Sunan al-Nasai", 5662),
                new Book("darimi", "سنن الدارمي", "Sunan al Darimi", 3367),
            };
        }

        public HadithController(IDBContext dbContext, IRandom random, ILogger<HadithController> logger, IOptions<ApiSettings> apiSettings)
        {
            _dbContext = dbContext;
            _random = random;
            _logger = logger;
            _apiSettings = apiSettings.Value;
        }

        [Route("books")]
        [HttpGet]
        [ResponseCache(Duration = 3600)] // Cache for 1 hour
        public ActionResult Books()
        {
            _logger.LogInformation("Retrieving list of all books");
            return Ok(_books);
        }

        [Route("{bookId}/{num}")]
        [HttpGet]
        [ProducesResponseType(typeof(HadithDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HadithDto>> Get(
            [Required] string bookId, 
            [Range(1, int.MaxValue, ErrorMessage = "Hadith number must be greater than 0")] int num)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = _books.FirstOrDefault(x => x.Id.Equals(bookId.Trim(), StringComparison.OrdinalIgnoreCase));

            if (book == null)
            {
                _logger.LogWarning("Book not found: {BookId}", bookId);
                return NotFound(new { error = "Invalid bookId" });
            }
            
            if (num > book.HadithCount)
            {
                _logger.LogWarning("Hadith number {Num} exceeds maximum {Max} for book {BookId}", num, book.HadithCount, bookId);
                return NotFound(new { error = $"Hadith number exceeds maximum of {book.HadithCount}" });
            }

            _logger.LogInformation("Retrieving hadith {Num} from book {BookId}", num, bookId);
            var hadith = await _dbContext
                .Hadiths
                .FirstOrDefaultAsync(x => x.Book == book.Id && x.Number == num);

            if (hadith == null)
            {
                _logger.LogWarning("Hadith {Num} not found in book {BookId}", num, bookId);
                return NotFound(new { error = "Hadith not found" });
            }

            return Ok(new HadithDto(hadith));
        }

        [Route("{bookId}/{start}/{size}")]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<HadithDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<HadithDto>>> List(
            [Required] string bookId, 
            [Range(1, int.MaxValue, ErrorMessage = "Start must be greater than 0")] int start, 
            [Range(1, int.MaxValue, ErrorMessage = "Size must be greater than 0")] int size)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = _books.FirstOrDefault(x => x.Id.Equals(bookId.Trim(), StringComparison.OrdinalIgnoreCase));

            if (book == null)
            {
                _logger.LogWarning("Book not found: {BookId}", bookId);
                return NotFound(new { error = "Invalid bookId" });
            }
            
            if (start > book.HadithCount)
            {
                _logger.LogWarning("Start position {Start} exceeds maximum {Max} for book {BookId}", start, book.HadithCount, bookId);
                return NotFound(new { error = $"Start position exceeds maximum hadith count of {book.HadithCount}" });
            }

            var maxSize = _apiSettings.MaxPageSize;
            start = start <= 0 ? 1 : start;
            size = (size <= 0 || size > maxSize) ? maxSize : size;

            _logger.LogInformation("Retrieving {Size} hadiths starting from {Start} in book {BookId}", size, start, bookId);
            
            var totalCount = await _dbContext.Hadiths
                .CountAsync(x => x.Book == book.Id);

            var hadiths = await _dbContext.Hadiths
                .Where(x => x.Book == book.Id)
                .OrderBy(x => x.Number)
                .Skip(start - 1)
                .Take(size)
                .ToListAsync();

            var result = new PagedResult<HadithDto>
            {
                Data = hadiths.Select(x => new HadithDto(x)),
                TotalCount = totalCount,
                Start = start,
                Size = hadiths.Count,
                HasMore = (start + hadiths.Count - 1) < totalCount
            };

            return Ok(result);
        }

        [Route("random/{bookId?}")]
        [HttpGet]
        [ProducesResponseType(typeof(HadithDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HadithDto>> Random(string bookId = "bukhari")
        {
            if (string.IsNullOrEmpty(bookId)) bookId = "bukhari";
            var book = _books.FirstOrDefault(x => x.Id.Equals(bookId.Trim(), StringComparison.OrdinalIgnoreCase));

            if (book == null)
            {
                _logger.LogWarning("Book not found for random hadith: {BookId}", bookId);
                return NotFound(new { error = "Invalid bookId" });
            }

            var hadithNumber = _random.RandPositive(book.HadithCount);
            _logger.LogInformation("Retrieving random hadith {Num} from book {BookId}", hadithNumber, bookId);

            var hadith = await _dbContext
                .Hadiths
                .FirstOrDefaultAsync(x => x.Book == book.Id && x.Number == hadithNumber);

            if (hadith == null)
            {
                _logger.LogWarning("Random hadith {Num} not found in book {BookId}", hadithNumber, bookId);
                return NotFound(new { error = "Hadith not found" });
            }

            return Ok(new HadithDto(hadith));
        }
    }
}