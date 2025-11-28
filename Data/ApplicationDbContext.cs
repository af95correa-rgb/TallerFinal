using Microsoft.EntityFrameworkCore;
using EmployeeManagementAPI.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagementAPI.Data
{
    /// <summary>
    /// Contexto de base de datos principal
    /// Gestiona todas las entidades y configuraciones de EF Core
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets - Tablas en la base de datos
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Dependent> Dependents { get; set; } = null!;

        /// <summary>
        /// Configuración de modelos y relaciones
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasDefaultValue("User");
            });

            // Configuración de Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
            });

            // Configuración de Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();

                // Relación con Department
                entity.HasOne(e => e.Department)
                      .WithMany(d => d.Employees)
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Configuración de Salary
                entity.Property(e => e.Salary)
                      .HasPrecision(18, 2);
            });

            // Configuración de Dependent
            modelBuilder.Entity<Dependent>(entity =>
            {
                // Relación con Employee
                entity.HasOne(d => d.Employee)
                      .WithMany(e => e.Dependents)
                      .HasForeignKey(d => d.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed inicial de datos (opcional)
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Override de SaveChanges para actualizar automáticamente los campos de auditoría
        /// </summary>
        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        /// <summary>
        /// Override de SaveChangesAsync para actualizar automáticamente los campos de auditoría
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Actualiza automáticamente CreatedAt y UpdatedAt
        /// </summary>
        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// Datos iniciales para pruebas
        /// </summary>
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Department
            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    Name = "Recursos Humanos",
                    Code = "HR",
                    Description = "Departamento de gestión de personal",
                    Location = "Edificio A - Piso 2",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Department
                {
                    Id = 2,
                    Name = "Tecnología",
                    Code = "IT",
                    Description = "Departamento de tecnología e innovación",
                    Location = "Edificio B - Piso 3",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Seed User (Admin)
            // Contraseña: Admin123!
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    FullName = "Administrador del Sistema",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }
    }
}