namespace URLShortenService.Utilities
{
	public class UrlShortener
	{
		public static string GenerateShortUrl(int lenght = 6)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var random = new Random();
			return new string(Enumerable.Repeat(chars, lenght)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}
