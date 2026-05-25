using AutoMapper;
using MaterialManageSystem.Core.Entities;
using MaterialManageSystem.Core.DTOs;

namespace MaterialManageSystem.Api.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User映射
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();

        // Employee映射
        CreateMap<Employee, EmployeeDto>();
        CreateMap<EmployeeDto, Employee>();

        // ReelId映射
        CreateMap<ReelId, ReelIdDto>();
        CreateMap<ReelIdDto, ReelId>();

        // PartNo映射
        CreateMap<PartNo, PartNoDto>();
        CreateMap<PartNoDto, PartNo>();

        // PartNoCategory映射
        CreateMap<PartNoCategory, PartNoCategoryDto>();
        CreateMap<PartNoCategoryDto, PartNoCategory>();

        // Warehouse映射
        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<WarehouseDto, Warehouse>();

        // Rack映射
        CreateMap<Rack, RackDto>();
        CreateMap<RackDto, Rack>();

        // Layer映射
        CreateMap<Layer, LayerDto>();
        CreateMap<LayerDto, Layer>();

        // Cell映射
        CreateMap<Cell, CellDto>();
        CreateMap<CellDto, Cell>();

        // Role映射
        CreateMap<Role, RoleDto>();
        CreateMap<RoleDto, Role>();

        // WarningConfig映射
        CreateMap<WarningConfig, WarningConfigDto>();
        CreateMap<WarningConfigDto, WarningConfig>();

        // WarningRecord映射
        CreateMap<WarningRecord, WarningRecordDto>();
        CreateMap<WarningRecordDto, WarningRecord>();

        // ReelUsageLog映射
        CreateMap<ReelUsageLog, ReelUsageLogDto>();
        CreateMap<ReelUsageLogDto, ReelUsageLog>();
    }
}
