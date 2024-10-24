using AutoMapper;
using ProyectoExamenU1.Helpers;
using SolicitudPermiso.Database.Entities;
using SolicitudPermiso.Dtos.Auth;


namespace ProyectoExamenU1.Helpers
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            MapsForEmployees();
        }

        private void MapsForEmployees()
        {
            CreateMap<EmployeeEditDto, Employee>();
        }

    }
}