using System;
using System.Linq;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.Enums;

namespace MaterialManageSystem.Infrastructure.Data;

public static class DataSeeder
{
    public static void SeedData(MaterialDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var employee = new Employee
        {
            EmployeeNo = "EMP001",
            Name = "系统管理员",
            Department = "IT",
            Position = "管理员",
            Phone = "13800138000",
            Email = "admin@example.com",
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = "System"
        };
        context.Employees.Add(employee);
        context.SaveChanges();

        var user = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            EmployeeId = employee.Id,
            UserType = UserType.SystemAdmin,
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = "System"
        };
        context.Users.Add(user);
        context.SaveChanges();

        var warehouse = new Warehouse
        {
            Code = "WH001",
            Name = "主仓库",
            Location = "一楼",
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = "System"
        };
        context.Warehouses.Add(warehouse);
        context.SaveChanges();

        var category = new PartNoCategory
        {
            Code = "CAT001",
            Name = "电子元件",
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = "System"
        };
        context.PartNoCategories.Add(category);
        context.SaveChanges();

        var partNo = new PartNo
        {
            PartNoCode = "PN001",
            Name = "电阻 10KΩ",
            Specification = "0603 10KΩ 1%",
            Unit = "EA",
            CategoryId = category.Id,
            TotalQuantity = 0,
            WarningQuantity = 100,
            WarningDays = 30,
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = "System"
        };
        context.PartNos.Add(partNo);
        context.SaveChanges();

        var warningConfig = new WarningConfig
        {
            PartNoId = partNo.Id,
            QuantityThreshold = 100,
            DaysThreshold = 30,
            IsActive = true,
            CreatedAt = DateTime.Now,
            CreatedBy = "System"
        };
        context.WarningConfigs.Add(warningConfig);
        context.SaveChanges();
    }
}
