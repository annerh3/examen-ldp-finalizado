namespace SolicitudPermiso.Dtos.Auth
{
    public class RegisterResponseDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string EmployeePosition { get; set; }
        public DateOnly EmployeeJoinDate { get; set; }

    }
}
