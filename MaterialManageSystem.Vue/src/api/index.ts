import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api/v1',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor
api.interceptors.response.use(
  (response) => {
    // 后端返回的是 ApiResponse<T> 结构，直接返回 data 部分
    return response.data.data
  },
  (error) => {
    if (error.response) {
      const { code, message } = error.response.data
      if (code === 401) {
        localStorage.removeItem('token')
        window.location.href = '/login'
      }
      return Promise.reject({ code, message })
    }
    return Promise.reject(error)
  }
)

export default api
