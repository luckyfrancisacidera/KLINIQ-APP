import apiClient from "./axios"
import type { PaginatedResult } from "../types/common.types"
import type { PatientDto, UpdatePatientPayload } from "../types/patient.types"

export const patientApi = {
  getAll: (params: { page?: number; pageSize?: number; search?: string } = {}, signal?: AbortSignal) =>
    apiClient.get<PaginatedResult<PatientDto>>("/patient", { params, signal }),
  getCurrent: (signal?: AbortSignal) => apiClient.get<PatientDto>("/patient/me", { signal }),
  getById: (id: string, signal?: AbortSignal) => apiClient.get<PatientDto>(`/patient/${id}`, { signal }),
  update: (id: string, payload: UpdatePatientPayload) => apiClient.put<PatientDto>(`/patient/${id}`, payload),
  delete: (id: string) => apiClient.delete(`/patient/${id}`),
}
