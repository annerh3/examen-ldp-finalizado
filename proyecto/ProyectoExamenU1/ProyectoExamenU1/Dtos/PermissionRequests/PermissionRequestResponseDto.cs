namespace SolicitudPermiso.Dtos.PermissionRequests
{
    public class PermissionRequestResponseDto
    {
        public Guid PermissionRequestId {  get; set; }
        public Guid EmployeeId { get; set; }
        public string PermissionTypeName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Motive { get; set; }
        public string PermissionStatusName { get; set; }
    }
}
