using Microsoft.EntityFrameworkCore;
using ApiMaps.Models.Base;
using ApiMaps.Models.Entities;
using ApiMaps.Services.AuditServices;

namespace ApiMaps.Data
{
    /// <summary>
    /// Representa el contexto de datos principal de la aplicación, 
    /// encargado de administrar la conexión a la base de datos y la configuración de las entidades.
    /// </summary>
    /// <param name="options">Opciones de configuración para el DbContext.</param>
    /// <param name="auditEntitiesService">
    /// Servicio que se encarga de aplicar auditoría a las entidades derivadas de <see cref="AuditEntities"/>.
    /// </param>
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditEntitiesService auditEntitiesService)
        : DbContext(options)
    {

        /// <summary>
        /// Conjunto de datos para la entidad <see cref="Parameter"/>.
        /// </summary>
        public DbSet<Parameter> Parameters { get; set; } = null!;

        /// <summary>
        /// Conjunto de datos para la entidad <see cref="ApiTrace"/>.
        /// </summary>
        public DbSet<ApiTrace> ApiTraces { get; set; } = null!;
        
        /// <summary>
        /// Conjunto de datos para la entidad <see cref="ServicioEjecucion"/>.
        /// </summary>
        public DbSet<ServicioEjecucion> ServicioEjecucion { get; set; } = null!;
        
        /// <summary>   
        /// Guarda los cambios en la base de datos, aplicando previamente la auditoría a las entidades modificadas.
        /// </summary>
        /// <returns>El número de registros afectados.</returns>
        public override int SaveChanges()
        {
            auditEntitiesService.ApplyAudit(ChangeTracker.Entries<AuditEntities>());
            return base.SaveChanges();
        }
    }
}
