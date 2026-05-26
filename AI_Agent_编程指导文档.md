# 物料管理系统 - AI Agent 编程指导文档

> Material Management System - AI Agent Programming Guide

## 文档版本
- 版本号: 1.1.0
- 创建日期: 2026-05-21
- 最后更新: 2026-05-22

## 更新历史
| 版本号 | 更新日期 | 说明 |
|--------|----------|------|
| 1.0.0 | 2026-05-21 | 初始版本 - 完整项目设计与实现 |
| 1.1.0 | 2026-05-22 | 安全修复与功能增强 - 密码哈希、事务处理、预警服务、异常处理、输入验证 |

---

## 目录
1. [项目概述与目标](#1-项目概述与目标)
2. [系统架构设计](#2-系统架构设计)
3. [数据库设计](#3-数据库设计)
4. [API接口规范](#4-api接口规范)
5. [实时通信设计(SignalR)](#5-实时通信设计signalr)
6. [前端架构设计](#6-前端架构设计)
7. [安全与权限管理](#7-安全与权限管理)
8. [分步实现指南](#8-分步实现指南)

---

## 1. 项目概述与目标

### 1.1 项目背景
物料管理系统是一个用于制造型企业管理物料、料卷(Reel)、库房的综合管理系统。系统支持PC网页端和手机端网页两种访问方式。

### 1.2 技术栈
| 层级 | 技术选型 | 说明 |
|------|----------|------|
| 后端框架 | ASP.NET Core 8.0 | Web API |
| 数据库 | SQL Server 2019+ | 主数据库 |
| ORM | Entity Framework Core 8.0 | 数据库访问，支持多数据库 |
| 实时通信 | SignalR | 预警推送 |
| PC前端 | Bootstrap 5 + Razor Pages | 后台管理 |
| 移动端前端 | Vue 3 + Vite + TypeScript | 手机端网页 |
| 状态管理 | Pinia (Vue) | 前端状态管理 |
| HTTP客户端 | Axios | API调用 |

### 1.3 项目模块划分
```
MaterialManageSystem/
├── MaterialManageSystem.sln                    # 解决方案文件
├── MaterialManageSystem.Api/               # API项目
│   ├── Controllers/                        # API控制器
│   ├── Hubs/                               # SignalR Hub
│   ├── Services/                           # 业务服务
│   └── Program.cs
├── MaterialManageSystem.Core/              # 核心业务逻辑
│   ├── Entities/                           # 实体模型
│   ├── Interfaces/                         # 接口定义
│   └── Services/
├── MaterialManageSystem.Infrastructure/    # 基础设施
│   ├── Data/                               # DbContext
│   └── Repositories/                       # 数据仓储
├── MaterialManageSystem.Admin/             # Bootstrap后台管理(ASP.NET Core Razor)
│   └── Pages/
└── MaterialManageSystem.Vue/               # Vue3手机端前端
    ├── src/
    └── package.json
```

---

## 2. 系统架构设计

### 2.1 整体架构
```
┌─────────────────────────────────────────────────────────────────┐
│                        客户端层                                  │
├─────────────────────┬───────────────────────────────────────────┤
│   Bootstrap Admin   │              Vue3 Mobile                  │
│   (PC网页管理端)    │              (手机端网页)                   │
└─────────┬───────────┴─────────────────────┬─────────────────────┘
          │                                 │
          └────────────┬────────────────────┘
                       │ HTTP/REST + SignalR
          ┌────────────▼────────────────────┐
          │         API Gateway             │
          │      (ASP.NET Core Web API)     │
          │    + SignalR Hub (预警推送)     │
          └────────────┬────────────────────┘
                       │
     ┌─────────────────┼─────────────────┐
     │                 │                 │
┌────▼────┐      ┌─────▼─────┐     ┌─────▼─────┐
│  Core   │      │Infrastructure│   │   Tests   │
│ (业务逻辑)│      │ (数据访问)   │     │  (单元测试) │
└─────────┘      └───────────┘     └───────────┘
                       │
              ┌────────▼────────┐
              │   SQL Server    │
              │   (数据库)       │
              └─────────────────┘
```

### 2.2 核心业务流程
1. **物料入库**: 创建PartNo → 创建ReelId → 分配存储位 → 入库
2. **物料出库**: 查找ReelId → 验证存储位 → 更新状态为"出库"
3. **物料上线**: 出库ReelId → 状态变为"在线" → 记录使用数据
4. **预警触发**: 定时检查使用数据 → 计算剩余量/剩余时间 → 推送预警

---

## 3. 数据库设计

### 3.1 ER图概览
```
Warehouse(库房) 1──n Rack(货架) 1──n Layer(层) 1──n Cell(格) 1──n ReelId(料卷)
                                                                            │
PartNo(物料) 1──n ─────────────────────────────────────────────────────────┘
                                                                            │
ReelUsageLog(使用记录) ◄── UsageUpload(使用上传) ◄── Employee(员工) ◄── User(用户)──► Role(角色)──► Permission(权限)
                                                                                 │
                                                                         OperationLog(操作日志)
```

### 3.2 数据表详细设计

#### 3.2.1 库房相关表

**Warehouses (库房表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| Code | nvarchar(50) | Unique, Not Null | 库房编码 |
| Name | nvarchar(200) | Not Null | 库房名称 |
| Location | nvarchar(500) | Null | 库房位置 |
| Remark | nvarchar(1000) | Null | 备注 |
| IsActive | bit | Default=1 | 是否启用 |
| CreatedAt | datetime2 | Not Null | 创建时间 |
| CreatedBy | nvarchar(100) | Not Null | 创建人 |
| UpdatedAt | datetime2 | Null | 更新时间 |
| UpdatedBy | nvarchar(100) | Null | 更新人 |

**Racks (货架表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| WarehouseId | bigint | FK, Not Null | 所属库房 |
| Code | nvarchar(50) | Unique, Not Null | 货架编码 |
| Name | nvarchar(200) | Not Null | 货架名称 |
| RowCount | int | Not Null, Default=1 | 行数 |
| ColumnCount | int | Not Null, Default=1 | 列数 |
| IsActive | bit | Default=1 | 是否启用 |
| CreatedAt | datetime2 | Not Null | 创建时间 |
| CreatedBy | nvarchar(100) | Not Null | 创建人 |
| UpdatedAt | datetime2 | Null | 更新时间 |
| UpdatedBy | nvarchar(100) | Null | 更新人 |

**Layers (层表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| RackId | bigint | FK, Not Null | 所属货架 |
| Level | int | Not Null | 层号(从1开始) |
| Height | decimal(10,2) | Null | 高度(cm) |
| WeightLimit | decimal(10,2) | Null | 承重限制(kg) |
| IsActive | bit | Default=1 | 是否启用 |
| CreatedAt | datetime2 | Not Null | 创建时间 |
| CreatedBy | nvarchar(100) | Not Null | 创建人 |
| UpdatedAt | datetime2 | Null | 更新时间 |
| UpdatedBy | nvarchar(100) | Null | 更新人 |

**Cells (存储格表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| LayerId | bigint | FK, Not Null | 所属层 |
| Row | int | Not Null | 所在行 |
| Col | int | Not Null | 所在列 |
| StorageMode | int | Not Null, Default=0 | 存储模式: 0=同料号多Reel, 1=单Reel独占, 2=不限制 |
| PartNoId | bigint | FK, Null | 限定的物料ID(仅模式0/1时使用) |
| IsActive | bit | Default=1 | 是否启用 |
| CreatedAt | datetime2 | Not Null | 创建时间 |
| CreatedBy | nvarchar(100) | Not Null | 创建人 |
| UpdatedAt | datetime2 | Null | 更新时间 |
| UpdatedBy | nvarchar(100) | Null | 更新人 |

#### 3.2.2 物料相关表

**PartNos (物料表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| PartNo | nvarchar(100) | Unique, Not Null | 物料编号 |
| Name | nvarchar(200) | Not Null | 物料名称 |
| Specification | nvarchar(500) | Null | 规格 |
| Size | nvarchar(200) | Null | 大小尺寸 |
| Packaging | nvarchar(200) | Null | 包装方式 |
| Unit | nvarchar(50) | Not Null, Default='EA' | 单位 |
| CategoryId | bigint | FK, Null | 物料分类 |
| TotalQuantity | decimal(18,6) | Default=0 | 库存总量 |
| WarningQuantity | decimal(18,6) | Null | 预警数量阈值 |
| WarningDays | int | Null | 预警天数阈值 |
| IsActive | bit | Default=1 | 是否启用 |
| CreatedAt | datetime2 | Not Null | 创建时间 |
| CreatedBy | nvarchar(100) | Not Null | 创建人 |
| UpdatedAt | datetime2 | Null | 更新时间 |
| UpdatedBy | nvarchar(100) | Null | 更新人 |

**PartNoCategories (物料分类表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| ParentId | bigint | FK, Null | 父分类 |
| Code | nvarchar(50) | Unique, Not Null | 分类编码 |
| Name | nvarchar(200) | Not Null | 分类名称 |
| SortOrder | int | Default=0 | 排序 |
| IsActive | bit | Default=1 | 是否启用 |
| CreatedAt | datetime2 | Not Null | 创建时间 |
| CreatedBy | nvarchar(100) | Not Null | 创建人 |

#### 3.2.3 料卷相关表

**ReelIds (料卷表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| ReelNo | nvarchar(100) | Unique, Not Null | 料卷编号 |
| PartNoId | bigint | FK, Not Null | 所属物料 |
| CellId | bigint | FK, Null | 存储位置 |
| InitialQuantity | decimal(18,6) | Not Null | 初始数量 |
| CurrentQuantity | decimal(18,6) | Not Null | 当前数量 |
| Status | int | Not Null, Default=0 | 状态: 0=在库, 1=出库, 2=在线 |
| ManufactureDate | datetime2 | Null | 生产日期 |
| ExpiryDate | datetime2 | Null | 过期日期 |
| ReceivedAt | datetime2 | Null | 收货时间 |
| FirstUseAt | datetime2 | Null | 首次使用时间 |
| LastUseAt | datetime2 | Null | 最后使用时间 |
| IsActive | bit | Default=1 | 是否启用 |
| CreatedAt | datetime2 | Not Null | 创建时间 |
| CreatedBy | nvarchar(100) | Not Null | 创建人 |
| UpdatedAt | datetime2 | Null | 更新时间 |
| UpdatedBy | nvarchar(100) | Null | 更新人 |

**ReelUsageLogs (料卷使用记录表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| ReelId | bigint | FK, Not Null | 料卷ID |
| EmployeeId | bigint | FK, Not Null | 操作员工 |
| UsageType | int | Not Null | 使用类型: 0=出库, 1=上线使用, 2=退库, 3=报废 |
| Quantity | decimal(18,6) | Not Null | 使用数量 |
| RemainingQuantity | decimal(18,6) | Not Null | 剩余数量 |
| UsageDuration | int | Null | 使用时长(分钟) |
| Remark | nvarchar(500) | Null | 备注 |
| RecordedAt | datetime2 | Not Null | 记录时间 |
| CreatedAt | datetime2 | Not Null | 创建时间 |

#### 3.2.4 员工与用户相关表

**Employees (员工表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| EmployeeNo | nvarchar(50) | Unique, Not Null | 员工工号 |
| Name | nvarchar(100) | Not Null | 姓名 |
| Department | nvarchar(100) | Null | 部门 |
| Position | nvarchar(100) | Null | 职位 |
| Phone | nvarchar(50) | Null | 电话 |
| Email | nvarchar(200) | Null | 邮箱 |
| IsActive | bit | Default=1 | 是否启用 |
| CreatedAt | datetime2 | Not Null | 创建时间 |
| CreatedBy | nvarchar(100) | Not Null | 创建人 |
| UpdatedAt | datetime2 | Null | 更新时间 |
| UpdatedBy | nvarchar(100) | Null | 更新人 |

**Users (用户表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| EmployeeId | bigint | FK, Unique, Not Null | 员工ID |
| Username | nvarchar(100) | Unique, Not Null | 用户名 |
| PasswordHash | nvarchar(500) | Not Null | 密码哈希 |
| UserType | int | Not Null | 用户类型: 0=普通用户, 1=管理员, 2=系统管理员 |
| IsActive | bit | Default=1 | 是否启用 |
| LastLoginAt | datetime2 | Null | 最后登录时间 |
| LastLoginIp | nvarchar(50) | Null | 最后登录IP |
| CreatedAt | datetime2 | Not Null | 创建时间 |
| CreatedBy | nvarchar(100) | Not Null | 创建人 |
| UpdatedAt | datetime2 | Null | 更新时间 |
| UpdatedBy | nvarchar(100) | Null | 更新人 |

**Roles (角色表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| Code | nvarchar(50) | Unique, Not Null | 角色编码 |
| Name | nvarchar(100) | Not Null | 角色名称 |
| Description | nvarchar(500) | Null | 角色描述 |
| IsActive | bit | Default=1 | 是否启用 |
| CreatedAt | datetime2 | Not Null | 创建时间 |
| CreatedBy | nvarchar(100) | Not Null | 创建人 |

**Permissions (权限表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| Code | nvarchar(100) | Unique, Not Null | 权限编码 |
| Name | nvarchar(100) | Not Null | 权限名称 |
| Module | nvarchar(50) | Not Null | 所属模块 |
| Description | nvarchar(500) | Null | 权限描述 |
| IsActive | bit | Default=1 | 是否启用 |

**RolePermissions (角色权限关联表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| RoleId | bigint | FK, Not Null | 角色ID |
| PermissionId | bigint | FK, Not Null | 权限ID |

**UserRoles (用户角色关联表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| UserId | bigint | FK, Not Null | 用户ID |
| RoleId | bigint | FK, Not Null | 角色ID |

#### 3.2.5 日志相关表

**OperationLogs (操作日志表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| UserId | bigint | FK, Not Null |操作用户 |
| Module | nvarchar(50) | Not Null | 操作模块 |
| Action | nvarchar(100) | Not Null | 操作动作 |
| EntityType | nvarchar(100) | Null | 实体类型 |
| EntityId | nvarchar(100) | Null | 实体ID |
| OldValue | nvarchar(max) | Null | 旧值(JSON) |
| NewValue | nvarchar(max) | Null | 新值(JSON) |
| IpAddress | nvarchar(50) | Null | IP地址 |
| UserAgent | nvarchar(500) | Null | 用户代理 |
| Remark | nvarchar(500) | Null | 备注 |
| CreatedAt | datetime2 | Not Null | 操作时间 |

#### 3.2.6 预警相关表

**WarningConfigs (预警配置表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| PartNoId | bigint | FK, Null | 物料ID(Null表示全局) |
| WarningType | int | Not Null | 预警类型: 0=数量预警, 1=时间预警, 2=双预警 |
| QuantityThreshold | decimal(18,6) | Null | 数量阈值 |
| DaysThreshold | int | Null | 天数阈值 |
| IsEnabled | bit | Default=1 | 是否启用 |
| CreatedAt | datetime2 | Not Null | 创建时间 |
| CreatedBy | nvarchar(100) | Not Null | 创建人 |
| UpdatedAt | datetime2 | Null | 更新时间 |
| UpdatedBy | nvarchar(100) | Null | 更新人 |

**WarningRecords (预警记录表)**
| 字段名 | 数据类型 | 约束 | 说明 |
|--------|----------|------|------|
| Id | bigint | PK, Identity | 主键 |
| ReelId | bigint | FK, Not Null | 料卷ID |
| WarningType | int | Not Null | 预警类型 |
| CurrentQuantity | decimal(18,6) | Not Null | 当前数量 |
| RemainingDays | int | Null | 剩余天数 |
| WarningLevel | int | Not Null | 预警级别: 0=正常, 1=提醒, 2=警告, 3=严重 |
| IsPushed | bit | Default=0 | 是否已推送 |
| PushedAt | datetime2 | Null | 推送时间 |
| IsResolved | bit | Default=0 | 是否已处理 |
| ResolvedAt | datetime2 | Null | 处理时间 |
| ResolvedBy | nvarchar(100) | Null | 处理人 |
| Remark | nvarchar(500) | Null | 处理备注 |
| CreatedAt | datetime2 | Not Null | 创建时间 |

---

## 4. API接口规范

### 4.1 API基础信息
- 基础路径: `/api/v1`
- 认证方式: JWT Bearer Token
- 数据格式: JSON
- 分页格式: `?page=1&pageSize=20`

### 4.2 通用响应格式
```json
{
  "code": 200,
  "message": "操作成功",
  "data": {},
  "total": 100,
  "page": 1,
  "pageSize": 20
}
```

### 4.3 错误码定义
| 错误码 | 说明 |
|--------|------|
| 200 | 成功 |
| 400 | 请求参数错误 |
| 401 | 未授权 |
| 403 | 禁止访问 |
| 404 | 资源不存在 |
| 500 | 服务器内部错误 |

### 4.4 API接口列表

#### 4.4.1 认证模块 `/api/v1/auth`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| POST | /login | 用户登录 | Public |
| POST | /logout | 用户登出 | Auth |
| GET | /current | 获取当前用户信息 | Auth |
| POST | /refresh-token | 刷新Token | Auth |

#### 4.4.2 库房模块 `/api/v1/warehouses`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | / | 获取库房列表 | Auth |
| GET | /{id} | 获取库房详情 | Auth |
| POST | / | 创建库房 | Admin |
| PUT | /{id} | 更新库房 | Admin |
| DELETE | /{id} | 删除库房 | Admin |
| GET | /{id}/racks | 获取库房下的货架 | Auth |

#### 4.4.3 货架模块 `/api/v1/racks`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | / | 获取货架列表 | Auth |
| GET | /{id} | 获取货架详情 | Auth |
| POST | / | 创建货架 | Admin |
| PUT | /{id} | 更新货架 | Admin |
| DELETE | /{id} | 删除货架 | Admin |
| GET | /{id}/layers | 获取货架下的层 | Auth |

#### 4.4.4 层模块 `/api/v1/layers`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | / | 获取层列表 | Auth |
| GET | /{id} | 获取层详情 | Auth |
| POST | / | 创建层 | Admin |
| PUT | /{id} | 更新层 | Admin |
| DELETE | /{id} | 删除层 | Admin |
| GET | /{id}/cells | 获取层下的存储格 | Auth |

#### 4.4.5 存储格模块 `/api/v1/cells`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | / | 获取存储格列表 | Auth |
| GET | /{id} | 获取存储格详情 | Auth |
| POST | / | 创建存储格 | Admin |
| PUT | /{id} | 更新存储格 | Admin |
| DELETE | /{id} | 删除存储格 | Admin |
| PUT | /{id}/mode | 设置存储模式 | Admin |
| GET | /{id}/reels | 获取存储格内的料卷 | Auth |

#### 4.4.6 物料模块 `/api/v1/partnos`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | / | 获取物料列表 | Auth |
| GET | /{id} | 获取物料详情 | Auth |
| GET | /{id}/reels | 获取物料的料卷列表 | Auth |
| POST | / | 创建物料 | Admin |
| PUT | /{id} | 更新物料 | Admin |
| DELETE | /{id} | 删除物料 | Admin |
| GET | /{id}/statistics | 获取物料统计 | Auth |

#### 4.4.7 物料分类模块 `/api/v1/partno-categories`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /tree | 获取分类树 | Auth |
| POST | / | 创建分类 | Admin |
| PUT | /{id} | 更新分类 | Admin |
| DELETE | /{id} | 删除分类 | Admin |

#### 4.4.8 料卷模块 `/api/v1/reels`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | / | 获取料卷列表 | Auth |
| GET | /{id} | 获取料卷详情 | Auth |
| GET | /{id}/usage-logs | 获取料卷使用记录 | Auth |
| POST | / | 创建料卷(入库) | Operator |
| PUT | /{id}/checkout | 料卷出库 | Operator |
| PUT | /{id}/online | 料卷上线 | Operator |
| PUT | /{id}/offline | 料卷下线 | Operator |
| PUT | /{id}/return | 料卷退库 | Operator |
| PUT | /{id}/scrap | 料卷报废 | Operator |
| DELETE | /{id} | 删除料卷 | Admin |

#### 4.4.9 使用记录模块 `/api/v1/usage-logs`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | / | 获取使用记录列表 | Auth |
| GET | /{id} | 获取使用记录详情 | Auth |
| POST | /upload | 上传使用数据 | Operator |
| GET | /statistics | 使用统计 | Auth |

#### 4.4.10 员工模块 `/api/v1/employees`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | / | 获取员工列表 | Admin |
| GET | /{id} | 获取员工详情 | Admin |
| POST | / | 创建员工 | Admin |
| PUT | /{id} | 更新员工 | Admin |
| DELETE | /{id} | 删除员工 | Admin |

#### 4.4.11 用户模块 `/api/v1/users`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | / | 获取用户列表 | Admin |
| GET | /{id} | 获取用户详情 | Admin |
| GET | /{id}/roles | 获取用户角色 | Admin |
| POST | / | 创建用户 | Admin |
| PUT | /{id} | 更新用户 | Admin |
| PUT | /{id}/password | 重置密码 | Admin |
| DELETE | /{id} | 删除用户 | Admin |
| PUT | /{id}/roles | 分配角色 | Admin |

#### 4.4.12 角色模块 `/api/v1/roles`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | / | 获取角色列表 | Admin |
| GET | /{id} | 获取角色详情 | Admin |
| GET | /{id}/permissions | 获取角色权限 | Admin |
| POST | / | 创建角色 | Admin |
| PUT | /{id} | 更新角色 | Admin |
| DELETE | /{id} | 删除角色 | Admin |
| PUT | /{id}/permissions | 分配权限 | Admin |

#### 4.4.13 权限模块 `/api/v1/permissions`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /modules | 获取权限模块列表 | Admin |
| GET | / | 获取权限列表 | Admin |

#### 4.4.14 预警模块 `/api/v1/warnings`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /configs | 获取预警配置列表 | Admin |
| PUT | /configs | 更新预警配置 | Admin |
| GET | /records | 获取预警记录列表 | Auth |
| PUT | /records/{id}/resolve | 处理预警 | Operator |
| GET | /active | 获取当前预警列表 | Auth |

#### 4.4.15 看板模块 `/api/v1/dashboard`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /overview | 概览统计 | Auth |
| GET | /inventory | 库存统计 | Auth |
| GET | /usage-trend | 使用趋势 | Auth |
| GET | /warning-summary | 预警汇总 | Auth |

#### 4.4.16 日志模块 `/api/v1/logs`
| 方法 | 路径 | 说明 | 权限 |
|------|------|------|------|
| GET | /operations | 获取操作日志 | Admin |
| GET | /operations/{id} | 获取日志详情 | Admin |

#### 4.4.17 SignalR Hub `/hubs/warning`
| 方法 | 说明 | 权限 |
|------|------|------|
| JoinGroup | 加入预警组 | Auth |
| LeaveGroup | 离开预警组 | Auth |
| OnWarningPushed | 接收预警推送(服务端调用) | - |

---

## 5. 实时通信设计(SignalR)

### 5.1 Hub端点
- 路径: `/hubs/warning`
- 认证: JWT Bearer Token

### 5.2 预警推送流程
1. 定时任务扫描预警数据(每5分钟)
2. 达到预警条件的Reel生成WarningRecord
3. 通过SignalR推送预警到前端
4. 前端接收预警并展示

### 5.3 预警数据结构
```json
{
  "type": "warning",
  "data": {
    "id": 1,
    "reelId": 123,
    "reelNo": "REEL-2024-001",
    "partNo": "PART-001",
    "partName": "测试物料",
    "warningType": 0,
    "warningLevel": 2,
    "currentQuantity": 10.5,
    "threshold": 20,
    "remainingDays": 5,
    "message": "物料PART-001剩余数量10.5，低于阈值20"
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## 6. 前端架构设计

### 6.1 Vue3移动端结构
```
MaterialManageSystem.Vue/
├── src/
│   ├── api/                    # API调用
│   │   ├── index.ts           # Axios配置
│   │   ├── auth.ts
│   │   ├── warehouse.ts
│   │   ├── partno.ts
│   │   ├── reel.ts
│   │   ├── warning.ts
│   │   └── dashboard.ts
│   ├── components/            # 公共组件
│   ├── composables/            # 组合式函数
│   ├── layouts/                # 布局组件
│   ├── pages/                  # 页面
│   │   ├── auth/
│   │   ├── home/
│   │   ├── warehouse/
│   │   ├── partno/
│   │   ├── reel/
│   │   ├── warning/
│   │   └── dashboard/
│   ├── router/                 # 路由
│   ├── stores/                 # Pinia状态
│   ├── types/                  # TypeScript类型
│   ├── utils/                  # 工具函数
│   ├── App.vue
│   └── main.ts
├── index.html
├── vite.config.ts
└── package.json
```

### 6.2 Bootstrap管理端结构
```
MaterialManageSystem.Admin/
├── Pages/
│   ├── _Layout.cshtml
│   ├── _ViewImports.cshtml
│   ├── Login.cshtml
│   ├── Dashboard.cshtml
│   ├── Warehouse/
│   ├── PartNo/
│   ├── Reel/
│   ├── Employee/
│   ├── User/
│   ├── Role/
│   ├── Warning/
│   └── Log/
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── lib/
└── Program.cs
```

---

## 7. 安全与权限管理

### 7.1 权限模块定义
| 模块 | 权限编码 | 说明 |
|------|----------|------|
| 库房 | Warehouse.View | 查看库房 |
| 库房 | Warehouse.Create | 创建库房 |
| 库房 | Warehouse.Edit | 编辑库房 |
| 库房 | Warehouse.Delete | 删除库房 |
| 货架 | Rack.View | 查看货架 |
| 货架 | Rack.Create | 创建货架 |
| 货架 | Rack.Edit | 编辑货架 |
| 货架 | Rack.Delete | 删除货架 |
| 物料 | PartNo.View | 查看物料 |
| 物料 | PartNo.Create | 创建物料 |
| 物料 | PartNo.Edit | 编辑物料 |
| 物料 | PartNo.Delete | 删除物料 |
| 料卷 | Reel.View | 查看料卷 |
| 料卷 | Reel.Create | 创建料卷 |
| 料卷 | Reel.Checkout | 料卷出库 |
| 料卷 | Reel.Online | 料卷上线 |
| 料卷 | Reel.Return | 料卷退库 |
| 料卷 | Reel.Scrap | 料卷报废 |
| 料卷 | Reel.Delete | 删除料卷 |
| 员工 | Employee.View | 查看员工 |
| 员工 | Employee.Create | 创建员工 |
| 员工 | Employee.Edit | 编辑员工 |
| 员工 | Employee.Delete | 删除员工 |
| 用户 | User.View | 查看用户 |
| 用户 | User.Create | 创建用户 |
| 用户 | User.Edit | 编辑用户 |
| 用户 | User.Delete | 删除用户 |
| 用户 | User.ResetPassword | 重置密码 |
| 角色 | Role.View | 查看角色 |
| 角色 | Role.Create | 创建角色 |
| 角色 | Role.Edit | 编辑角色 |
| 角色 | Role.Delete | 删除角色 |
| 角色 | Role.AssignPermission | 分配权限 |
| 预警 | Warning.View | 查看预警 |
| 预警 | Warning.Config | 配置预警 |
| 预警 | Warning.Resolve | 处理预警 |
| 日志 | Log.View | 查看日志 |

### 7.2 内置角色
| 角色编码 | 角色名称 | 权限 |
|----------|----------|------|
| SystemAdmin | 系统管理员 | 所有权限 |
| Admin | 管理员 | 大部分权限 |
| Operator | 操作员 | 操作相关权限 |
| Viewer | 查看者 | 查看权限 |

---

## 8. 分步实现指南

### 步骤1: 创建解决方案和项目结构
```bash
# 创建解决方案
dotnet new sln -n MaterialManageSystem

# 创建各项目
dotnet new classlib -n MaterialManageSystem.Core -o MaterialManageSystem.Core
dotnet new classlib -n MaterialManageSystem.Infrastructure -o MaterialManageSystem.Infrastructure
dotnet new webapi -n MaterialManageSystem.Api -o MaterialManageSystem.Api
dotnet new razor -n MaterialManageSystem.Admin -o MaterialManageSystem.Admin

# 添加项目到解决方案
dotnet sln add MaterialManageSystem.Core/MaterialManageSystem.Core.csproj
dotnet sln add MaterialManageSystem.Infrastructure/MaterialManageSystem.Infrastructure.csproj
dotnet sln add MaterialManageSystem.Api/MaterialManageSystem.Api.csproj
dotnet sln add MaterialManageSystem.Admin/MaterialManageSystem.Admin.csproj

# 添加项目引用
dotnet add MaterialManageSystem.Infrastructure reference MaterialManageSystem.Core
dotnet add MaterialManageSystem.Api reference MaterialManageSystem.Core MaterialManageSystem.Infrastructure
dotnet add MaterialManageSystem.Admin reference MaterialManageSystem.Core MaterialManageSystem.Infrastructure
```

### 步骤2: 安装NuGet包
```bash
# Core项目
dotnet add MaterialManageSystem.Core/MaterialManageSystem.Core.csproj package Microsoft.Extensions.DependencyInjection.Abstractions

# Infrastructure项目
dotnet add MaterialManageSystem.Infrastructure/MaterialManageSystem.Infrastructure.csproj package Microsoft.EntityFrameworkCore
dotnet add MaterialManageSystem.Infrastructure/MaterialManageSystem.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add MaterialManageSystem.Infrastructure/MaterialManageSystem.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design

# Api项目
dotnet add MaterialManageSystem.Api/MaterialManageSystem.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add MaterialManageSystem.Api/MaterialManageSystem.Api.csproj package Microsoft.AspNetCore.SignalR
dotnet add MaterialManageSystem.Api/MaterialManageSystem.Api.csproj package Swashbuckle.AspNetCore
dotnet add MaterialManageSystem.Api/MaterialManageSystem.Api.csproj package Serilog.AspNetCore
dotnet add MaterialManageSystem.Api/MaterialManageSystem.Api.csproj package Quartz.Extensions.Hosting
```

### 步骤3: 实现实体模型(Core/Entities)
按照数据库设计创建所有实体类。

### 步骤4: 实现DbContext(Infrastructure)
配置Entity Framework Core的DbContext和实体映射。

### 步骤5: 实现数据仓储(Core/Interfaces + Infrastructure/Repositories)
创建泛型仓储接口和实现。

### 步骤6: 实现业务服务层(Core/Services)
创建业务服务接口和实现。

### 步骤7: 实现API控制器(Api/Controllers)
实现RESTful API控制器。

### 步骤8: 实现SignalR Hub
实现预警推送Hub。

### 步骤9: 实现后台管理页面(Admin)
使用Razor Pages实现Bootstrap后台管理。

### 步骤10: 创建Vue3移动端前端
使用Vite创建Vue3项目并实现手机端网页。

### 步骤11: 实现定时预警任务
使用Quartz.NET实现定时预警检查。

### 步骤12: 测试和部署
单元测试 + 部署文档。

---

## 附录

### A. 配置参考
详见 appsettings.json 配置参考。

### B. 部署检查清单
1. 数据库迁移
2. API部署
3. 前端部署
4. Nginx配置(如有需要)
5. 防火墙设置
6. 备份策略

### C. 常见问题
1. 数据库连接问题
2. JWT认证问题
3. SignalR连接问题
4. 前端构建问题

---

## D. 详细实现历史 (v1.1.0)

### D.1 安全修复 - 密码哈希
**文件修改**: 
- `MaterialManageSystem.Api/Controllers/AuthController.cs`
- `MaterialManageSystem.Infrastructure/Data/DataSeeder.cs`
- 项目文件添加 `BCrypt.Net-Next` 包

**实现内容**:
- 使用 BCrypt 算法进行密码哈希和验证
- 移除明文密码比较
- 种子数据创建时使用 BCrypt 哈希

### D.2 料卷状态修复
**文件修改**:
- `MaterialManageSystem.Core/Enums/Enums.cs`
- `MaterialManageSystem.Api/Controllers/ReelsController.cs`

**实现内容**:
- 添加 `Scrapped = 3` 状态
- 修复报废操作状态设置
- 报废时设置 `IsActive = false`

### D.3 数据库种子数据
**新增文件**:
- `MaterialManageSystem.Infrastructure/Data/DataSeeder.cs`

**实现内容**:
- 创建默认管理员账户 (admin/Admin@123)
- 初始化员工数据
- 初始化库房、物料分类、预警配置

### D.4 API授权保护
**文件修改**:
- `AuthController.cs` (添加 AllowAnonymous)
- `WarehousesController.cs` (添加 Authorize)
- `PartNosController.cs` (添加 Authorize)
- `DashboardController.cs` (添加 Authorize)
- `ReelsController.cs` (添加 Authorize)

### D.5 事务处理 - 料卷操作事务保护
**文件修改/新增**:
- `MaterialManageSystem.Core/Interfaces/IRepository.cs` (新增 Add/Update/SaveChangesAsync)
- `MaterialManageSystem.Infrastructure/Repositories/Repository.cs` (新增事务相关方法)
- `MaterialManageSystem.Api/Controllers/ReelsController.cs` (添加事务逻辑)

**实现内容**:
- 在 IRepository 接口添加 `Add()`, `Update()`, `SaveChangesAsync()` 方法
- 在 Repository 基类实现事务支持
- ReelsController 中为所有关键操作添加 `BeginTransactionAsync()`
- 创建料卷、出库、上线、退库、报废都在事务内执行
- 出现异常时调用 `RollbackAsync()` 回滚

### D.6 预警检测服务 - 实现实际预警逻辑
**文件修改/新增**:
- `MaterialManageSystem.Core/Interfaces/ISpecificRepositories.cs` (新增 IWarningConfigRepository/IWarningRecordRepository)
- `MaterialManageSystem.Core/Interfaces/IServices.cs` (新增 IWarningDetectionService)
- `MaterialManageSystem.Infrastructure/Repositories/SpecificRepositories.cs` (新增警告仓储)
- `MaterialManageSystem.Infrastructure/Services/WarningDetectionService.cs` (新增预警检测服务)
- `MaterialManageSystem.Api/HostedServices/WarningDetectionHostedService.cs` (新增后台任务)
- `MaterialManageSystem.Api/Program.cs` (注册服务和后台任务)
- `MaterialManageSystem.Api/Controllers/DashboardController.cs` (使用真实预警数据)

**实现内容**:
- 完整的预警检测服务，包含库存不足、过期、存储时间过长三种类型
- 后台定时任务每5分钟执行一次检测
- 支持全局和特定物料的预警配置
- 检测到预警时创建记录并通过 SignalR 推送到前端
- Dashboard 使用真实的预警数据，不再返回空数据

### D.7 全局异常处理 - 统一异常中间件
**文件修改/新增**:
- `MaterialManageSystem.Api/Middleware/ExceptionHandlingMiddleware.cs` (新增)
- `MaterialManageSystem.Api/Program.cs` (注册中间件)

**实现内容**:
- 创建 ExceptionHandlingMiddleware 中间件
- 统一捕获所有未处理异常
- 根据异常类型返回不同状态码
- 错误响应保持与 ApiResponse 一致的格式
- 集成日志记录

### D.8 输入验证 - DTO数据注解验证
**文件修改**:
- `MaterialManageSystem.Api/DTOs/Dtos.cs` (添加验证特性)
- `MaterialManageSystem.Api/Program.cs` (配置验证响应)

**实现内容**:
- 为 LoginRequest 添加 `[Required]` 验证
- 为 CreateReelRequest 添加 `[Required]`, `[StringLength]`, `[Range]`
- 为 UsageUploadRequest 添加 `[Required]`, `[Range]`, `[StringLength]`
- 配置 InvalidModelStateResponseFactory 以 ApiResponse 格式返回验证错误
- 自动拼接多个验证错误消息

---

## E. 功能完成说明 (v1.1.0)

所有中等优先级改进项已全部实现完成！
