import api from './index'

export interface PartNoDto {
  id: number
  partNoCode: string
  name: string
  specification?: string
  size?: string
  packaging?: string
  unit: string
  categoryId?: number
  categoryName?: string
  totalQuantity: number
  warningQuantity?: number
  warningDays?: number
}

export interface PartNoListResponse {
  data: PartNoDto[]
  total: number
  page: number
  pageSize: number
}

export const partNoApi = {
  getAll: async (page = 1, pageSize = 20): Promise<PartNoListResponse> => {
    return api.get<PartNoListResponse>('/partnos', {
      params: { page, pageSize }
    }) as Promise<PartNoListResponse>
  },
  getById: async (id: number): Promise<PartNoDto> => {
    return api.get<PartNoDto>(`/partnos/${id}`) as Promise<PartNoDto>
  },
  create: async (data: Partial<PartNoDto>): Promise<PartNoDto> => {
    return api.post<PartNoDto>('/partnos', data) as Promise<PartNoDto>
  },
  update: async (id: number, data: Partial<PartNoDto>): Promise<PartNoDto> => {
    return api.put<PartNoDto>(`/partnos/${id}`, data) as Promise<PartNoDto>
  },
  delete: async (id: number) => {
    return api.delete(`/partnos/${id}`)
  }
}
