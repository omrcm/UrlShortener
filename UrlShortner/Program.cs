using System.Data.SQLite;
using URLShortener.Data;
using UrlShortner.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IShortenedUrlRepository>(new ShortenedUrlRepository("Data Source=shorturls.sqlite;Version=3;"));
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var connection = services.GetRequiredService<IShortenedUrlRepository>() as ShortenedUrlRepository;
    connection?.CreateTable();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();