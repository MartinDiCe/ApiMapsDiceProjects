namespace ApiMaps.Exceptions;

public abstract class NotFoundException(string message) : Exception(message);