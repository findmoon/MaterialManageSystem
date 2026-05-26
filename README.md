# 物料管理系统 (MaterialManageSystem)

一个基于 ASP.NET Core 10.0 和 Vue 3 的现代化物料管理平台，采用分层架构设计，提供物料入库、出库、库存预警、在线监控等核心功能。

## 🎯 功能特性

### 核心功能
- **物料管理**：物料信息的增删改查、分类管理、预警阈值配置
- **仓库管理**：库房、货架、层级、仓位的完整管理体系
- **料卷管理**：入库、出库、上线、退回、报废等完整操作流程
- **预警系统**：库存不足预警、过期预警、实时推送通知
- **实时监控**：基于 SignalR 的实时预警推送
- **操作日志**：完整的操作记录和审计追踪

### 技术特性
- 🔒 **安全认证**：JWT Token 认证 + BCrypt 密码哈希
- 🛡️ **授权保护**：基于角色的权限控制
- 💾 **事务处理**：完整的数据库事务保护
- ⚡ **实时通信**：SignalR 实时预警推送
- 📊 **数据统计**：仪表盘数据展示和趋势分析

## 🛠️ 技术栈

### 后端
| 技术 | 版本 | 说明 |
|------|------|------|
| ASP.NET Core | 10.0 | Web API 框架 |
| Entity Framework Core | 8.0 | ORM 框架 |
| SQL Server | 2019+ | 数据库 |
| SignalR | 8.0 | 实时通信 |
| JWT | - | 身份认证 |
| BCrypt.Net-Next | 4.0 | 密码哈希 |
| AutoMapper | 12.0 | 对象映射 |
| Serilog | 10.0 | 结构化日志 |

### 前端
| 技术 | 版本 | 说明 |
|------|------|------|
| Vue | 3.4 | 前端框架 |
| TypeScript | 5.3 | 类型安全 |
| Vite | 5.0 | 构建工具 |
| Pinia | 2.1 | 状态管理 |
| Vue Router | 4.2 | 路由管理 |
| Bootstrap | 5.3 | UI 框架 |
| TailwindCSS | 3.4 | CSS 框架 |
| Axios | 1.6 | HTTP 客户端 |

## 📁 项目结构

```
MaterialManageSystem/
├── MaterialManageSystem.Core/          # 核心层
│   ├── Entities/                       # 领域实体
│   ├── DTOs/                           # 数据传输对象
│   ├── Enums/                          # 枚举类型
│   └── Interfaces/                     # 业务接口
├── MaterialManageSystem.Infrastructure/ # 基础设施层
│   ├── Data/                           # 数据库上下文
│   ├── Repositories/                   # 仓储实现
│   └── Services/                       # 业务服务
├── MaterialManageSystem.Api/            # API层
│   ├── Controllers/                    # API控制器
│   ├── Middleware/                     # 中间件
│   ├── HostedServices/                 # 后台定时任务
│   ├── Hubs/                           # SignalR Hub
│   └── Mapping/                        # 对象映射配置
├── MaterialManageSystem.Admin/         # 管理后台 (Razor Pages)
│   ├── Pages/                          # 页面组件
│   └── wwwroot/                        # 静态资源
├── MaterialManageSystem.Vue/           # 前端 (Vue 3)
│   ├── src/
│   │   ├── pages/                      # 页面组件
│   │   ├── api/                        # API调用封装
│   │   ├── stores/                     # 状态管理
│   │   └── router/                     # 路由配置
│   └── dist/                           # 构建产物
└── docs/                               # 文档目录
```

## 🚀 快速开始

### 环境要求
- .NET 8.0 SDK 或更高版本
- Node.js 20.x (LTS)
- SQL Server 2019+ 或 SQL Server Express

### 数据库配置

1. 创建数据库：
```sql
CREATE DATABASE MaterialManageSystem
GO
```

2. 更新连接字符串（`MaterialManageSystem.Api/appsettings.json`）：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MaterialManageSystem;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

### 运行后端服务

```bash
# 进入 API 目录
cd MaterialManageSystem.Api

# 安装依赖
dotnet restore

# 执行数据库迁移
dotnet ef database update --project ../MaterialManageSystem.Infrastructure --startup-project .

# 启动服务
dotnet run --urls "http://localhost:5000"
```

### 运行前端服务

```bash
# 进入 Vue 目录
cd MaterialManageSystem.Vue

# 安装依赖
npm install

# 启动开发服务器
npm run dev
```

### 访问地址
- API 服务：http://localhost:5000
- Swagger 文档：http://localhost:5000/swagger
- 管理后台：http://localhost:5000
- 前端页面：http://localhost:5173

## 🔐 默认账户

系统启动时自动创建默认管理员账户：

| 用户名 | 密码 | 用户类型 |
|--------|------|----------|
| admin | Admin@123 | 系统管理员 |

## 📡 API 接口

### 基础信息
- 基础路径：`/api/v1`
- 认证方式：JWT Bearer Token
- 数据格式：JSON

### 主要接口

| 模块 | 路径 | 功能 |
|------|------|------|
| 认证 | `/api/v1/auth` | 登录、登出、获取当前用户 |
| 仪表盘 | `/api/v1/dashboard` | 获取概览数据 |
| 库房 | `/api/v1/warehouses` | 库房 CRUD |
| 物料 | `/api/v1/partnos` | 物料 CRUD |
| 料卷 | `/api/v1/reels` | 料卷管理（入库、出库、上线、退回、报废） |
| 预警 | `/api/v1/warnings` | 预警配置和记录 |

### 响应格式

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

## 📦 部署

### Docker 部署

创建 `Dockerfile`：
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MaterialManageSystem.Api/MaterialManageSystem.Api.csproj", "MaterialManageSystem.Api/"]
COPY ["MaterialManageSystem.Infrastructure/MaterialManageSystem.Infrastructure.csproj", "MaterialManageSystem.Infrastructure/"]
COPY ["MaterialManageSystem.Core/MaterialManageSystem.Core.csproj", "MaterialManageSystem.Core/"]
RUN dotnet restore "MaterialManageSystem.Api/MaterialManageSystem.Api.csproj"
COPY . .
WORKDIR "/src/MaterialManageSystem.Api"
RUN dotnet build "MaterialManageSystem.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MaterialManageSystem.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MaterialManageSystem.Api.dll"]
```

构建并运行：
```bash
docker build -t materialmanageapi .
docker run -d -p 5000:80 --name materialmanageapi materialmanageapi
```

### 前端部署

```bash
# 构建生产版本
npm run build

# 部署到 Nginx
# 配置 nginx.conf 反向代理
```

## 🧪 测试

```bash
# 运行单元测试
dotnet test tests/MaterialManageSystem.Tests/MaterialManageSystem.Tests.csproj

# API 测试（使用 curl 或 Postman）
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}'
```

## 🤝 贡献指南

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打开 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情

## 📞 联系方式

如有问题或建议，请通过以下方式联系：

- 项目地址：https://github.com/your-repo/MaterialManageSystem
- 邮箱：contact@example.com

---

**版本**: v1.1.0  
**更新日期**: 2026-05-26  
**作者**: MaterialManageSystem Team
