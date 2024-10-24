using BlogUNAH.API.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoExamenU1.Constants;
using ProyectoExamenU1.Dtos.Common;
using ProyectoExamenU1.Services.Interfaces;
using SolicitudPermiso.Database.Entities;
using SolicitudPermiso.Dtos.Auth;

namespace BlogUNAH.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(
            IAuthService authService
            )
        {
            this._authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Login(LoginDto dto) 
        {
            var response = await _authService.LoginAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("register")]
        [Authorize(Roles = $"{RolesConstant.ADMIN}, {RolesConstant.HUMAN_RESOURCES}")]
        public async Task<ActionResult<ResponseDto<RegisterResponseDto>>> Register(RegisterDto dto)
        {
            var response = await _authService.RegisterAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("edit/{id}")]
        [Authorize(Roles = $"{RolesConstant.ADMIN}, {RolesConstant.HUMAN_RESOURCES}")]
        public async Task<ActionResult<ResponseDto<RegisterResponseDto>>> Edit(EmployeeEditDto dto, Guid id)
        {
            var response = await _authService.EditAsync(dto, id);
            return StatusCode(response.StatusCode, response);
        }


        [HttpDelete("delete/{id}")]
        [Authorize(Roles = $"{RolesConstant.ADMIN}, {RolesConstant.HUMAN_RESOURCES}")]
        public async Task<ActionResult<ResponseDto<Employee>>> Delete(Guid id)
        {
            var response = await _authService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
