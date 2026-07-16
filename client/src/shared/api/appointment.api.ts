import apiClient from "./axios"
import type { AppointmentDto, BookAppointmentPayload, RescheduleAppointmentPayload } from "../types/appointment.types"
import type { PaginatedResult } from "../types/common.types"

export interface AppointmentListParams {
  page?: number
  pageSize?: number
  status?: string
  dateFrom?: string
  dateTo?: string
}

export const appointmentApi = {
  getById: (id: string, signal?: AbortSignal) => apiClient.get<AppointmentDto>(`/appointments/${id}`, { signal }),
  getByPatient: (patientId: string, params: AppointmentListParams = {}, signal?: AbortSignal) =>
    apiClient.get<PaginatedResult<AppointmentDto>>(`/appointments/patient/${patientId}`, { params, signal }),
  getByPractitioner: (practitionerId: string, params: AppointmentListParams = {}, signal?: AbortSignal) =>
    apiClient.get<PaginatedResult<AppointmentDto>>(`/appointments/practitioner/${practitionerId}`, { params, signal }),
  book: (payload: BookAppointmentPayload) => apiClient.post<AppointmentDto>("/appointments", payload),
  reschedule: (id: string, payload: RescheduleAppointmentPayload) => apiClient.post<AppointmentDto>(`/appointments/${id}/reschedule`, payload),
  confirm: (id: string) => apiClient.post<AppointmentDto>(`/appointments/${id}/confirm`),
  queue: (id: string) => apiClient.post<AppointmentDto>(`/appointments/${id}/queue`),
  startConsultation: (id: string) => apiClient.post<AppointmentDto>(`/appointments/${id}/start-consultation`),
  cancel: (id: string) => apiClient.post<AppointmentDto>(`/appointments/${id}/cancel`),
  complete: (id: string, notes?: string) => apiClient.post<AppointmentDto>(`/appointments/${id}/complete`, { notes }),
}
