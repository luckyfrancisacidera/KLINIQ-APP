interface ApiErrorData {
  message?: string
  detail?: string
  title?: string
  code?: string
  errors?: Record<string, string[]>
}

interface ApiError {
  response?: { data?: ApiErrorData }
  message?: string
}

export const getApiErrorMessage = (error: unknown): string => {
  if (error && typeof error === "object") {
    const candidate = error as ApiError
    const data = candidate.response?.data

    if (data?.errors) {
      const first = Object.values(data.errors)[0]
      return Array.isArray(first) && first[0] ? first[0] : "Validation error."
    }

    return data?.message ?? data?.detail ?? data?.title ?? candidate.message ?? "Something went wrong."
  }
  return "Something went wrong. Please try again."
}
