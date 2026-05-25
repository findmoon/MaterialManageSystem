import api from './index'

export interface WarehouseDto {
  id: number
  code: string
  name: string
  location?: string
  remark?: string
  isActive: boolean
}

export interface WarehouseListResponse {
  data: WarehouseDto[]
  total: number
  page: number
  pageSize: number
}

export const warehouseApi = {
  getAll: async (page = 1, pageSize = 20): Promise<WarehouseListResponse> => {
    return api.get<WarehouseListResponse>('/warehouses', {
      params: { page, pageSize }
    }) as Promise<WarehouseListResponse>
  },
  getById: async (id: number): Promise<WarehouseDto> => {
    return api.get<WarehouseDto>(`/warehouses/${id}`) as Promise<WarehouseDto>
  },
  create: async (data: Partial<WarehouseDto>): Promise<WarehouseDto> => {
    return api.post<WarehouseDto>('/warehouses', data) as Promise<WarehouseDto>
  },
  update: async (id: number, data: Partial<WarehouseDto>): Promise<WarehouseDto> => {
    return api.put<WarehouseDto>(`/warehouses/${id}`, data) as Promise<WarehouseDto>
  },
  delete: async (id: number) => {
    return api.delete(`/warehouses/${id}`)
  }
}
