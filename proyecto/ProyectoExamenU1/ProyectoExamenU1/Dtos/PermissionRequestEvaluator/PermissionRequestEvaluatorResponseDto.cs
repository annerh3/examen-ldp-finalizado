namespace SolicitudPermiso.Dtos.PermissionRequestEvaluator
{
    public class PermissionRequestEvaluatorResponseDto
    {
        public Guid EmployeeId { get; set; }
        public string PermissionTypeName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Motive { get; set; }
        public string PermissionStatusName { get; set; }
        public string ResolutionDetail { get; set; }
    }
}
