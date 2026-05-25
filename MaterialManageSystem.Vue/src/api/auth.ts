import api from './index'

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  token: string
  userId: number
  username: string
  name: string
  userType: number
}

export const authApi = {
  login: async (data: LoginRequest): Promise<LoginResponse> => {
    return api.post<LoginResponse>('/auth/login', data) as Promise<LoginResponse>
  },
  logout: async () => {
    return api.post('/auth/logout')
  },
  getCurrentUser: async (): Promise<LoginResponse> => {
    return api.get<LoginResponse>('/auth/current') as Promise<LoginResponse>
  }
}
