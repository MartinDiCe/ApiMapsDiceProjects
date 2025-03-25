using System.Text.Json.Serialization;

namespace ApiMaps.Models.DTOs.IADtos;

/// <summary>
/// Representa la sección de horarios dentro de "result".
/// </summary>
public class OpeningHoursDto
{
    [JsonPropertyName("open_now")]
    public bool OpenNow { get; set; }

    [JsonPropertyName("weekday_text")]
    public List<string>? WeekdayText { get; set; }
}