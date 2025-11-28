using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    /// <summary>
    /// Modelo de Usuario para autenticación
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Nombre de usuario único
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email del usuario
        /// </summary>
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hash de la contraseña (nunca almacenar en texto plano)
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        [MaxLength(200)]
        public string? FullName { get; set; }

        /// <summary>
        /// Rol del usuario (Admin, User, etc.)
        /// </summary>
        [MaxLength(50)]
        public string Role { get; set; } = "User";

        /// <summary>
        /// Refresh Token para renovar el JWT
        /// </summary>
        [MaxLength(500)]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Fecha de expiración del Refresh Token
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}