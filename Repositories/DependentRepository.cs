using Microsoft.EntityFrameworkCore;
using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.Interfaces;
using EmployeeManagementAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementAPI.Repositories
{
    /// <summary>
    /// Interfaz específica para Dependientes
    /// </summary>
    public interface IDependentRepository : IRepository<Dependent>
    {
        Task<IEnumerable<Dependent>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<Dependent>> GetActiveByEmployeeIdAsync(int employeeId);
        Task<int> CountByEmployeeIdAsync(int employeeId);
    }

    /// <summary>
    /// Repositorio de Dependientes con operaciones específicas
    /// </summary>
    public class DependentRepository : Repository<Dependent>, IDependentRepository
    {
        public DependentRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Override para incluir información del empleado
        /// </summary>
        public override async Task<IEnumerable<Dependent>> GetAllAsync()
        {
            return await _dbSet
                .Include(d => d.Employee)
                .ToListAsync();
        }

        /// <summary>
        /// Override para incluir información del empleado
        /// </summary>
        public override async Task<Dependent?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Employee)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        /// <summary>
        /// Obtiene todos los dependientes de un empleado
        /// </summary>
        public async Task<IEnumerable<Dependent>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .Include(d => d.Employee)
                .Where(d => d.EmployeeId == employeeId)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene dependientes activos de un empleado
        /// </summary>
        public async Task<IEnumerable<Dependent>> GetActiveByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .Include(d => d.Employee)
                .Where(d => d.EmployeeId == employeeId && d.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Cuenta cuántos dependientes tiene un empleado
        /// </summary>
        public async Task<int> CountByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .CountAsync(d => d.EmployeeId == employeeId && d.IsActive);
        }
    }
}