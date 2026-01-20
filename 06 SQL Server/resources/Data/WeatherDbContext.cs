using Microsoft.EntityFrameworkCore;

namespace AspireAndPostgres.Server.Data;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
        : base(options)
    {
    }
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();
}
