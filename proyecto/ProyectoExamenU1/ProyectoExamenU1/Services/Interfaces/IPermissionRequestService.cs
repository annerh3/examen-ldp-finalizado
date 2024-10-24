using ProyectoExamenU1.Dtos.Common;
using SolicitudPermiso.Database.Entities;
using SolicitudPermiso.Dtos.PermissionRequests;

namespace SolicitudPermiso.Services.Interfaces
{
    public interface IPermissionRequestService
    {
        Task<ResponseDto<List<PermissionRequestResponseDto>>> GetAllMyRequestsAsync();
        Task<ResponseDto<PermissionRequestResponseDto>> RequestPermissionAsync(PermissionRequestDto dto);
    }
}
