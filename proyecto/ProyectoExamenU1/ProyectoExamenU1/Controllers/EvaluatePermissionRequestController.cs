using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoExamenU1.Constants;
using ProyectoExamenU1.Database;
using ProyectoExamenU1.Dtos.Common;
using SolicitudPermiso.Dtos.PermissionRequestEvaluator;
using SolicitudPermiso.Dtos.PermissionRequests;
using SolicitudPermiso.Services;
using SolicitudPermiso.Services.Interfaces;

namespace SolicitudPermiso.Controllers
{
    [Route("api/permissions/auth/")]
    [ApiController]
    public class EvaluatePermissionRequestController : ControllerBase
    {
        private readonly IPermissionRequestEvaluatorService _permissionRequestEvaluatorService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PermissionRequestContext _context;

        public EvaluatePermissionRequestController(
            IPermissionRequestEvaluatorService permissionRequestEvaluatorService,
            RoleManager<IdentityRole> roleManager,
            PermissionRequestContext context
            )
        {
            this._permissionRequestEvaluatorService = permissionRequestEvaluatorService;
            this._roleManager = roleManager;
            this._context = context;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ResponseDto<PermissionRequestResponseDto>>> GetRequest(Guid id)
        {
            var response = await _permissionRequestEvaluatorService.GetPermissionRequestByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [Authorize(Roles = $"{RolesConstant.HUMAN_RESOURCES}")]
        public async Task<ActionResult<ResponseDto<List<PermissionRequestResponseDto>>>> GetAllRequests()
        {
            var response = await _permissionRequestEvaluatorService.GetAllRequestsListAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("evaluate/{id}")]
        [Authorize(Roles = $"{RolesConstant.HUMAN_RESOURCES}")]
        public async Task<ActionResult<ResponseDto<PermissionRequestResponseDto>>> EvaluatePermission(PermissionRequestEvaluatorDto dto, Guid id)
        {
            var response = await _permissionRequestEvaluatorService.EvaluatePermissionRequestByIdAsync(dto, id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
