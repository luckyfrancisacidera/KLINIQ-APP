import apiClient from "./axios"
import type { PaginatedResult } from "../types/common.types"
import type { ClinicDetailDto, ClinicSearchParams, ClinicSummaryDto } from "../types/clinic.types"

export const clinicApi = {
  search: (params: ClinicSearchParams, signal?: AbortSignal) =>
    apiClient.get<PaginatedResult<ClinicSummaryDto>>("/clinics", { params, signal }),
  getById: (id: string, signal?: AbortSignal) => apiClient.get<ClinicDetailDto>(`/clinics/${id}`, { signal }),
}
