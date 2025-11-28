using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementAPI.DTOs;
using EmployeeManagementAPI.Models;
using EmployeeManagementAPI.Repositories;
using EmployeeManagementAPI.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere autenticación para todos los endpoints
    public class DependentsController : ControllerBase
    {
        private readonly IDependentRepository _dependentRepository;
        private readonly IRepository<Employee> _employeeRepository;

        public DependentsController(
            IDependentRepository dependentRepository,
            IRepository<Employee> employeeRepository)
        {
            _dependentRepository = dependentRepository;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Obtener todos los dependientes
        /// </summary>
        /// <returns>Lista de dependientes</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DependentResponseDTO>>> GetAll()
        {
            var dependents = await _dependentRepository.GetAllAsync();

            var response = dependents.Select(d => new DependentResponseDTO
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                FullName = d.FullName,
                DateOfBirth = d.DateOfBirth,
                Age = d.Age,
                Relationship = d.Relationship,
                Gender = d.Gender,
                IdentificationNumber = d.IdentificationNumber,
                EmployeeId = d.EmployeeId,
                EmployeeName = d.Employee?.FullName ?? "N/A",
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            });

            return Ok(response);
        }

        /// <summary>
        /// Obtener un dependiente por ID
        /// </summary>
        /// <param name="id">ID del dependiente</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<DependentResponseDTO>> GetById(int id)
        {
            var dependent = await _dependentRepository.GetByIdAsync(id);

            if (dependent == null)
            {
                return NotFound(new { message = $"Dependiente con ID {id} no encontrado" });
            }

            var response = new DependentResponseDTO
            {
                Id = dependent.Id,
                FirstName = dependent.FirstName,
                LastName = dependent.LastName,
                FullName = dependent.FullName,
                DateOfBirth = dependent.DateOfBirth,
                Age = dependent.Age,
                Relationship = dependent.Relationship,
                Gender = dependent.Gender,
                IdentificationNumber = dependent.IdentificationNumber,
                EmployeeId = dependent.EmployeeId,
                EmployeeName = dependent.Employee?.FullName ?? "N/A",
                CreatedAt = dependent.CreatedAt,
                UpdatedAt = dependent.UpdatedAt
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtener dependientes por empleado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<DependentResponseDTO>>> GetByEmployee(int employeeId)
        {
            // Verificar que el empleado exista
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound(new { message = $"Empleado con ID {employeeId} no encontrado" });
            }

            var dependents = await _dependentRepository.GetByEmployeeIdAsync(employeeId);

            var response = dependents.Select(d => new DependentResponseDTO
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                FullName = d.FullName,
                DateOfBirth = d.DateOfBirth,
                Age = d.Age,
                Relationship = d.Relationship,
                Gender = d.Gender,
                IdentificationNumber = d.IdentificationNumber,
                EmployeeId = d.EmployeeId,
                EmployeeName = d.Employee?.FullName ?? "N/A",
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            });

            return Ok(response);
        }

        /// <summary>
        /// Crear un nuevo dependiente
        /// </summary>
        /// <param name="createDto">Datos del dependiente</param>
        [HttpPost]
        public async Task<ActionResult<DependentResponseDTO>> Create([FromBody] CreateDependentDTO createDto)
        {
            // Verificar que el empleado exista
            var employee = await _employeeRepository.GetByIdAsync(createDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest(new { message = $"Empleado con ID {createDto.EmployeeId} no encontrado" });
            }

            // Crear dependiente
            var dependent = new Dependent
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                DateOfBirth = createDto.DateOfBirth,
                Relationship = createDto.Relationship,
                Gender = createDto.Gender,
                IdentificationNumber = createDto.IdentificationNumber,
                EmployeeId = createDto.EmployeeId,
                IsActive = true
            };

            var created = await _dependentRepository.AddAsync(dependent);

            // Recargar con datos del empleado
            var result = await _dependentRepository.GetByIdAsync(created.Id);

            var response = new DependentResponseDTO
            {
                Id = result!.Id,
                FirstName = result.FirstName,
                LastName = result.LastName,
                FullName = result.FullName,
                DateOfBirth = result.DateOfBirth,
                Age = result.Age,
                Relationship = result.Relationship,
                Gender = result.Gender,
                IdentificationNumber = result.IdentificationNumber,
                EmployeeId = result.EmployeeId,
                EmployeeName = result.Employee?.FullName ?? "N/A",
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Actualizar un dependiente
        /// </summary>
        /// <param name="id">ID del dependiente</param>
        /// <param name="updateDto">Datos a actualizar</param>
        [HttpPut("{id}")]
        public async Task<ActionResult<DependentResponseDTO>> Update(int id, [FromBody] UpdateDependentDTO updateDto)
        {
            var dependent = await _dependentRepository.GetByIdAsync(id);

            if (dependent == null)
            {
                return NotFound(new { message = $"Dependiente con ID {id} no encontrado" });
            }

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrEmpty(updateDto.FirstName))
                dependent.FirstName = updateDto.FirstName;

            if (!string.IsNullOrEmpty(updateDto.LastName))
                dependent.LastName = updateDto.LastName;

            if (updateDto.DateOfBirth.HasValue)
                dependent.DateOfBirth = updateDto.DateOfBirth.Value;

            if (!string.IsNullOrEmpty(updateDto.Relationship))
                dependent.Relationship = updateDto.Relationship;

            if (!string.IsNullOrEmpty(updateDto.Gender))
                dependent.Gender = updateDto.Gender;

            if (!string.IsNullOrEmpty(updateDto.IdentificationNumber))
                dependent.IdentificationNumber = updateDto.IdentificationNumber;

            var updated = await _dependentRepository.UpdateAsync(dependent);

            var response = new DependentResponseDTO
            {
                Id = updated.Id,
                FirstName = updated.FirstName,
                LastName = updated.LastName,
                FullName = updated.FullName,
                DateOfBirth = updated.DateOfBirth,
                Age = updated.Age,
                Relationship = updated.Relationship,
                Gender = updated.Gender,
                IdentificationNumber = updated.IdentificationNumber,
                EmployeeId = updated.EmployeeId,
                EmployeeName = updated.Employee?.FullName ?? "N/A",
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt
            };

            return Ok(response);
        }

        /// <summary>
        /// Eliminar un dependiente (soft delete)
        /// </summary>
        /// <param name="id">ID del dependiente</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dependent = await _dependentRepository.GetByIdAsync(id);

            if (dependent == null)
            {
                return NotFound(new { message = $"Dependiente con ID {id} no encontrado" });
            }

            // Soft delete: marcar como inactivo
            dependent.IsActive = false;
            await _dependentRepository.UpdateAsync(dependent);

            return Ok(new { message = "Dependiente eliminado exitosamente" });
        }

        /// <summary>
        /// Eliminar permanentemente un dependiente
        /// </summary>
        /// <param name="id">ID del dependiente</param>
        [HttpDelete("{id}/permanent")]
        [Authorize(Roles = "Admin")] // Solo admins pueden eliminar permanentemente
        public async Task<IActionResult> DeletePermanent(int id)
        {
            var success = await _dependentRepository.DeleteAsync(id);

            if (!success)
            {
                return NotFound(new { message = $"Dependiente con ID {id} no encontrado" });
            }

            return Ok(new { message = "Dependiente eliminado permanentemente" });
        }

        /// <summary>
        /// Obtener estadísticas de dependientes por empleado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        [HttpGet("employee/{employeeId}/count")]
        public async Task<ActionResult> GetCountByEmployee(int employeeId)
        {
            var count = await _dependentRepository.CountByEmployeeIdAsync(employeeId);

            return Ok(new { employeeId, totalDependents = count });
        }
    }
}