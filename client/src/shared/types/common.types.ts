export interface PaginatedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalItems: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface ProblemDetails {
  status?: number
  title?: string
  detail?: string
  traceId?: string
  errors?: Record<string, string[]>
  code?: string
  message?: string
}
