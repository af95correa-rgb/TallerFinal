using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Interfaces;
using EmployeeManagementAPI.Models;
using EmployeeManagementAPI.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IJwtService _jwtService;

        public AuthController(IRepository<User> userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Registrar un nuevo usuario
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO registerDto)
        {
            // Validar que el usuario no exista
            if (await _userRepository.ExistsAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest(new { message = "El nombre de usuario ya existe" });
            }

            if (await _userRepository.ExistsAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { message = "El email ya está registrado" });
            }

            // Crear nuevo usuario
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FullName = registerDto.FullName,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(user);

            // Generar tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Guardar refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            var response = new AuthResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            return Ok(response);
        }

        /// <summary>
        /// Iniciar sesión
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDto)
        {
            // Buscar usuario
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new { message = "Usuario desactivado" });
            }

            // Generar tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Guardar refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            var response = new AuthResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            return Ok(response);
        }

        /// <summary>
        /// Renovar el access token usando el refresh token
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDto)
        {
            // Validar el token expirado
            var principal = _jwtService.GetPrincipalFromExpiredToken(refreshTokenDto.Token);
            if (principal == null)
            {
                return BadRequest(new { message = "Token inválido" });
            }

            var username = principal.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Token inválido" });
            }

            // Buscar usuario
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || user.RefreshToken != refreshTokenDto.RefreshToken)
            {
                return BadRequest(new { message = "Refresh token inválido" });
            }

            // Verificar que el refresh token no haya expirado
            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest(new { message = "Refresh token expirado" });
            }

            // Generar nuevos tokens
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Actualizar refresh token
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            var response = new AuthResponseDTO
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            return Ok(response);
        }

        /// <summary>
        /// Cerrar sesión (invalidar refresh token)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Usuario no encontrado" });
            }

            var user = await _userRepository.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userRepository.UpdateAsync(user);
            }

            return Ok(new { message = "Sesión cerrada exitosamente" });
        }

        /// <summary>
        /// Obtener información del usuario actual
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult> GetCurrentUser()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var user = await _userRepository.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.FullName,
                user.Role,
                user.CreatedAt
            });
        }
    }
}