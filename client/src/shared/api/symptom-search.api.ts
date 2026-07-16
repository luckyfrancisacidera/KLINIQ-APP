import apiClient from "./axios"
import type { SymptomSearchPayload, SymptomSearchResponseDto } from "../types/symptom-search.types"

export const symptomSearchApi = {
  search: (payload: SymptomSearchPayload) => apiClient.post<SymptomSearchResponseDto>("/symptom-search", payload),
}
