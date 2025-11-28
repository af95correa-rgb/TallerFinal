using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementAPI.Models
{
    /// <summary>
    /// Modelo de Dependiente
    /// Representa familiares o personas a cargo del empleado
    /// </summary>
    public class Dependent : BaseEntity
    {
        /// <summary>
        /// Nombre del dependiente
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del dependiente
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de nacimiento
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Relación con el empleado (hijo, cónyuge, etc.)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Relationship { get; set; } = string.Empty;

        /// <summary>
        /// Género
        /// </summary>
        [MaxLength(20)]
        public string? Gender { get; set; }

        /// <summary>
        /// Número de identificación o seguro
        /// </summary>
        [MaxLength(50)]
        public string? IdentificationNumber { get; set; }

        /// <summary>
        /// ID del empleado responsable
        /// </summary>
        [Required]
        public int EmployeeId { get; set; }

        /// <summary>
        /// Empleado relacionado
        /// </summary>
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// Propiedad calculada: Nombre completo
        /// </summary>
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Propiedad calculada: Edad
        /// </summary>
        [NotMapped]
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}