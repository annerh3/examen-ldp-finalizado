using Microsoft.AspNetCore.Identity;

namespace SolicitudPermiso.Database.Entities
{
    public class Employee : IdentityUser
    {
        public string Name { get; set; }
        public DateOnly EmployeeJoinDate { get; set; }
    }
}
