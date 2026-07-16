import apiClient from "./axios"
import type { PaginatedResult } from "../types/common.types"
import type {
  AddBreakPayload,
  AvailableSlotDto,
  CreateSchedulePayload,
  PractitionerDetailDto,
  PractitionerDto,
  ScheduleSummaryDto,
  UpdatePractitionerPayload,
  UpdateSchedulePayload,
} from "../types/practitioner.types"

export interface PractitionerSearchParams {
  search?: string
  specialization?: string
  page?: number
  pageSize?: number
}

export const practitionerApi = {
  getAll: (params?: PractitionerSearchParams, signal?: AbortSignal) =>
    apiClient.get<PaginatedResult<PractitionerDto>>("/practitioners", { params, signal }),
  getCurrent: (signal?: AbortSignal) => apiClient.get<PractitionerDetailDto>("/practitioners/me", { signal }),
  getById: (id: string, signal?: AbortSignal) => apiClient.get<PractitionerDetailDto>(`/practitioners/${id}`, { signal }),
  update: (id: string, payload: UpdatePractitionerPayload) => apiClient.put<PractitionerDetailDto>(`/practitioners/${id}`, payload),
  delete: (id: string) => apiClient.delete(`/practitioners/${id}`),
  getSchedules: (id: string, signal?: AbortSignal) => apiClient.get<ScheduleSummaryDto[]>(`/practitioners/${id}/schedules`, { signal }),
  getAvailableSlots: (id: string, from?: string, to?: string, signal?: AbortSignal) =>
    apiClient.get<AvailableSlotDto[]>(`/practitioners/${id}/available-slots`, { params: { from, to }, signal }),
  createSchedule: (id: string, payload: CreateSchedulePayload) => apiClient.post<ScheduleSummaryDto>(`/practitioners/${id}/schedules`, payload),
  updateSchedule: (practitionerId: string, scheduleId: string, payload: UpdateSchedulePayload) =>
    apiClient.put<ScheduleSummaryDto>(`/practitioners/${practitionerId}/schedules/${scheduleId}`, payload),
  deleteSchedule: (practitionerId: string, scheduleId: string) => apiClient.delete(`/practitioners/${practitionerId}/schedules/${scheduleId}`),
  addBreak: (practitionerId: string, scheduleId: string, payload: AddBreakPayload) =>
    apiClient.post<ScheduleSummaryDto>(`/practitioners/${practitionerId}/schedules/${scheduleId}/breaks`, payload),
  deleteBreak: (practitionerId: string, scheduleId: string, breakId: string) =>
    apiClient.delete(`/practitioners/${practitionerId}/schedules/${scheduleId}/breaks/${breakId}`),
}
