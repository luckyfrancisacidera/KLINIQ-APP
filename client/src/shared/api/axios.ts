import Axios, { AxiosHeaders, type AxiosError, type InternalAxiosRequestConfig } from "axios"
import env from "../config/env"

export const SESSION_EXPIRED_EVENT = "kliniq:session-expired"

const apiClient = Axios.create({
  baseURL: env.apiBaseUrl,
  withCredentials: true,
  timeout: 20_000,
  headers: {
    Accept: "application/json",
  },
})

apiClient.interceptors.request.use((config) => {
  if (config.data instanceof FormData) {
    const headers = AxiosHeaders.from(config.headers)
    headers.delete("Content-Type")
    config.headers = headers
  }
  return config
})

let isRefreshing = false
let queue: Array<{ resolve: () => void; reject: (error: unknown) => void }> = []

const processQueue = (error?: unknown) => {
  queue.forEach((pending) => (error ? pending.reject(error) : pending.resolve()))
  queue = []
}

const isAuthenticationRequest = (url?: string) =>
  Boolean(url && ["/auth/login", "/auth/register", "/auth/refresh-token"].some((path) => url.includes(path)))

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as (InternalAxiosRequestConfig & { _retry?: boolean }) | undefined

    if (!originalRequest || error.response?.status !== 401 || originalRequest._retry || isAuthenticationRequest(originalRequest.url)) {
      return Promise.reject(error)
    }

    if (isRefreshing) {
      return new Promise<void>((resolve, reject) => queue.push({ resolve, reject }))
        .then(() => apiClient(originalRequest))
    }

    originalRequest._retry = true
    isRefreshing = true

    try {
      await apiClient.post("/auth/refresh-token")
      processQueue()
      return apiClient(originalRequest)
    } catch (refreshError) {
      processQueue(refreshError)
      window.dispatchEvent(new Event(SESSION_EXPIRED_EVENT))
      return Promise.reject(refreshError)
    } finally {
      isRefreshing = false
    }
  },
)

export default apiClient
