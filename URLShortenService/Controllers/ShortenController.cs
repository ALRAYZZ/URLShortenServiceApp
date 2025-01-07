using Microsoft.AspNetCore.Mvc;
using URLShortenService.Models;
using URLShortenService.Services;

namespace URLShortenService.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ShortenController : Controller
	{
		public readonly ShortUrlService _shortUrlService;

		public ShortenController(ShortUrlService shortUrlService)
		{
			_shortUrlService = shortUrlService;
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateUrlRequest createUrl)
		{
			// Check if the URL is valid
			if (string.IsNullOrEmpty(createUrl.OriginalUrl) || !Uri.IsWellFormedUriString(createUrl.OriginalUrl, UriKind.Absolute))
			{
				return BadRequest("Invalid URL format.");
			}
			// Check if the URL already exists
			var existingUrl = await _shortUrlService.GetByOriginalUrlAsync(createUrl.OriginalUrl);
			if (existingUrl != null)
			{
				return Conflict("URL already exists.");
			}
			var url = new UrlModel
			{
				OriginalUrl = createUrl.OriginalUrl
			};

			var result = await _shortUrlService.CreateAsync(url);
			var responseDto = new UrlResponse
			{
				Id = result.Id,
				OriginalUrl = result.OriginalUrl,
				ShortUrl = result.ShortUrl,
				CreatedAt = result.CreatedAt,
				UpdatedAt = result.UpdatedAt
			};
			return CreatedAtAction(nameof(Get), new { shortUrl = result.ShortUrl }, responseDto);
		}
		[HttpGet("{shortUrl}")]
		public async Task<IActionResult> Get(string shortUrl)
		{
			var result = await _shortUrlService.GetAsync(shortUrl);
			if (result == null)
			{
				return NotFound();
			}
			var responseDto = new UrlResponse
			{
				Id = result.Id,
				OriginalUrl = result.OriginalUrl,
				ShortUrl = result.ShortUrl,
				CreatedAt = result.CreatedAt,
				UpdatedAt = result.UpdatedAt
			};
			return Ok(responseDto);
		}
		[HttpPut("{shortUrl}")]
		public async Task<IActionResult> Update(string shortUrl, [FromBody] CreateUrlRequest updatedUrl)
		{
			// Check if the short URL exists
			var existingUrl = await _shortUrlService.GetAsync(shortUrl);
			if (existingUrl == null)
			{
				return NotFound();
			}

			// Check if the updated URL is valid
			if (string.IsNullOrEmpty(updatedUrl.OriginalUrl) || !Uri.IsWellFormedUriString(updatedUrl.OriginalUrl, UriKind.Absolute))
			{
				return BadRequest("Invalid URL format.");
			}

			var duplicateUrl = await _shortUrlService.GetByOriginalUrlAsync(updatedUrl.OriginalUrl);
			if (duplicateUrl != null)
			{
				return Conflict("URL already exists.");
			}
			await _shortUrlService.UpdateAsync(shortUrl, updatedUrl.OriginalUrl);
			return Ok();
		}
		[HttpDelete("{shortUrl}")]
		public async Task<IActionResult> Delete(string shortUrl)
		{
			var result = await _shortUrlService.GetAsync(shortUrl);
			if (result == null)
			{
				return NotFound();
			}
			await _shortUrlService.DeleteAsync(shortUrl);
			return NoContent();
		}
		[HttpGet("{shortUrl}/stats")]
		public async Task<IActionResult> GetStats(string shortUrl)
		{
			var result = await _shortUrlService.GetStatsAsync(shortUrl);
			if (result == null)
			{
				return NotFound();
			}
			var statsDto = new UrlStatsResponse
			{
				Id = result.Id,
				ShortUrl = result.ShortUrl,
				OriginalUrl = result.OriginalUrl,
				CreatedAt = result.CreatedAt,
				UpdatedAt = result.UpdatedAt,
				AccessCount = result.AccessCount
			};
			return Ok(statsDto);
		}
	}
}
