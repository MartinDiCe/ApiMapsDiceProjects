using System.Text.RegularExpressions;

namespace ApiMaps.Helpers;

/// <summary>
/// Proporciona métodos de ayuda para limpiar y normalizar direcciones.
/// </summary>
public static class AddressHelper
{
    /// <summary>
    /// Limpia una dirección eliminando espacios extra, caracteres no permitidos y normalizando el formato.
    /// Se permiten letras, dígitos, espacios, comas, puntos, guiones, barras y el símbolo #.
    /// </summary>
    /// <param name="address">La dirección a limpiar.</param>
    /// <returns>La dirección limpia y normalizada.</returns>
    public static string CleanAddress(string address)
    {
        // Elimina espacios en blanco iniciales y finales.
        string cleaned = address.Trim();

        // Elimina cualquier carácter que no esté en el conjunto permitido.
        // Permitimos: letras, dígitos, espacios, comas, puntos, guiones, barras y '#'.
        cleaned = Regex.Replace(cleaned, @"[^a-zA-Z0-9\s,.\-/#]", string.Empty);

        // Reemplaza múltiples espacios consecutivos por uno solo.
        cleaned = Regex.Replace(cleaned, @"\s+", " ");

        return cleaned;
    }
}