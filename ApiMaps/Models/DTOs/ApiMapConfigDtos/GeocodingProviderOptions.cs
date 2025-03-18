namespace ApiMaps.Models.DTOs.ApiMapConfigDtos;
public class GeocodingProviderOptions
{
    public string ProviderName { get; set; } = string.Empty;
    public string EndpointTemplate { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    
    public int Priority { get; set; }  
}
