using System.Reactive.Linq;
using ApiMaps.Models.DTOs.ApiMapConfigDtos;
using ApiMaps.Models.Entities;
using ApiMaps.Services.ApiMapConfigServices;
using Microsoft.AspNetCore.Mvc;

namespace ApiMaps.Controllers.ApiMapConfigControllers;

/// <summary>
/// Controlador para gestionar operaciones CRUD sobre la entidad <see cref="ApiMapConfig"/>.
/// </summary>
[ApiController]
[Route("api/ApiMapConfig")]
public class ApiMapConfigController : ControllerBase
{
    private readonly IApiMapConfigService _apiMapConfigService;

    /// <summary>
    /// Constructor que inyecta la capa de servicio para <see cref="ApiMapConfig"/>.
    /// </summary>
    /// <param name="apiMapConfigService">Servicio que contiene la lógica de negocio para ApiMapConfig.</param>
    public ApiMapConfigController(IApiMapConfigService apiMapConfigService)
    {
        _apiMapConfigService = apiMapConfigService ?? throw new ArgumentNullException(nameof(apiMapConfigService));
    }

    /// <summary>
    /// Crea una nueva configuración de proveedor de mapas.
    /// </summary>
    /// <param name="dto">Datos necesarios para crear la configuración.</param>
    /// <returns>
    /// 201 Created si la creación es exitosa, o 400 BadRequest en caso de error.
    /// </returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateApiMapConfigAsync([FromBody] CreateApiMapConfigDto dto)
    {
        var createdConfig = await _apiMapConfigService.CreateAsync(dto).FirstAsync();
        return CreatedAtAction("GetApiMapConfigById", new { id = createdConfig.Id }, createdConfig);
    }

    /// <summary>
    /// Obtiene una configuración de proveedor de mapas por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la configuración a buscar.</param>
    /// <returns>La configuración encontrada o NotFound si no existe.</returns>
    [HttpGet("getbyid/{id}", Name = "GetApiMapConfigById")]
    public async Task<IActionResult> GetApiMapConfigByIdAsync(int id)
    {
        var config = await _apiMapConfigService.GetByIdAsync(id).FirstAsync();
        if (config == null)
        {
            return NotFound();
        }

        return Ok(config);
    }

    /// <summary>
    /// Obtiene todas las configuraciones de proveedores de mapas.
    /// </summary>
    /// <returns>La lista de todas las configuraciones registradas.</returns>
    [HttpGet("getall")]
    public async Task<IActionResult> GetAllApiMapConfigsAsync()
    {
        var configs = await _apiMapConfigService.GetAllAsync().FirstAsync();
        return Ok(configs);
    }

    /// <summary>
    /// Actualiza una configuración de proveedor de mapas existente.
    /// </summary>
    /// <param name="dto">Datos necesarios para actualizar la configuración.</param>
    /// <returns>La configuración actualizada.</returns>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateApiMapConfigAsync([FromBody] UpdateApiMapConfigDto dto)
    {
        var updatedConfig = await _apiMapConfigService.UpdateAsync(dto).FirstAsync();
        return Ok(updatedConfig);
    }

    /// <summary>
    /// Elimina una configuración de proveedor de mapas por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la configuración a eliminar.</param>
    /// <returns>NoContent si la eliminación es exitosa.</returns>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteApiMapConfigAsync(int id)
    {
        await _apiMapConfigService.DeleteAsync(id).FirstAsync();
        return NoContent();
    }
}