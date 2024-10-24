using AutoMapper;
using BlogUNAH.API.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using ProyectoExamenU1.Constants;
using ProyectoExamenU1.Dtos.Common;
using ProyectoExamenU1.Services.Interfaces;
using SolicitudPermiso.Database.Entities;
using SolicitudPermiso.Dtos.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoExamenU1.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<Employee> _signInManager;
        private readonly UserManager<Employee> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(
            SignInManager<Employee> signInManager,
            UserManager<Employee> userManager,
            IMapper mapper,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
            )
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._mapper = mapper;
            this._configuration = configuration;
        }


        public async Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto)
        {
            var result = await _signInManager
                .PasswordSignInAsync(dto.Email,
                                     dto.Password,
                                     isPersistent: false,
                                     lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Generación del token
                var userEntity = await _userManager.FindByEmailAsync(dto.Email);

                // ClaimList creation
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, userEntity.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", userEntity.Id),
                };

                var userRoles = await _userManager.GetRolesAsync(userEntity);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var jwtToken = GetToken(authClaims);

                return new ResponseDto<LoginResponseDto>
                {
                    StatusCode = 200,
                    Status = true,
                    Message = "Inicio de sesion satisfactorio",
                    Data = new LoginResponseDto
                    {
                        Email = userEntity.Email,
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        TokenExpiration = jwtToken.ValidTo,
                    }
                };

            }

            return new ResponseDto<LoginResponseDto>
            {
                Status = false,
                StatusCode = 401,
                Message = "Fallo el inicio de sesión"
            };
        
    }

        public async Task<ResponseDto<RegisterResponseDto>> RegisterAsync (RegisterDto dto)
        {
            var emailMatch = await _userManager.FindByEmailAsync(dto.Email);
            if (emailMatch is not null)
            {
                return new ResponseDto<RegisterResponseDto>
                {
                    StatusCode = 409,
                    Status = false,
                    Message = "Un empleado con este correo electronico ya existe."
                };
            }

            var employee = new Employee
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                EmployeeJoinDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await _userManager.CreateAsync(employee, dto.Password);

            var userRoleEntity = await _roleManager.FindByIdAsync(dto.EmployeePositionId.ToString());

            if (userRoleEntity == null)
            {
                return new ResponseDto<RegisterResponseDto>
                {
                    StatusCode = 400,
                    Status = false,
                    Message = "No se pudo asignar la posición deseada al empleado porque la posición no existe en el sistema. Por favor, verifica que la posición ingresada sea válida."
                };
            }

            await _userManager.AddToRoleAsync(employee, userRoleEntity.Name);

            return new ResponseDto<RegisterResponseDto>
            {
                StatusCode = 201,
                Status = true,
                Message = $"Empleado registrado correctamente.",
                Data = new RegisterResponseDto 
                {
                    Name = employee.Name,
                    Email = employee.Email,
                    Password = dto.Password,
                    EmployeePosition = userRoleEntity.Name,
                    EmployeeJoinDate = employee.EmployeeJoinDate
                }
            };
        }

        public async Task<ResponseDto<RegisterResponseDto>> EditAsync (EmployeeEditDto dto, Guid id)
        {
            var employeeEntity = await _userManager.FindByIdAsync(id.ToString());
            if (employeeEntity == null) 
            {
                return new ResponseDto<RegisterResponseDto>
                {
                    StatusCode = 404,
                    Status = false,
                    Message = "El empleado que deseas editar no ha sido encontrado."
                };
            }

            // modificando propiedades que se pueden editar directamente
            employeeEntity.Name = dto.Name; 
            employeeEntity.Email = dto.Email;

            // obtener roles del usuario 
            var currentRoles = await _userManager.GetRolesAsync(employeeEntity);

            // eliminar todos los roles asociados al usuario
            foreach (var role in currentRoles)
            {
                await _userManager.RemoveFromRoleAsync(employeeEntity, role);
            }

            // validar si el rol existe
            var newRoleEntity = await _roleManager.FindByIdAsync(dto.EmployeePositionId.ToString());
            if (newRoleEntity == null)
            {
                return new ResponseDto<RegisterResponseDto>
                {
                    StatusCode = 400,
                    Status = false,
                    Message = "No se pudo asignar el nuevo rol porque no existe en el sistema."
                };
            }

            // asignar el nuevo rol
            await _userManager.AddToRoleAsync(employeeEntity, newRoleEntity.Name);

            // cambiar contraseña
            var token = await _userManager.GeneratePasswordResetTokenAsync(employeeEntity); // Token de restablecimiento de contraseña que se va a comprobar. https://learn.microsoft.com/es-es/dotnet/api/microsoft.aspnetcore.identity.usermanager-1.resetpasswordasync?view=aspnetcore-8.0
            var result = await _userManager.ResetPasswordAsync(employeeEntity, token, dto.Password);
            if (!result.Succeeded)
            {
                return new ResponseDto<RegisterResponseDto>
                {
                    StatusCode = 400,
                    Status = false,
                    Message = "Error al actualizar la contraseña"
                };
            }

            // guardar cambios
            var updateResult = await _userManager.UpdateAsync(employeeEntity);
            if (updateResult.Succeeded)
            {
                return new ResponseDto<RegisterResponseDto>
                {
                    StatusCode = 200,
                    Status = true,
                    Message = "Empleado actualizado correctamente. Estos son los nuevos datos.",
                    Data = new RegisterResponseDto
                    {
                        Name = employeeEntity.Name,
                        Email = employeeEntity.Email,
                        Password = dto.Password,
                        EmployeePosition = newRoleEntity.Name, 
                    }
                };
            }

            return new ResponseDto<RegisterResponseDto>
            {
                StatusCode = 400,
                Status = false,
                Message = "Error al actualizar el empleado."
            };
        }

        public async Task<ResponseDto<Employee>> DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return new ResponseDto<Employee>
                {
                    StatusCode = 404,
                    Status = false,
                    Message = "El empleado a eliminar no fue encontrado"
                };
            }

            // Obtener roles asociados al usuario
            var userRoles = await _userManager.GetRolesAsync(user);

            // Eliminar el usuario de cada rol
            foreach (var role in userRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            // Eliminar el usuario
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return new ResponseDto<Employee>
                {
                    StatusCode = 200,
                    Status = true,
                    Message = "Usuario eliminado exitosamente"
                };
            }

            return new ResponseDto<Employee>
            {
                StatusCode = 500,
                Status = false,
                Message = "No se pudo eliminar el usuario"
            };
        }


        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration["JWT:Secret"]));

            return new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey,
                    SecurityAlgorithms.HmacSha256)
            );
        }
    }
}
