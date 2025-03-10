namespace ApiMaps.Models.DTOs.ParameterDtos;

public class UpdateParameterDto
{
    public int ParameterId { get; set; }
    public string ParameterName { get; set; }
    public string ParameterValue { get; set; }
    public string? ParameterDescription { get; set; }
    public string ParameterCategory { get; set; }
}
