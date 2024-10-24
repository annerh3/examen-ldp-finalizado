using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoExamenU1.Database;
using ProyectoExamenU1.Dtos.Common;
using ProyectoExamenU1.Services.Interfaces;
using SolicitudPermiso.Database.Entities;
using SolicitudPermiso.Dtos.PermissionRequestEvaluator;
using SolicitudPermiso.Dtos.PermissionRequests;
using SolicitudPermiso.Services.Interfaces;

namespace SolicitudPermiso.Services
{
    public class PermissionRequestEvaluatorService : IPermissionRequestEvaluatorService
    {
        private readonly PermissionRequestContext _context;
        private readonly IAuditService _auditservice;
        private readonly ILogger<PermissionRequestService> _logger;
        private readonly UserManager<Employee> _userManager;
        private readonly IConfiguration _configuration;

        public PermissionRequestEvaluatorService(
            PermissionRequestContext context,
           IAuditService auditservice,
           ILogger<PermissionRequestService> logger,
           UserManager<Employee> userManager,
           IConfiguration configuration
            )
        {
            this._context = context;
            this._auditservice = auditservice;
            this._logger = logger;
            this._userManager = userManager;
            this._configuration = configuration;
        }

        public async Task<ResponseDto<List<PermissionRequestResponseDto>>> GetAllRequestsListAsync()
        {
            
            var permissionRequestsEntity = await _context.PermissionRequests
                .Include(p => p.PermissionType) 
                .Include(p => p.PermissionStatus)
                .ToListAsync();

            if (permissionRequestsEntity == null || !permissionRequestsEntity.Any())
            {
                return new ResponseDto<List<PermissionRequestResponseDto>>
                {
                    StatusCode = 404,
                    Status = false,
                    Message = "No se han encontrado solicitudes de permiso para este usuario."
                };
            }



            var dtos = permissionRequestsEntity.Select(p => new PermissionRequestResponseDto
            {
                PermissionRequestId = p.Id,
                EmployeeId = Guid.Parse(p.EmployeeId),
                PermissionTypeName = p.PermissionType?.NameType ?? "Tipo de permiso no encontrado",
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Motive = p.Motive,
                PermissionStatusName = p.PermissionStatus?.StateName ?? "Estado de permiso no encontrado"
            }).ToList();



            return new ResponseDto<List<PermissionRequestResponseDto>>
            {
                StatusCode = 302,
                Status = true,
                Message = "Lista de todas las solicitudes de permiso obtenidas correctamente.",
                Data = dtos
            };
        }

        public async Task<ResponseDto<PermissionRequestResponseDto>> GetPermissionRequestByIdAsync(Guid id)
        {
            var permissionRequestEntity = await _context.PermissionRequests
                .Include(p => p.PermissionType)   
                .Include(p => p.PermissionStatus) 
                .FirstOrDefaultAsync(p => p.Id == id);

            if (permissionRequestEntity == null)
            {
                return new ResponseDto<PermissionRequestResponseDto>
                {
                    StatusCode = 404,
                    Status = false,
                    Message = "No se han encontrado la solicitud de permiso."
                };
            }

            var dto = new PermissionRequestResponseDto
            {
                PermissionRequestId = id,
                EmployeeId = Guid.Parse(permissionRequestEntity.EmployeeId),
                PermissionTypeName = permissionRequestEntity.PermissionType.NameType, 
                StartDate = permissionRequestEntity.StartDate,
                EndDate = permissionRequestEntity.EndDate,
                Motive = permissionRequestEntity.Motive,
                PermissionStatusName = permissionRequestEntity.PermissionStatus.StateName 
            };

            return new ResponseDto<PermissionRequestResponseDto>
            {
                StatusCode = 302,
                Status = true,
                Message = "Solicitud de permiso encontrada exitosamente.",
                Data = dto
            };

        }

        public async Task<ResponseDto<PermissionRequestEvaluatorResponseDto>> EvaluatePermissionRequestByIdAsync(PermissionRequestEvaluatorDto dto, Guid id)
        {
            var permissionRequestEntity = await _context.PermissionRequests
                .Include(p => p.PermissionType)
                .Include(p => p.PermissionStatus)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (permissionRequestEntity == null)
            {
                return new ResponseDto<PermissionRequestEvaluatorResponseDto>
                {
                    StatusCode = 404,
                    Status = false,
                    Message = "No se ha encontrado la solicitud de permiso."
                };
            }

            // Actualizar el estado de la solicitud de permiso y los detalles de la evaluación
            permissionRequestEntity.PermissionStatusId = dto.PermissionStatusId;
            permissionRequestEntity.ResolutionDetail = dto.ResolutionDetail;
           // permissionRequestEntity.EvaluationDate = DateTime.UtcNow; // Registrar la fecha de evaluación

            // Guardar los cambios en la base de datos
            _context.PermissionRequests.Update(permissionRequestEntity);
            await _context.SaveChangesAsync();

            // Recuperar la entidad actualizada para obtener los datos más recientes
            permissionRequestEntity = await _context.PermissionRequests
                .Include(p => p.PermissionType)
                .Include(p => p.PermissionStatus)
                .FirstOrDefaultAsync(p => p.Id == id);

            // Preparar el DTO de respuesta
            var responseDto = new PermissionRequestEvaluatorResponseDto
            {
                EmployeeId = Guid.Parse(permissionRequestEntity.EmployeeId),
                PermissionTypeName = permissionRequestEntity.PermissionType.NameType, 
                StartDate = permissionRequestEntity.StartDate,
                EndDate = permissionRequestEntity.EndDate,
                Motive = permissionRequestEntity.Motive,
                PermissionStatusName = permissionRequestEntity.PermissionStatus.StateName, 
                ResolutionDetail = permissionRequestEntity.ResolutionDetail
            };

          
            return new ResponseDto<PermissionRequestEvaluatorResponseDto>
            {
                StatusCode = 200,
                Status = true,
                Message = "La solicitud de permiso ha sido evaluada correctamente.",
                Data = responseDto
            };
        }
    }
}
