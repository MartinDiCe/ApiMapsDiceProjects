# API Maps - Geocodificación y Aproximación con IA

Esta extensión de la API base incorpora funcionalidades de geolocalización, aproximación de direcciones y consulta a múltiples proveedores de mapas (por ejemplo, Google Maps) de forma 100% reactiva. Se integra también una capa de inteligencia artificial (IA) para refinar la precisión en el futuro. Todo esto se gestiona mediante una arquitectura en capas, con repositorios (acceso a datos), servicios (lógica de negocio) y controladores (exposición de endpoints). Además, se aplican principios SOLID, documentación XML, manejo de errores y un robusto sistema de logging.

---

## Tabla de Contenidos

1. [Características Principales](#características-principales)
2. [Arquitectura y Flujo de Datos](#arquitectura-y-flujo-de-datos)
3. [Instalación y Configuración](#instalación-y-configuración)
4. [Principales Endpoints](#principales-endpoints)
5. [Ejemplo de Uso](#ejemplo-de-uso)
6. [Mejoras de Inteligencia Artificial](#mejoras-de-inteligencia-artificial)
7. [Contribuciones](#contribuciones)

---

## Características Principales

### Reactividad Pura con Rx
- Toda la lógica interna (servicios, repositorios, orquestadores) utiliza Reactive Extensions (Rx), facilitando la asincronía y la composición de operaciones reactivas.

### Diseño en Capas con Principios SOLID
- **Repositorios** (Acceso a datos con Entity Framework Core).
- **Servicios** (Orquestación y lógica de negocio).
- **Controladores** (Exposición de Endpoints HTTP).

### Configuración Dinámica de Proveedores
- Cada proveedor tiene su endpoint, API key y prioridad, almacenados en base de datos.
- Un servicio de configuración de proveedores (`ProviderConfigurationService`) extrae estos datos y los inyecta en los servicios de geocodificación.

### Agregador de Proveedores
- Posibilidad de unificar resultados de distintos proveedores según su prioridad.
- Métodos para obtener un único resultado (prioridad mínima), todos los resultados o agrupar por proveedor.

### Logging y Manejo de Errores
- Middleware para capturar excepciones y registrar logs de cada request/response.
- Servicio de trazas (`ApiTrace`) que registra el contenido de las solicitudes y respuestas, el tiempo de ejecución y el código HTTP resultante.

### Documentación XML y Swagger
- Comentarios detallados en cada clase, interfaz y método.
- Swagger habilitado en entornos de desarrollo para explorar y probar los endpoints.

### Diseño para Escalabilidad
- Patrón Fábrica (`GeocodingProviderFactory`) para crear dinámicamente proveedores de geocodificación.
- Mecanismo de sembrado de datos (`ParameterSeeder`) para registrar parámetros iniciales.

---

## Arquitectura y Flujo de Datos

### Controllers
- Reciben las solicitudes HTTP y validan la entrada.
- Delegan la lógica a los servicios, y retornan el resultado.

### Services
- Encapsulan la lógica de negocio (por ejemplo, `ParameterService`, `ApiMapConfigService`, `GeocodeService`).
- Se suscriben a repositorios u otros servicios de forma reactiva.
- Aplica validaciones y maneja errores, registrándolos en el logger.

### Repositories
- Conectan con la base de datos usando Entity Framework Core.
- Ofrecen métodos CRUD (Create, Read, Update, Delete) para cada entidad (`ApiMapConfig`, `Parameter`, etc.).

### Middleware
- **RequestResponseLoggingMiddleware**: Registra el cuerpo de la solicitud, el de la respuesta y el código HTTP de salida.
- **ApiTraceMiddleware**: Captura la traza final y la envía al `ApiTraceBus`, que la persiste en la base de datos de forma asíncrona.
- **ExceptionMiddleware**: Maneja de forma centralizada las excepciones, devolviendo códigos de error apropiados (404, 400, 500...).

### Almacenamiento de Configuraciones
- **Tabla `Parameters`** para valores globales (por ejemplo, `TimeoutGlobal`, `ApiTraceEnabled`).
- **Tabla `ApiMapConfigs`** para los distintos proveedores de geocodificación (endpoint, clave, prioridad...).

### Geocodificación
1. El `GeocodeService` hace llamadas al `GeocodingProviderAggregatorService`.
2. Este agregador invoca a uno o más proveedores (instancias de `IGeocodingProvider`) usando su endpoint y API key.
3. Retorna los resultados unificados, filtrados o agrupados según corresponda.

---

## Instalación y Configuración

### 1. Clona el Repositorio

```bash
git clone https://github.com/MartinDiCe/ApiASPNETBaseDiCe.git
cd ApiASPNETBaseDiCe
```

### 2. Restaura los Paquetes NuGet

```bash
dotnet restore
```

### 3. Configura la Base de Datos

Edita `appsettings.json` y modifica la cadena de conexión `"DefaultConnection"`.

El archivo `Program.cs` ejecuta `DatabaseInitializer.Initialize(...)`, que crea la base si no existe y siembra los parámetros por defecto.

### 4. Ejecuta la Aplicación

```bash
dotnet run
```

### 5. Verifica los Endpoints

Swagger estará disponible en entornos de desarrollo en:  
`https://<host>:<puerto>/swagger`

---

## Principales Endpoints

### API Base (Parámetros)

- **Crear Parámetro**  
  `POST /api/Parameter/create`

- **Obtener Parámetro por ID**  
  `GET /api/Parameter/getbyid/{id}`

- **Obtener Parámetro por Nombre**  
  `GET /api/Parameter/getbyname/{name}`

- **Actualizar Parámetro**  
  `PUT /api/Parameter/update`

---

### API Maps (Configuración de Proveedores)

- **Crear Configuración**  
  `POST /api/ApiMapConfig/create`

- **Obtener Configuración por ID**  
  `GET /api/ApiMapConfig/getbyid/{id}`

- **Obtener Todas**  
  `GET /api/ApiMapConfig/getall`

- **Actualizar Configuración**  
  `PUT /api/ApiMapConfig/update`

- **Eliminar Configuración**  
  `DELETE /api/ApiMapConfig/delete/{id}`

---

### API Geo (Geocodificación)

- **Geo Orchestrator**  
  `GET /api/geocode/process?address={direccion}&mode={modo}&priorities={lista}`

#### Modos Disponibles

- `mode=first`: Primer resultado exitoso según prioridad.
- `mode=all`: Todos los resultados disponibles.
- `mode=group`: Agrupa los resultados según prioridades (ej. priorities=1,2).

---

## Ejemplo de Uso

**Petición:**

```http
GET /api/geocode/process?address=Calle%20Falsa%20123&mode=first
```

**Respuesta:**

```json
{
  "status": "OK",
  "results": [
    {
      "formattedAddress": "Calle Falsa 123, Ciudad, País",
      "location": { "lat": 40.1234, "lng": -3.1234 }
    }
  ]
}
```

---

## Mejoras de Inteligencia Artificial

- Conexión futura a modelos de IA o servicios externos.
- Corrección ortográfica y desambiguación de direcciones.
- Priorización contextual de resultados.
- Integración con API Places para puntos de interés.

---

## Contribuciones

¡Las contribuciones son bienvenidas!

1. Haz un fork del repositorio.
2. Crea una rama para tus cambios.
3. Envía un Pull Request.

Por favor, seguí las convenciones del proyecto.

---

### Contacto

**Autor Principal**: Mdice  
**Email**: [mdice@diceprojects.com](mailto:mdice@diceprojects.com)