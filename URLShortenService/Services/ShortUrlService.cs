using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using URLShortenService.Models;
using URLShortenService.Utilities;

namespace URLShortenService.Services
{
	public class ShortUrlService
	{
		private readonly IMongoCollection<UrlModel> _shortUrls;

		public ShortUrlService(IOptions<DatabaseSettings> dbSettings)
		{
			var client = new MongoClient(dbSettings.Value.ConnectionString);
			var database = client.GetDatabase(dbSettings.Value.DatabaseName);
			_shortUrls = database.GetCollection<UrlModel>(dbSettings.Value.CollectionName);
		}


		public async Task<UrlModel> CreateAsync(UrlModel url)
		{
			url.ShortUrl = UrlShortener.GenerateShortUrl();
			url.CreatedAt = DateTime.Now;
			url.UpdatedAt = DateTime.Now;
			await _shortUrls.InsertOneAsync(url);
			return url;
		}

		public async Task<UrlModel> GetAsync(string shortUrl)
		{
			var filter = Builders<UrlModel>.Filter.Eq("ShortUrl", shortUrl);
			var url = await _shortUrls.Find(filter).FirstOrDefaultAsync();
			if (url != null)
			{
				var update = Builders<UrlModel>.Update.Inc("AccessCount", 1);
				await _shortUrls.UpdateOneAsync(filter, update);
			}
			return url;
		}
		public async Task<UrlModel> GetByOriginalUrlAsync(string originalUrl)
		{
			var filter = Builders<UrlModel>.Filter.Eq("OriginalUrl", originalUrl);
			return await _shortUrls.Find(filter).FirstOrDefaultAsync();
			
		}

		public async Task<UpdateResult> UpdateAsync(string shortUrl, string updatedUrl)
		{
			var filter = Builders<UrlModel>.Filter.Eq("ShortUrl", shortUrl);
			var update = Builders<UrlModel>.Update.Set("OriginalUrl", updatedUrl).Set("UpdatedAt", DateTime.Now);
			return await _shortUrls.UpdateOneAsync(filter, update);
		}
		public async Task<DeleteResult> DeleteAsync(string shortUrl)
		{
			var filter = Builders<UrlModel>.Filter.Eq("ShortUrl", shortUrl);
			return await _shortUrls.DeleteOneAsync(filter);
		}

		public async Task<UrlModel> GetStatsAsync(string shortUrl)
		{
			var filter = Builders<UrlModel>.Filter.Eq("ShortUrl", shortUrl);
			return await _shortUrls.Find(filter).FirstOrDefaultAsync();
		}
	}
}
