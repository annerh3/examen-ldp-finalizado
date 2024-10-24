using BlogUNAH.API.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using ProyectoExamenU1.Dtos.Common;
using SolicitudPermiso.Database.Entities;
using SolicitudPermiso.Dtos.Auth;

namespace ProyectoExamenU1.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto);
        Task<ResponseDto<RegisterResponseDto>> RegisterAsync(RegisterDto dto);
        Task<ResponseDto<RegisterResponseDto>> EditAsync(EmployeeEditDto dto, Guid id);
        Task<ResponseDto<Employee>> DeleteAsync(Guid id);
    }
}
