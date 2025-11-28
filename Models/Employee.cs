using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementAPI.Models
{
    /// <summary>
    /// Modelo de Empleado
    /// </summary>
    public class Employee : BaseEntity
    {
        /// <summary>
        /// Nombre del empleado
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del empleado
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Email corporativo
        /// </summary>
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Número de teléfono
        /// </summary>
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Fecha de contratación
        /// </summary>
        [Required]
        public DateTime HireDate { get; set; }

        /// <summary>
        /// Cargo o puesto
        /// </summary>
        [MaxLength(100)]
        public string? Position { get; set; }

        /// <summary>
        /// Salario mensual
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        /// <summary>
        /// ID del departamento al que pertenece
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// Departamento relacionado
        /// </summary>
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Dependientes del empleado
        /// </summary>
        public virtual ICollection<Dependent> Dependents { get; set; } = new List<Dependent>();

        /// <summary>
        /// Propiedad calculada: Nombre completo
        /// </summary>
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}