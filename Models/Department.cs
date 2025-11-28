using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    /// <summary>
    /// Modelo de Departamento
    /// </summary>
    public class Department : BaseEntity
    {
        /// <summary>
        /// Nombre del departamento
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Código único del departamento
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del departamento
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Ubicación física del departamento
        /// </summary>
        [MaxLength(200)]
        public string? Location { get; set; }

        /// <summary>
        /// Empleados que pertenecen a este departamento
        /// </summary>
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}