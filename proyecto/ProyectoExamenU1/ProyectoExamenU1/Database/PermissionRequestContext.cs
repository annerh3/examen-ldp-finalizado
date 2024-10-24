using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoExamenU1.Services.Interfaces;
using SolicitudPermiso.Database.Entities;

namespace ProyectoExamenU1.Database
{
    public class PermissionRequestContext : IdentityDbContext<Employee>
    {
        private readonly IAuditService _auditService;

        public PermissionRequestContext(
            DbContextOptions options,
            IAuditService auditService   
  
            ) 
            : base(options)
        {
            this._auditService = auditService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");
            //modelBuilder.Entity<PermitionApplicationEntity>()   // por que usa una entidad
            //.Property(e => e.Type)   // y por que el nombre
            //.UseCollation("SQL_Latin1_General_CP1_CI_AS");


            modelBuilder.HasDefaultSchema("security");

            //ASignamos nombres a nuestras tablas para no confundirnos
            modelBuilder.Entity<Employee>().ToTable("users");
            modelBuilder.Entity<IdentityRole>().ToTable("roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("users_roles");


            //Estos son los permisos
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("users_claims");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("roles_claims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("users_logins");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("users_tokens");

            // modelBuilder.ApplyConfiguration(new CategoryConfiguration());          <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
           

            //set FKs on Restrict
            //var eTypes = modelBuilder.Model.GetEntityTypes();
            //foreach (var type in eTypes)
            //{
            //    var foreingkeys = type.GetForeignKeys();
            //    foreach (var foreingkey in foreingkeys)
            //    {
            //        foreingkey.DeleteBehavior = DeleteBehavior.Restrict;
            //    }
            //}
        }
        

        //public DbSet<ENTITY_CLASS> ENTITY_NAME { get; set; }

        public DbSet<PermissionRequestEntity> PermissionRequests { get; set; }
        public DbSet<PermissionTypeEntity> PermissionTypes { get; set; }
        public DbSet<PermissionRequestStatusEntity> PermissionRequestStatus { get; set; }

    }
}
