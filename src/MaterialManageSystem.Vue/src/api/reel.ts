import api from './index'

export interface ReelIdDto {
  id: number
  reelNo: string
  partNoId: number
  partNoCode: string
  partNoName: string
  cellId?: number
  cellLocation?: string
  initialQuantity: number
  currentQuantity: number
  status: number
  statusName: string
  manufactureDate?: string
  expiryDate?: string
  receivedAt?: string
  firstUseAt?: string
  lastUseAt?: string
}

export interface CreateReelRequest {
  reelNo: string
  partNoId: number
  cellId?: number
  initialQuantity: number
  manufactureDate?: string
  expiryDate?: string
}

export interface ReelListResponse {
  data: ReelIdDto[]
  total: number
  page: number
  pageSize: number
}

export const reelApi = {
  getAll: async (page = 1, pageSize = 20, status?: number): Promise<ReelListResponse> => {
    return api.get<ReelListResponse>('/reels', {
      params: { page, pageSize, status }
    }) as Promise<ReelListResponse>
  },
  getById: async (id: number): Promise<ReelIdDto> => {
    return api.get<ReelIdDto>(`/reels/${id}`) as Promise<ReelIdDto>
  },
  getUsageLogs: async (id: number) => {
    return api.get(`/reels/${id}/usage-logs`)
  },
  create: async (data: CreateReelRequest): Promise<ReelIdDto> => {
    return api.post<ReelIdDto>('/reels', data) as Promise<ReelIdDto>
  },
  checkout: async (id: number, data: { employeeId: number; quantity?: number; remark?: string }) => {
    return api.put(`/reels/${id}/checkout`, data)
  },
  online: async (id: number, data: { employeeId: number; quantity: number; usageDuration?: number; remark?: string }) => {
    return api.put(`/reels/${id}/online`, data)
  },
  return: async (id: number, data: { employeeId: number; quantity: number; remark?: string }) => {
    return api.put(`/reels/${id}/return`, data)
  },
  scrap: async (id: number, data: { employeeId: number; remark?: string }) => {
    return api.put(`/reels/${id}/scrap`, data)
  }
}
