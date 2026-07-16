export interface ClinicSummaryDto {
  id: string
  name: string
  latitude: number
  longitude: number
  distanceKm: number | null
  practitionerCount: number
  specializations: string[]
}

export interface ClinicPractitionerSummaryDto {
  id: string
  firstName: string
  lastName: string
  specializations: string[]
}

export interface ClinicDetailDto {
  id: string
  name: string
  latitude: number
  longitude: number
  practitioners: ClinicPractitionerSummaryDto[]
}

export interface ClinicSearchParams {
  search?: string
  specialization?: string
  latitude?: number
  longitude?: number
  radiusKm?: number
  sortBy?: "name" | "name-desc" | "nearest"
  page?: number
  pageSize?: number
}
