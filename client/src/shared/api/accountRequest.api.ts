import apiClient from "./axios"
import type { AccountRequestDto, AccountRequestSummaryDto, ApprovePayload, RejectPayload, SubmitAccountRequestPayload } from "../types/accountRequest.types"
import type { PaginatedResult } from "../types/common.types"

export interface AccountRequestSearchParams {
  page?: number
  pageSize?: number
  search?: string
  status?: string
}

export const accountRequestApi = {
  getAll: (params?: AccountRequestSearchParams, signal?: AbortSignal) =>
    apiClient.get<PaginatedResult<AccountRequestSummaryDto>>("/account-requests", { params, signal }),
  getById: (id: string, signal?: AbortSignal) => apiClient.get<AccountRequestDto>(`/account-requests/${id}`, { signal }),
  submit: (payload: SubmitAccountRequestPayload) => {
    const form = new FormData()
    form.append("firstName", payload.firstName)
    form.append("lastName", payload.lastName)
    form.append("email", payload.email)
    form.append("licenseNumber", payload.licenseNumber)
    payload.specializations.forEach((specialization) => form.append("specializations", specialization))
    form.append("street", payload.street)
    form.append("city", payload.city)
    form.append("country", payload.country)
    form.append("clinicName", payload.clinicName)
    form.append("clinicLatitude", String(payload.clinicLatitude))
    form.append("clinicLongitude", String(payload.clinicLongitude))
    form.append("prcLicense", payload.prcLicense)
    form.append("governmentId", payload.governmentId)
    form.append("professionalPhoto", payload.professionalPhoto)
    form.append("cv", payload.cv)
    return apiClient.post<AccountRequestDto>("/account-requests/submit", form)
  },
  approve: (id: string, payload: ApprovePayload) => apiClient.post(`/account-requests/${id}/approve`, payload),
  reject: (id: string, payload: RejectPayload) => apiClient.post(`/account-requests/${id}/reject`, payload),
}
