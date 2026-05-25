import api from './index'

export interface RegisterRequest {
  username: string
  password: string
  confirmPassword: string
}

export interface RegisterResponse {
  userId: number
  username: string
}

export const registerApi = {
  register: async (data: RegisterRequest): Promise<RegisterResponse> => {
    if (data.password !== data.confirmPassword) {
      throw new Error('两次输入的密码不一致')
    }
    
    const response = await api.post<RegisterResponse>('/auth/register', {
      username: data.username,
      password: data.password
    })
    
    return response as RegisterResponse
  }
}
