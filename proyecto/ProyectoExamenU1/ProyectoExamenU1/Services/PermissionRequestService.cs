using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoExamenU1.Constants;
using ProyectoExamenU1.Database;
using ProyectoExamenU1.Dtos.Common;
using ProyectoExamenU1.Services;
using ProyectoExamenU1.Services.Interfaces;
using SolicitudPermiso.Constants;
using SolicitudPermiso.Database.Entities;
using SolicitudPermiso.Dtos.PermissionRequests;
using SolicitudPermiso.Services.Interfaces;

namespace SolicitudPermiso.Services
{
    public class PermissionRequestService : IPermissionRequestService
    {
        private readonly PermissionRequestContext _context;
        private readonly IAuditService _auditservice;
        private readonly ILogger<PermissionRequestService> _logger;
        private readonly UserManager<Employee> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PermissionRequestService(PermissionRequestContext context,
           IAuditService auditservice,
           ILogger<PermissionRequestService> logger,
           UserManager<Employee> userManager,
           IMapper mapper,
           IConfiguration configuration
            )
        {
            this._context = context;
            this._auditservice = auditservice;
            this._logger = logger;
            this._userManager = userManager;
            this._mapper = mapper;
            this._configuration = configuration;
        }


        public async Task<ResponseDto<PermissionRequestResponseDto>> RequestPermissionAsync (PermissionRequestDto dto)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);


            // Validar que la fecha de inicio sea anterior o igual a la fecha de fin
            if (dto.StartDate > dto.EndDate)
            {
                return new ResponseDto<PermissionRequestResponseDto>
                {
                    StatusCode = 400,
                    Status = false,
                    Message = "Fechas no validas. La fecha de inicio debe ser menor a la fecha final."
                };
     
            }

            if (dto.StartDate < currentDate.AddDays(VariousConstants.MINIMUMDAYSNOTICE))
            {
                return new ResponseDto<PermissionRequestResponseDto>
                {
                    StatusCode = 400,
                    Status = false,
                    Message = $"No puedes hacer esta solicitud ahora. Tienes que hacerla con {VariousConstants.MINIMUMDAYSNOTICE} días de anticipacion"
                };
            }
            var employeeId = _auditservice.GetUserId(); // Obtiene el ID del usuario autenticado
            if (employeeId == null)
            {
                return new ResponseDto<PermissionRequestResponseDto>
                {
                    StatusCode = 404,
                    Status = false,
                    Message = "Error al obtener Id del usuario."
                };
            }
            var employeeEntity = await _userManager.FindByIdAsync(employeeId);

            var permissionTypeEntity = await _context.PermissionTypes.FindAsync(dto.PermissionTypeId);

            if(permissionTypeEntity == null)
            {
                return new ResponseDto<PermissionRequestResponseDto>
                {
                    StatusCode = 400,
                    Status = false,
                    Message = "El tipo de permiso no fue encontrado."
                };
            }


            // Calcular la diferencia en días
            var daysDifference = (dto.EndDate.ToDateTime(TimeOnly.MinValue) - dto.StartDate.ToDateTime(TimeOnly.MinValue)).Days;
            var max = permissionTypeEntity.MaxDays;

            if (daysDifference > permissionTypeEntity.MaxDays)
            {
                return new ResponseDto<PermissionRequestResponseDto>
                {
                    StatusCode = 400,
                    Status = false,
                    Message = "La cantidad de días que estás pidiendo exceden al máximo de días para este tipo de permiso."
                };
            }
            string permissionStatusPendingId = "d0362608-8e81-427b-9cd1-ac0d23c68186"; 
            Guid permissionTypeId = Guid.Parse(permissionStatusPendingId);

            var permissionStatusPendingEntity = await _context.PermissionRequestStatus.FindAsync(permissionTypeId);

            var permissionRequestEntity = new PermissionRequestEntity
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                PermissionTypeId = permissionTypeEntity.Id,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Motive = dto.Motive,
                PermissionStatusId = permissionStatusPendingEntity.Id,
            };

            _context.PermissionRequests.Add(permissionRequestEntity);
            await _context.SaveChangesAsync();

            var permissionRequestResponseDto = new PermissionRequestResponseDto
            {
                PermissionRequestId = permissionRequestEntity.Id,
                EmployeeId = permissionStatusPendingEntity.Id,
                PermissionTypeName = permissionTypeEntity.NameType,
                StartDate = permissionRequestEntity.StartDate,
                EndDate = permissionRequestEntity.EndDate,
                Motive = permissionRequestEntity.Motive,
                PermissionStatusName = permissionStatusPendingEntity.StateName

            };

            return new ResponseDto<PermissionRequestResponseDto>
            {
                StatusCode = 201,
                Status = true,
                Message = "Solicitud de Permiso realizada correctamente.",
                Data = permissionRequestResponseDto
            };
        }
        
        public async Task<ResponseDto<List<PermissionRequestResponseDto>>> GetAllMyRequestsAsync()
        {
            var employeeId = _auditservice.GetUserId();
            if (employeeId == null)
            {
                return new ResponseDto<List<PermissionRequestResponseDto>>
                {
                    StatusCode = 404,
                    Status = false,
                    Message = "Error al obtener Id del usuario."
                };
            }
            var permissionRequestsEntity = await _context.PermissionRequests
                .Include(p => p.PermissionType) // se usa Include para cargar relaciones entre entidades de forma explícita
                .Include(p => p.PermissionStatus)
                .Where(p => p.EmployeeId == employeeId)
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
                Message = "Lista de solicitudes de permiso obtenidas correctamente.",
                Data = dtos
            };
        }

    }
}
