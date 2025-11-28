using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.Interfaces;
using EmployeeManagementAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementAPI.Controllers
{
    /// <summary>
    /// Controlador para gestión de Empleados
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly ApplicationDbContext _context;

        public EmployeesController(
            IRepository<Employee> employeeRepository,
            IRepository<Department> departmentRepository,
            ApplicationDbContext context)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _context = context;
        }

        /// <summary>
        /// Obtener todos los empleados
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Dependents)
                .Where(e => e.IsActive)
                .ToListAsync();

            var response = employees.Select(e => new
            {
                e.Id,
                e.FirstName,
                e.LastName,
                FullName = e.FullName,
                e.Email,
                e.PhoneNumber,
                e.HireDate,
                e.Position,
                e.Salary,
                e.DepartmentId,
                Department = e.Department != null ? new
                {
                    e.Department.Id,
                    e.Department.Name,
                    e.Department.Code
                } : null,
                DependentsCount = e.Dependents.Count(d => d.IsActive),
                e.CreatedAt,
                e.UpdatedAt
            });

            return Ok(response);
        }

        /// <summary>
        /// Obtener un empleado por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Dependents)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound(new { message = $"Empleado con ID {id} no encontrado" });
            }

            var response = new
            {
                employee.Id,
                employee.FirstName,
                employee.LastName,
                FullName = employee.FullName,
                employee.Email,
                employee.PhoneNumber,
                employee.HireDate,
                employee.Position,
                employee.Salary,
                employee.DepartmentId,
                Department = employee.Department != null ? new
                {
                    employee.Department.Id,
                    employee.Department.Name,
                    employee.Department.Code,
                    employee.Department.Location
                } : null,
                Dependents = employee.Dependents.Where(d => d.IsActive).Select(d => new
                {
                    d.Id,
                    d.FirstName,
                    d.LastName,
                    FullName = d.FullName,
                    d.Relationship,
                    Age = d.Age
                }),
                employee.CreatedAt,
                employee.UpdatedAt,
                employee.IsActive
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtener empleados por departamento
        /// </summary>
        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetByDepartment(int departmentId)
        {
            var department = await _departmentRepository.GetByIdAsync(departmentId);
            if (department == null)
            {
                return NotFound(new { message = $"Departamento con ID {departmentId} no encontrado" });
            }

            var employees = await _context.Employees
                .Include(e => e.Dependents)
                .Where(e => e.DepartmentId == departmentId && e.IsActive)
                .ToListAsync();

            var response = employees.Select(e => new
            {
                e.Id,
                FullName = e.FullName,
                e.Email,
                e.Position,
                e.Salary,
                DependentsCount = e.Dependents.Count(d => d.IsActive)
            });

            return Ok(response);
        }

        /// <summary>
        /// Crear un nuevo empleado
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] CreateEmployeeDTO dto)
        {
            // Validar que el email no exista
            if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
            {
                return BadRequest(new { message = "El email ya está registrado" });
            }

            // Validar departamento si se proporciona
            if (dto.DepartmentId.HasValue)
            {
                var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId.Value);
                if (department == null)
                {
                    return BadRequest(new { message = $"Departamento con ID {dto.DepartmentId} no encontrado" });
                }
            }

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                HireDate = dto.HireDate,
                Position = dto.Position,
                Salary = dto.Salary,
                DepartmentId = dto.DepartmentId,
                IsActive = true
            };

            var created = await _employeeRepository.AddAsync(employee);

            // Recargar con relaciones
            var result = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == created.Id);

            var response = new
            {
                result!.Id,
                result.FirstName,
                result.LastName,
                FullName = result.FullName,
                result.Email,
                result.PhoneNumber,
                result.HireDate,
                result.Position,
                result.Salary,
                result.DepartmentId,
                Department = result.Department?.Name,
                result.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Actualizar un empleado
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> Update(int id, [FromBody] UpdateEmployeeDTO dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound(new { message = $"Empleado con ID {id} no encontrado" });
            }

            // Validar email único si se está cambiando
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != employee.Email)
            {
                if (await _context.Employees.AnyAsync(e => e.Email == dto.Email && e.Id != id))
                {
                    return BadRequest(new { message = "El email ya está registrado" });
                }
            }

            // Validar departamento si se proporciona
            if (dto.DepartmentId.HasValue)
            {
                var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId.Value);
                if (department == null)
                {
                    return BadRequest(new { message = $"Departamento con ID {dto.DepartmentId} no encontrado" });
                }
            }

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrEmpty(dto.FirstName))
                employee.FirstName = dto.FirstName;

            if (!string.IsNullOrEmpty(dto.LastName))
                employee.LastName = dto.LastName;

            if (!string.IsNullOrEmpty(dto.Email))
                employee.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                employee.PhoneNumber = dto.PhoneNumber;

            if (dto.HireDate.HasValue)
                employee.HireDate = dto.HireDate.Value;

            if (!string.IsNullOrEmpty(dto.Position))
                employee.Position = dto.Position;

            if (dto.Salary.HasValue)
                employee.Salary = dto.Salary.Value;

            if (dto.DepartmentId.HasValue)
                employee.DepartmentId = dto.DepartmentId.Value;

            var updated = await _employeeRepository.UpdateAsync(employee);

            // Recargar con relaciones
            var result = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == updated.Id);

            var response = new
            {
                result!.Id,
                result.FirstName,
                result.LastName,
                FullName = result.FullName,
                result.Email,
                result.Position,
                result.Salary,
                Department = result.Department?.Name,
                result.UpdatedAt
            };

            return Ok(response);
        }

        /// <summary>
        /// Eliminar un empleado (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound(new { message = $"Empleado con ID {id} no encontrado" });
            }

            // Soft delete
            employee.IsActive = false;
            await _employeeRepository.UpdateAsync(employee);

            return Ok(new { message = "Empleado eliminado exitosamente" });
        }

        /// <summary>
        /// Eliminar permanentemente un empleado
        /// </summary>
        [HttpDelete("{id}/permanent")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePermanent(int id)
        {
            var success = await _employeeRepository.DeleteAsync(id);

            if (!success)
            {
                return NotFound(new { message = $"Empleado con ID {id} no encontrado" });
            }

            return Ok(new { message = "Empleado eliminado permanentemente" });
        }

        /// <summary>
        /// Buscar empleados por nombre
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Debe proporcionar un término de búsqueda" });
            }

            var employees = await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.IsActive &&
                    (e.FirstName.Contains(query) ||
                     e.LastName.Contains(query) ||
                     e.Email.Contains(query)))
                .ToListAsync();

            var response = employees.Select(e => new
            {
                e.Id,
                FullName = e.FullName,
                e.Email,
                e.Position,
                Department = e.Department?.Name
            });

            return Ok(response);
        }

        /// <summary>
        /// Obtener estadísticas de empleados
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStats()
        {
            var totalEmployees = await _context.Employees.CountAsync(e => e.IsActive);
            var totalDependents = await _context.Dependents.CountAsync(d => d.IsActive);
            var averageSalary = await _context.Employees
                .Where(e => e.IsActive)
                .AverageAsync(e => (decimal?)e.Salary) ?? 0;

            var employeesByDepartment = await _context.Employees
                .Where(e => e.IsActive && e.DepartmentId != null)
                .GroupBy(e => e.Department!.Name)
                .Select(g => new
                {
                    Department = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(new
            {
                TotalEmployees = totalEmployees,
                TotalDependents = totalDependents,
                AverageSalary = Math.Round(averageSalary, 2),
                EmployeesByDepartment = employeesByDepartment
            });
        }
    }

    // DTOs para Employee
    public class CreateEmployeeDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public string? Position { get; set; }
        public decimal Salary { get; set; }
        public int? DepartmentId { get; set; }
    }

    public class UpdateEmployeeDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Position { get; set; }
        public decimal? Salary { get; set; }
        public int? DepartmentId { get; set; }
    }
}