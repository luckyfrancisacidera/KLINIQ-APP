interface ApiErrorData {
  message?: string
  detail?: string
  title?: string
  code?: string
  errors?: Record<string, string[]>
}

interface ApiError {
  response?: {
    data?: ApiErrorData
  }
}

export const getApiErrorMessage = (error: unknown): string => {
  if (error && typeof error === "object" && "response" in error) {
    const resp = (error as ApiError).response

    if (resp?.data?.errors) {
      const first = Object.values(resp.data.errors)[0]
      return Array.isArray(first) ? first[0] : "Validation error."
    }

    return (
      resp?.data?.message ??
      resp?.data?.detail ??
      resp?.data?.title ??
      "Something went wrong."
    )
  }
  return "Something went wrong. Please try again."
}