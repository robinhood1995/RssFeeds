using System.ServiceModel.Syndication;
using System.Xml;

namespace RssFeeds.Services
{
    //https://www.c-sharpcorner.com/article/building-a-blazor-server-app-to-fetch-rss-feeds-using-net-core/

    public class RssFeedService
    {
        private readonly HttpClient _httpClient;

        public RssFeedService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetRssFeedAsync()
        {
          
            var response = await _httpClient.GetStringAsync("https://moxie.foxnews.com/google-publisher/latest.xml");
            return response;
        }

        public async Task<List<SyndicationItem>> GetRssFeedAsync(string feedUrl)
        {
            try
            {
                using var response = await _httpClient.GetAsync(feedUrl);
                response.EnsureSuccessStatusCode(); // This will throw if the status code is not 200-299
                using var stream = await response.Content.ReadAsStreamAsync();
                using var xmlReader = XmlReader.Create(stream);
                var syndicationFeed = SyndicationFeed.Load(xmlReader);
                return syndicationFeed?.Items.ToList() ?? new List<SyndicationItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<SyndicationItem>(); // Return empty if an error occurs
            }
        }
    }
}
