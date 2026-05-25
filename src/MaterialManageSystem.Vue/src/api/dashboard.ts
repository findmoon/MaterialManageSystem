import api from './index'

export interface DashboardDto {
  totalWarehouses: number
  totalPartNos: number
  totalReels: number
  inStockReels: number
  outStockReels: number
  onlineReels: number
  activeWarnings: number
}

export const dashboardApi = {
  getOverview: async (): Promise<DashboardDto> => {
    return api.get<DashboardDto>('/dashboard/overview') as Promise<DashboardDto>
  },
  getInventory: async () => {
    return api.get('/dashboard/inventory')
  },
  getUsageTrend: async () => {
    return api.get('/dashboard/usage-trend')
  },
  getWarningSummary: async () => {
    return api.get('/dashboard/warning-summary')
  }
}
