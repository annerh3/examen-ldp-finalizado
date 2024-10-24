using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SolicitudPermiso.Database.Entities
{
    [Table("permission_request_status", Schema = "dbo")]
    public class PermissionRequestStatusEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Display(Name = "Nombre de Estado")]
        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(50)]
        [Column("state_name")]
        public string StateName { get; set; }

        [Display(Name = "Descripción de Estado")]
        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(100)]
        [Column("state_description")]
        public string StateDescription { get; set; }
    }
}
