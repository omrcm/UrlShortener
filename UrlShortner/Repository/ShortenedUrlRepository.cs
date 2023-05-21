using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Dapper;
using UrlShortner;
using UrlShortner.Interface;

namespace URLShortener.Data
{
    public class ShortenedUrlRepository : IShortenedUrlRepository
    {
        private readonly string _connectionString;

        public ShortenedUrlRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public void CreateTable()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS ShortenedUrls (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        OriginalUrl TEXT NOT NULL,
                        ShortUrl TEXT NOT NULL
                    )");
            }
        }

        public ShortenedUrl GetByOriginalUrl(string originalUrl)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefault<ShortenedUrl>("SELECT * FROM ShortenedUrls WHERE OriginalUrl = @OriginalUrl", new { OriginalUrl = originalUrl });
            }
        }

        public void Create(ShortenedUrl shortenedUrl) 
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                connection.Execute("INSERT INTO ShortenedUrls (OriginalUrl, ShortUrl) VALUES (@OriginalUrl, @ShortUrl)", shortenedUrl);
            }
        }

        private IDbConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
    
}