using System.ComponentModel.DataAnnotations;

namespace SolicitudPermiso.Dtos.PermissionRequestEvaluator
{
    public class PermissionRequestEvaluatorDto
    {

        [Display(Name = "Id del estado de Solicitud")]
        [Required(ErrorMessage = "El {0} es requerido.")]
        public Guid PermissionStatusId { get; set; }

        [Display(Name = "Detalles de evaluación")]
        [Required(ErrorMessage = "Los {0} es requerido.")]
        public string ResolutionDetail { get; set; }
    }
}
