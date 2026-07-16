import type { PaginatedResult } from "./common.types"

export type SymptomUrgency = "Routine" | "Urgent" | "Emergency"

export interface SpecialtySuggestionDto {
  specialty: string
  matchScore: number
  matchedSignals: string[]
}

export interface SuggestedPractitionerDto {
  id: string
  firstName: string
  lastName: string
  licenseNumber: string
  specializations: string[]
  clinicId: string | null
  clinicName: string | null
  matchScore: number
}

export interface SymptomSearchResponseDto {
  urgency: SymptomUrgency
  guidance: string
  suggestedSpecialties: SpecialtySuggestionDto[]
  practitioners: PaginatedResult<SuggestedPractitionerDto>
  disclaimer: string
}

export interface SymptomSearchPayload {
  symptoms: string
  page?: number
  pageSize?: number
}
