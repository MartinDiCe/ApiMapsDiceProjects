using ApiMaps.Models.DTOs.ParameterDtos;
using ApiMaps.Models.Entities;

namespace ApiMaps.Services.ParameterServices
{
    /// <summary>
    /// Define las operaciones para la gestión de parámetros globales.
    /// </summary>
    public interface IParameterService
    {
        /// <summary>
        /// Crea un nuevo parámetro global.
        /// </summary>
        /// <param name="dto">Datos necesarios para crear el parámetro.</param>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite el parámetro creado.
        /// </returns>
        IObservable<Parameter> CreateAsync(CreateParameterDto dto);

        /// <summary>
        /// Obtiene un parámetro global por su identificador.
        /// </summary>
        /// <param name="parameterId">El identificador del parámetro.</param>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite el parámetro si se encuentra; de lo contrario, <c>null</c>.
        /// </returns>
        IObservable<Parameter?> GetByIdAsync(int parameterId);

        /// <summary>
        /// Obtiene un parámetro global por su nombre.
        /// </summary>
        /// <param name="parameterName">El nombre del parámetro.</param>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite el parámetro si se encuentra; de lo contrario, <c>null</c>.
        /// </returns>
        IObservable<Parameter?> GetByNameAsync(string parameterName);

        IObservable<Parameter> UpdateAsync(UpdateParameterDto dto);
    }
}