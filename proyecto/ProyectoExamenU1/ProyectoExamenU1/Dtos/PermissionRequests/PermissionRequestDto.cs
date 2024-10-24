using SolicitudPermiso.Database.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SolicitudPermiso.Dtos.PermissionRequests
{
    public class PermissionRequestDto
    {

        [Display(Name = "Tipo Solicitud de Permiso Id")]
        [Required(ErrorMessage = "El {0} es requerido.")]
        public Guid PermissionTypeId { get; set; }

        [Display(Name = "Fecha de Inicio")]
        [Required(ErrorMessage = "La {0} es requerida.")]
        public DateOnly StartDate { get; set; }

        [Display(Name = "Fecha de Fin")]
        [Required(ErrorMessage = "La {0} es requerida.")]
        public DateOnly EndDate { get; set; }

        [Display(Name = "Motivo")]
        [StringLength(200, ErrorMessage = "El {0} no puede tener más de {1} caracteres.")]
        public string Motive { get; set; }
    
    }
}
