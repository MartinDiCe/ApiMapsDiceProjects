# API Base ASP.NET Core

Esta API es una base sólida para la construcción de aplicaciones basadas en ASP.NET Core. Se han implementado funcionalidades esenciales como gestión de parámetros, trazabilidad de API, logging, manejo centralizado de excepciones y middleware para request/response. Además, se utiliza un enfoque reactivo para la ejecución de operaciones.

## Características

- **Controladores y Endpoints:**  
  Implementa endpoints para la creación, actualización, consulta y eliminación de parámetros.

- **Base de Datos:**  
  Configuración de **Entity Framework Core** con SQL Server para el manejo de datos a través de un `DbContext` personalizado.

- **Logging:**  
  Servicio de logging configurable para entornos de desarrollo y producción.

- **Middleware:**  
  Manejo de excepciones y trazabilidad a través de middleware personalizado, registrando peticiones y respuestas.

- **Swagger:**  
  Documentación interactiva de la API mediante Swagger, activada en entornos de desarrollo.

- **Ejecución Reactiva:**  
  Uso de **Reactive Extensions (Rx)** para encapsular la lógica asíncrona y reactiva en la capa de servicios.

## Arquitectura

La solución se estructura en capas, separando claramente la lógica de negocio, acceso a datos y la configuración de la API:

- **Controllers:**  
  Se encargan de recibir las peticiones HTTP, validar la entrada y delegar la lógica de negocio a los servicios.

- **Services:**  
  Implementan la lógica de negocio, validaciones y orquestación de operaciones, utilizando un patrón reactivo para la ejecución.

- **Repositories:**  
  Manejan la persistencia de datos con Entity Framework Core, ofreciendo métodos CRUD sobre las entidades.

- **Middleware:**  
  Se han creado middlewares personalizados para la trazabilidad de API, logging de requests/responses y manejo global de excepciones.

- **Configuración:**  
  Se configura la API mediante un archivo `Program.cs` que centraliza el registro de servicios, la configuración de Swagger, logging y la inicialización de la base de datos.

## Instalación y Configuración

1. **Clonar el repositorio:**

   ```bash
   git clone https://github.com/MartinDiCe/ApiASPNETBaseDiCe.git
   cd ApiASPNETBaseDiCe
   ```

2. **Restaurar paquetes NuGet:**

   Asegúrate de tener instalado el SDK de .NET 8.0 (o la versión correspondiente) y ejecuta:

   ```bash
   dotnet restore
   ```

3. **Configuración de la Base de Datos:**

   Actualiza la cadena de conexión en el archivo `appsettings.json` con los datos de tu servidor SQL Server.

4. **Ejecutar la Aplicación:**

   Inicia la API con el comando:

   ```bash
   dotnet run
   ```

## Uso

### Swagger:

En entornos de desarrollo, Swagger se activa en la ruta `/swagger`, permitiendo explorar y probar los endpoints de forma interactiva.

### Endpoints Principales:

- **Crear Parámetro:**  
  `POST /api/Parameter/create`  
  Crea un nuevo parámetro global.

- **Obtener Parámetro por ID:**  
  `GET /api/Parameter/getbyid/{id}`

- **Obtener Parámetro por Nombre:**  
  `GET /api/Parameter/getbyname/{name}`

- **Actualizar Parámetro:**  
  `PUT /api/Parameter/update`  
  Actualiza la información de un parámetro existente.

## Contribuciones

Si deseas mejorar esta API, siéntete libre de realizar un fork del repositorio y enviar tus propuestas mediante pull requests.  
Recuerda mantener las convenciones de código y la estructura establecida.

## Licencia

MIT License
