using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolicitudPermiso.Database.Entities
{
    [Table("permission_types", Schema = "dbo")]
    public class PermissionTypeEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Display(Name = "Tipo de Permiso")]
        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(50)]
        [Column("name_type")]
        public string NameType { get; set; }

        [Display(Name = "Descripcion")]
        [Required(ErrorMessage = "El {0} es requerido.")]
        [StringLength(100)]
        [Column("description")]
        public string Description { get; set; }

        [Display(Name = "Días maximos")]
        [Required(ErrorMessage = "Los {0} para el permiso es requerido.")]
        [Range(1, 365, ErrorMessage = "Los {0} deben estar entre {1} y {2}.")]
        [Column("max_days")]
        public int MaxDays { get; set; }
    }
}
