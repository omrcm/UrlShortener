namespace UrlShortner.Interface;

public interface IShortenedUrlRepository
{
    ShortenedUrl GetByOriginalUrl(string originalUrl);
    void Create(ShortenedUrl shortenedUrl);
}