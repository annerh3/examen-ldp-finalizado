using BlogUNAH.API.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoExamenU1.Constants;
using ProyectoExamenU1.Database;
using ProyectoExamenU1.Dtos.Common;
using ProyectoExamenU1.Services.Interfaces;
using SolicitudPermiso.Database.Entities;
using SolicitudPermiso.Dtos.Auth;
using SolicitudPermiso.Dtos.PermissionRequests;
using SolicitudPermiso.Services.Interfaces;

namespace SolicitudPermiso.Controllers
{
    [Route("api/permissions")]
    [ApiController]
    public class PermissionRequestController : ControllerBase
    {
        private readonly IPermissionRequestService _permissionRequestService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PermissionRequestContext _context;

        public PermissionRequestController(
            IPermissionRequestService permissionRequestService,
             RoleManager<IdentityRole> roleManager,
            PermissionRequestContext context
            )
        {
            this._permissionRequestService = permissionRequestService;
            this._roleManager = roleManager;
            this._context = context;
        }

        [HttpPost("request")]
        [Authorize]
        public async Task<ActionResult<ResponseDto<PermissionRequestEntity>>> Register(PermissionRequestDto dto)
        {
            var response = await _permissionRequestService.RequestPermissionAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("my-requests")]
        [Authorize]
        public async Task<ActionResult<ResponseDto<List<PermissionRequestResponseDto>>>> GetAllRequests()
        {
            var response = await _permissionRequestService.GetAllMyRequestsAsync();
            return StatusCode(response.StatusCode, response);
        }

    }
}
