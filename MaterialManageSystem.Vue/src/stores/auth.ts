import { defineStore } from 'pinia'
import { ref } from 'vue'
import { authApi } from '@/api/auth'
import type { LoginResponse } from '@/api/auth'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const user = ref<LoginResponse | null>(null)

  const setToken = (newToken: string) => {
    token.value = newToken
    localStorage.setItem('token', newToken)
  }

  const setUser = (userData: LoginResponse) => {
    user.value = userData
  }

  const login = async (username: string, password: string) => {
    const response = await authApi.login({ username, password })
    setToken(response.token)
    setUser(response)
    return response
  }

  const logout = async () => {
    try {
      await authApi.logout()
    } catch (e) {
      // ignore
    }
    token.value = null
    user.value = null
    localStorage.removeItem('token')
  }

  const fetchCurrentUser = async () => {
    if (!token.value) return null
    try {
      const response = await authApi.getCurrentUser()
      setUser(response)
      return response
    } catch (e) {
      logout()
      return null
    }
  }

  const isLoggedIn = () => !!token.value

  return {
    token,
    user,
    login,
    logout,
    fetchCurrentUser,
    isLoggedIn
  }
})
