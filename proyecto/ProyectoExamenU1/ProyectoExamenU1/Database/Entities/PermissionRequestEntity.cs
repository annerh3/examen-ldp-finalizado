using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolicitudPermiso.Database.Entities
{
    [Table("permission_requests", Schema = "dbo")]
    public class PermissionRequestEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Display(Name = "Empleado Id")]
        [Required(ErrorMessage = "El {0} es requerido.")]
        [Column("employee_id")]
        public string EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }

        [Display(Name = "Tipo Solicitud de Permiso Id")]
        [Required(ErrorMessage = "El {0} es requerido.")]
        [Column("permission_type_id")] 
        public Guid PermissionTypeId { get; set; }

        [ForeignKey(nameof(PermissionTypeId))]
        public virtual PermissionTypeEntity PermissionType { get; set; }

        [Display(Name = "Fecha de Inicio")]
        [Required(ErrorMessage = "La {0} es requerida.")]
        [Column("start_date")]
        public DateOnly StartDate { get; set; }

        [Display(Name = "Fecha de Fin")]
        [Required(ErrorMessage = "La {0} es requerida.")]
        [Column("end_date")] 
        public DateOnly EndDate { get; set; }

        [Display(Name = "Motivo")]
        [StringLength(500, ErrorMessage = "El {0} no puede tener más de {1} caracteres.")]
        [Column("motive")] 
        public string Motive { get; set; }

        [Display(Name = "Estado de Solicitud")]
        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(50, ErrorMessage = "El {0} no puede tener más de {1} caracteres.")]
        [Column("permission_status_id")]
        public Guid PermissionStatusId { get; set; }

        [ForeignKey(nameof(PermissionStatusId))]
        public virtual PermissionRequestStatusEntity PermissionStatus { get; set; }

        [Display(Name = "Detalle de Resolución")]
        [StringLength(50, ErrorMessage = "El {0} no puede tener más de {1} caracteres.")]
        [Column("resolution_detail")]
        public string ResolutionDetail { get; set; }

    }
}
