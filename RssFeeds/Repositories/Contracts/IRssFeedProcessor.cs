using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;
using RssFeeds.Models;
using Microsoft.EntityFrameworkCore;

namespace RssFeeds.Repositories.Contracts
{
    public interface IRssFeedProcessor
    {
        Task ProcessFeeds();
    }
    public class RssFeedProcessor : IRssFeedProcessor
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RssFeedProcessor> _logger;
        private readonly NewsFeedContext _dbContext;

        public RssFeedProcessor(HttpClient httpClient, ILogger<RssFeedProcessor> logger, NewsFeedContext dbContext)
        {
            _httpClient = httpClient;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task ProcessFeeds()
        {
            var rssFeedUrls = await _dbContext.NewsFeedUrls.ToListAsync(); // Fetch URLs from database.

            foreach (var feedUrl in rssFeedUrls)
            {
                try
                {
                    var response = await _httpClient.GetStringAsync(feedUrl.NewsFeedUrl1);
                    var rssXml = XDocument.Parse(response);
                    ProcessRssItems(rssXml);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error retrieving or processing RSS feed: {feedUrl.NewsFeedUrl1}");
                }
            }
            await _dbContext.SaveChangesAsync(); // Save the changes after processing all feeds.
        }

        private void ProcessRssItems(XDocument rssXml)
        {
            var items = rssXml.Descendants("item");
            foreach (var item in items)
            {
                var chkLink = item.Element("link")?.Value;
                
                if (_dbContext.NewsFeeds.Any(d => d.Link == chkLink)) continue;

                var title = item.Element("title")?.Value;
                var link = item.Element("link")?.Value;
                var pubDateString = item.Element("pubDate")?.Value;
                var description = item.Element("description")?.Value;

                if (DateTime.TryParse(pubDateString, out DateTime pubDate))
                {
                    _dbContext.NewsFeeds.Add(new NewsFeed { Title = title, Link = link, PublicationDate = pubDate, Description = description });
                }
                else
                {
                    _logger.LogWarning($"Could not parse pubDate: {pubDateString}");
                }

            }
        }
    }
}
