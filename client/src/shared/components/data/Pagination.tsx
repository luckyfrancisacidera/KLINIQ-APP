import { ChevronLeft, ChevronRight } from "lucide-react"
import { Button } from "@shared/components/ui/button"

export function Pagination({
  page,
  totalPages,
  hasPreviousPage,
  hasNextPage,
  onPageChange,
  isLoading = false,
}: {
  page: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
  onPageChange: (page: number) => void
  isLoading?: boolean
}) {
  if (totalPages <= 1) return null

  return (
    <nav className="flex items-center justify-between gap-3 border-t border-gray-200 pt-5" aria-label="Pagination">
      <Button
        type="button"
        variant="outline"
        className="h-11 min-w-28"
        disabled={!hasPreviousPage || isLoading}
        onClick={() => onPageChange(page - 1)}
      >
        <ChevronLeft aria-hidden="true" /> Previous
      </Button>
      <p className="text-sm text-gray-600" aria-live="polite">
        Page <span className="font-Geist-Bold text-gray-900">{page}</span> of {totalPages}
      </p>
      <Button
        type="button"
        variant="outline"
        className="h-11 min-w-28"
        disabled={!hasNextPage || isLoading}
        onClick={() => onPageChange(page + 1)}
      >
        Next <ChevronRight aria-hidden="true" />
      </Button>
    </nav>
  )
}
