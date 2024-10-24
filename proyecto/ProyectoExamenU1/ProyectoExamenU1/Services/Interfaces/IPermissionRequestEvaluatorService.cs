using ProyectoExamenU1.Dtos.Common;
using SolicitudPermiso.Dtos.PermissionRequestEvaluator;
using SolicitudPermiso.Dtos.PermissionRequests;

namespace SolicitudPermiso.Services.Interfaces
{
    public interface IPermissionRequestEvaluatorService
    {
        Task<ResponseDto<PermissionRequestEvaluatorResponseDto>> EvaluatePermissionRequestByIdAsync(PermissionRequestEvaluatorDto dto, Guid id);
        Task<ResponseDto<List<PermissionRequestResponseDto>>> GetAllRequestsListAsync();
        Task<ResponseDto<PermissionRequestResponseDto>> GetPermissionRequestByIdAsync(Guid id);
    }
}
