namespace URLShortenService.Models
{
	public class UrlResponseDto
	{
		public string Id { get; set; }
		public string OriginalUrl { get; set; }
		public string ShortUrl { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
