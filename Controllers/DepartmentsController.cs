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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly ApplicationDbContext _context;

        public DepartmentsController(
            IRepository<Department> departmentRepository,
            ApplicationDbContext context)
        {
            _departmentRepository = departmentRepository;
            _context = context;
        }

        // GET: api/departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var departments = await _context.Departments
                .Include(d => d.Employees)
                .ToListAsync();

            var response = departments.Select(d => new
            {
                d.Id,
                d.Name,
                d.Code,
                d.Description,
                d.Location,
                EmployeeCount = d.Employees.Count,
                d.CreatedAt,
                d.UpdatedAt
            });

            return Ok(response);
        }

        // GET: api/departments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                return NotFound(new { message = $"Departamento con ID {id} no encontrado" });
            }

            var response = new
            {
                department.Id,
                department.Name,
                department.Code,
                department.Description,
                department.Location,
                Employees = department.Employees.Select(e => new
                {
                    e.Id,
                    FullName = e.FullName,
                    e.Email,
                    e.Position,
                    e.Salary
                }),
                EmployeeCount = department.Employees.Count,
                department.CreatedAt,
                department.UpdatedAt
            };

            return Ok(response);
        }

        // GET: api/departments/code/{code}
        [HttpGet("code/{code}")]
        public async Task<ActionResult<object>> GetByCode(string code)
        {
            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Code == code);

            if (department == null)
            {
                return NotFound(new { message = $"Departamento con código {code} no encontrado" });
            }

            var response = new
            {
                department.Id,
                department.Name,
                department.Code,
                department.Description,
                department.Location,
                EmployeeCount = department.Employees.Count
            };

            return Ok(response);
        }

        // POST: api/departments
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> Create([FromBody] CreateDepartmentDTO dto)
        {
            if (await _context.Departments.AnyAsync(d => d.Code == dto.Code))
            {
                return BadRequest(new { message = $"Ya existe un departamento con el código {dto.Code}" });
            }

            if (await _context.Departments.AnyAsync(d => d.Name == dto.Name))
            {
                return BadRequest(new { message = $"Ya existe un departamento con el nombre {dto.Name}" });
            }

            var department = new Department
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                Location = dto.Location
            };

            var created = await _departmentRepository.AddAsync(department);

            var response = new
            {
                created.Id,
                created.Name,
                created.Code,
                created.Description,
                created.Location,
                created.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        // PUT: api/departments/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> Update(int id, [FromBody] UpdateDepartmentDTO dto)
        {
            var department = await _departmentRepository.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound(new { message = $"Departamento con ID {id} no encontrado" });
            }

            if (!string.IsNullOrEmpty(dto.Code) && dto.Code != department.Code)
            {
                if (await _context.Departments.AnyAsync(d => d.Code == dto.Code && d.Id != id))
                {
                    return BadRequest(new { message = $"Ya existe un departamento con el código {dto.Code}" });
                }
            }

            if (!string.IsNullOrEmpty(dto.Name) && dto.Name != department.Name)
            {
                if (await _context.Departments.AnyAsync(d => d.Name == dto.Name && d.Id != id))
                {
                    return BadRequest(new { message = $"Ya existe un departamento con el nombre {dto.Name}" });
                }
            }

            if (!string.IsNullOrEmpty(dto.Name))
                department.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Code))
                department.Code = dto.Code;

            if (!string.IsNullOrEmpty(dto.Description))
                department.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.Location))
                department.Location = dto.Location;

            var updated = await _departmentRepository.UpdateAsync(department);

            var response = new
            {
                updated.Id,
                updated.Name,
                updated.Code,
                updated.Description,
                updated.Location,
                updated.UpdatedAt
            };

            return Ok(response);
        }

        // DELETE: api/departments/{id}  (HARD DELETE)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                return NotFound(new { message = $"Departamento con ID {id} no encontrado" });
            }

            // Eliminar empleados asociados (si quieres)
            // foreach (var employee in department.Employees) _context.Employees.Remove(employee);

            await _departmentRepository.DeleteAsync(department);

            return Ok(new { message = "Departamento eliminado definitivamente" });
        }

        // DELETE: api/departments/{id}/permanent  (redundante pero mantenido)
        [HttpDelete("{id}/permanent")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePermanent(int id)
        {
            var success = await _departmentRepository.DeleteAsync(id);

            if (!success)
            {
                return NotFound(new { message = $"Departamento con ID {id} no encontrado" });
            }

            return Ok(new { message = "Departamento eliminado permanentemente" });
        }

        // GET: api/departments/search?query=
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Debe proporcionar un término de búsqueda" });
            }

            var departments = await _context.Departments
                .Include(d => d.Employees)
                .Where(d =>
                    d.Name.Contains(query) ||
                    d.Code.Contains(query) ||
                    (d.Description != null && d.Description.Contains(query)))
                .ToListAsync();

            var response = departments.Select(d => new
            {
                d.Id,
                d.Name,
                d.Code,
                d.Location,
                EmployeeCount = d.Employees.Count
            });

            return Ok(response);
        }

        // GET: api/departments/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStats()
        {
            var totalDepartments = await _context.Departments.CountAsync();
            var totalEmployees = await _context.Employees.CountAsync();

            var departmentStats = await _context.Departments
                .Select(d => new
                {
                    d.Name,
                    d.Code,
                    EmployeeCount = d.Employees.Count,
                    AverageSalary = d.Employees.Average(e => (decimal?)e.Salary) ?? 0
                })
                .OrderByDescending(d => d.EmployeeCount)
                .ToListAsync();

            return Ok(new
            {
                TotalDepartments = totalDepartments,
                TotalEmployees = totalEmployees,
                AverageEmployeesPerDepartment =
                    totalDepartments > 0 ? Math.Round((double)totalEmployees / totalDepartments, 2) : 0,
                DepartmentWithMostEmployees = departmentStats.FirstOrDefault(),
                DepartmentWithLeastEmployees = departmentStats.LastOrDefault(),
                AllDepartments = departmentStats
            });
        }

        // POST: api/departments/{from}/transfer/{to}
        [HttpPost("{fromDepartmentId}/transfer/{toDepartmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> TransferEmployees(
            int fromDepartmentId,
            int toDepartmentId,
            [FromBody] List<int> employeeIds)
        {
            var fromDepartment = await _departmentRepository.GetByIdAsync(fromDepartmentId);
            var toDepartment = await _departmentRepository.GetByIdAsync(toDepartmentId);

            if (fromDepartment == null)
                return NotFound(new { message = $"Departamento origen con ID {fromDepartmentId} no encontrado" });

            if (toDepartment == null)
                return NotFound(new { message = $"Departamento destino con ID {toDepartmentId} no encontrado" });

            var employees = await _context.Employees
                .Where(e => employeeIds.Contains(e.Id) && e.DepartmentId == fromDepartmentId)
                .ToListAsync();

            if (employees.Count != employeeIds.Count)
            {
                return BadRequest(new { message = "Algunos empleados no fueron encontrados o no pertenecen al departamento origen" });
            }

            foreach (var employee in employees)
            {
                employee.DepartmentId = toDepartmentId;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"{employees.Count} empleados transferidos exitosamente",
                from = fromDepartment.Name,
                to = toDepartment.Name,
                transferredEmployees = employees.Select(e => new
                {
                    e.Id,
                    FullName = e.FullName
                })
            });
        }
    }

    public class CreateDepartmentDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
    }

    public class UpdateDepartmentDTO
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
    }
}
