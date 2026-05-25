using System;
using Microsoft.EntityFrameworkCore;
using MaterialManageSystem.Core.Entities;

namespace MaterialManageSystem.Infrastructure.Data;

public class MaterialDbContext : DbContext
{
    public MaterialDbContext(DbContextOptions<MaterialDbContext> options) : base(options)
    {
    }

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Rack> Racks => Set<Rack>();
    public DbSet<Layer> Layers => Set<Layer>();
    public DbSet<Cell> Cells => Set<Cell>();
    public DbSet<PartNoCategory> PartNoCategories => Set<PartNoCategory>();
    public DbSet<PartNo> PartNos => Set<PartNo>();
    public DbSet<ReelId> ReelIds => Set<ReelId>();
    public DbSet<ReelUsageLog> ReelUsageLogs => Set<ReelUsageLog>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<OperationLog> OperationLogs => Set<OperationLog>();
    public DbSet<WarningConfig> WarningConfigs => Set<WarningConfig>();
    public DbSet<WarningRecord> WarningRecords => Set<WarningRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Warehouse
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Warehouses");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.Remark).HasMaxLength(1000);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        // Rack
        modelBuilder.Entity<Rack>(entity =>
        {
            entity.ToTable("Racks");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(e => e.Warehouse)
                .WithMany(w => w.Racks)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Layer
        modelBuilder.Entity<Layer>(entity =>
        {
            entity.ToTable("Layers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.Height).HasPrecision(10, 2);
            entity.Property(e => e.WeightLimit).HasPrecision(10, 2);

            entity.HasOne(e => e.Rack)
                .WithMany(r => r.Layers)
                .HasForeignKey(e => e.RackId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Cell
        modelBuilder.Entity<Cell>(entity =>
        {
            entity.ToTable("Cells");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(e => e.Layer)
                .WithMany(l => l.Cells)
                .HasForeignKey(e => e.LayerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PartNo)
                .WithMany()
                .HasForeignKey(e => e.PartNoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PartNoCategory
        modelBuilder.Entity<PartNoCategory>(entity =>
        {
            entity.ToTable("PartNoCategories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PartNo
        modelBuilder.Entity<PartNo>(entity =>
        {
            entity.ToTable("PartNos");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PartNoCode).IsUnique();
            entity.Property(e => e.PartNoCode).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Specification).HasMaxLength(500);
            entity.Property(e => e.Size).HasMaxLength(200);
            entity.Property(e => e.Packaging).HasMaxLength(200);
            entity.Property(e => e.Unit).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TotalQuantity).HasPrecision(18, 6);
            entity.Property(e => e.WarningQuantity).HasPrecision(18, 6);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.PartNos)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ReelId
        modelBuilder.Entity<ReelId>(entity =>
        {
            entity.ToTable("ReelIds");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ReelNo).IsUnique();
            entity.Property(e => e.ReelNo).HasMaxLength(100).IsRequired();
            entity.Property(e => e.InitialQuantity).HasPrecision(18, 6).IsRequired();
            entity.Property(e => e.CurrentQuantity).HasPrecision(18, 6).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(e => e.PartNo)
                .WithMany(p => p.Reels)
                .HasForeignKey(e => e.PartNoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Cell)
                .WithMany(c => c.Reels)
                .HasForeignKey(e => e.CellId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ReelUsageLog
        modelBuilder.Entity<ReelUsageLog>(entity =>
        {
            entity.ToTable("ReelUsageLogs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 6).IsRequired();
            entity.Property(e => e.RemainingQuantity).HasPrecision(18, 6).IsRequired();
            entity.Property(e => e.Remark).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.Reel)
                .WithMany(r => r.UsageLogs)
                .HasForeignKey(e => e.ReelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Employee)
                .WithMany(emp => emp.UsageLogs)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Employee
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EmployeeNo).IsUnique();
            entity.Property(e => e.EmployeeNo).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<User>(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.LastLoginIp).HasMaxLength(50);

            entity.HasOne(e => e.Employee)
                .WithOne(emp => emp.User)
                .HasForeignKey<User>(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
        });

        // Permission
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Module).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // RolePermission
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // UserRole
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // OperationLog
        modelBuilder.Entity<OperationLog>(entity =>
        {
            entity.ToTable("OperationLogs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OperationType).HasMaxLength(50);
            entity.Property(e => e.ControllerName).HasMaxLength(100);
            entity.Property(e => e.ActionName).HasMaxLength(100);
            entity.Property(e => e.RequestMethod).HasMaxLength(10);
            entity.Property(e => e.RequestPath).HasMaxLength(500);
            entity.Property(e => e.RequestBody).HasMaxLength(2000);
            entity.Property(e => e.ResponseBody).HasMaxLength(2000);
            entity.Property(e => e.UserId).HasMaxLength(100);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
        });

        // WarningConfig
        modelBuilder.Entity<WarningConfig>(entity =>
        {
            entity.ToTable("WarningConfigs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuantityThreshold).HasPrecision(18, 6);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(e => e.PartNo)
                .WithMany()
                .HasForeignKey(e => e.PartNoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // WarningRecord
        modelBuilder.Entity<WarningRecord>(entity =>
        {
            entity.ToTable("WarningRecords");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CurrentQuantity).HasPrecision(18, 6).IsRequired();
            entity.Property(e => e.Remark).HasMaxLength(500);
            entity.Property(e => e.ResolvedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100).IsRequired();

            entity.HasOne(e => e.Reel)
                .WithMany()
                .HasForeignKey(e => e.ReelId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
