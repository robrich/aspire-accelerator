using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspireAndPostgres.Server.Data;

[Table("weatherforecasts")]
public class WeatherForecast
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("date")]
    public DateOnly Date { get; set; }
    [Column("temperature_c")]
    public int TemperatureC { get; set; }
    [Column("summary")]
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
