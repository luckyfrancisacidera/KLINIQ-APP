import type {
  AuthResponse,
  ChangePasswordPayload,
  ForgotPasswordPayload,
  LoginPayload,
  MeResponse,
  RegisterPayload,
  ResetPasswordPayload,
  SetPasswordPayload,
  SetPasswordResponse,
} from "../types/auth.types"
import apiClient from "./axios"

export const authApi = {
  login: (payload: LoginPayload) => apiClient.post<AuthResponse>("/auth/login", payload),
  register: (payload: RegisterPayload) => apiClient.post<AuthResponse>("/auth/register", payload),
  logout: () => apiClient.post("/auth/logout"),
  me: () => apiClient.get<MeResponse>("/auth/me"),
  setPassword: (payload: SetPasswordPayload) => apiClient.post<SetPasswordResponse>("/auth/set-password", payload),
  forgotPassword: (payload: ForgotPasswordPayload) => apiClient.post<{ message: string }>("/auth/forgot-password", payload),
  resetPassword: (payload: ResetPasswordPayload) => apiClient.post("/auth/reset-password", payload),
  changePassword: (payload: ChangePasswordPayload) => apiClient.post("/auth/change-password", payload),
}
