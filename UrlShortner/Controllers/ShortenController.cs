using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using URLShortener.Data;
using UrlShortner;
using UrlShortner.Interface;

namespace URLShortener.Controllers
{
    [ApiController]
    [Route("api/shorten")]
    public class ShortenController : ControllerBase
    {
        private readonly IShortenedUrlRepository _repository;
        private string _shortDomain = "http://sample.site/";

        public ShortenController(IShortenedUrlRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Shorten([FromBody] ShortenRequest request)
        {
            string originalUrl = request.Url;
            string customShortUrl = request.CustomUrl;

            if (!IsValidUrl(originalUrl))
            {
                return BadRequest("Invalid URL format.");
            }

            if (string.IsNullOrEmpty(customShortUrl))
            {
                customShortUrl = GenerateShortUrl();
            }
            else
            {
                if (!IsCustomUrlValid(customShortUrl))
                {
                    return BadRequest("Invalid custom URL format.");
                }
            }

            var existing = _repository.GetByOriginalUrl(originalUrl);
            if (existing != null)
            {
                return Ok(existing.ShortUrl);
            }
            Console.WriteLine($" customShortUrl is : {customShortUrl}");
            var newUrl = new ShortenedUrl { OriginalUrl = originalUrl, ShortUrl = customShortUrl };
            _repository.Create(newUrl);

            return Ok(customShortUrl);
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

        private bool IsCustomUrlValid(string customUrl)
        {
            return Regex.IsMatch(customUrl, @"^(https?)://[^\s/$.?#].[^\s]*$");

        }

        private string GenerateShortUrl()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var shortUrl = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
            return _shortDomain + shortUrl;
        }
    }
}
