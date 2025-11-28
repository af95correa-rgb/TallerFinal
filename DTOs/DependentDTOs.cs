using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.DTOs
{
    /// <summary>
    /// DTO para crear un dependiente
    /// </summary>
    public class CreateDependentDTO
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "La relación es requerida")]
        [StringLength(50)]
        public string Relationship { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Gender { get; set; }

        [StringLength(50)]
        public string? IdentificationNumber { get; set; }

        [Required(ErrorMessage = "El ID del empleado es requerido")]
        public int EmployeeId { get; set; }
    }

    /// <summary>
    /// DTO para actualizar un dependiente
    /// </summary>
    public class UpdateDependentDTO
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(50)]
        public string? Relationship { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

        [StringLength(50)]
        public string? IdentificationNumber { get; set; }
    }

    /// <summary>
    /// DTO de respuesta de dependiente
    /// </summary>
    public class DependentResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Relationship { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public string? IdentificationNumber { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}