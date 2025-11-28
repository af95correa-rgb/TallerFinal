using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    /// <summary>
    /// Entidad base con campos de auditoría
    /// Todas las entidades heredan de esta clase
    /// </summary>
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Usuario que creó el registro
        /// </summary>
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Usuario que actualizó el registro
        /// </summary>
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Indica si el registro está activo
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}