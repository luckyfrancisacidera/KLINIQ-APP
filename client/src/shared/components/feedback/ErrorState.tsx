import { AlertTriangle, RotateCcw } from "lucide-react"
import { Button } from "@shared/components/ui/button"

export function ErrorState({
  title = "Something went wrong",
  description = "KLINIQ could not load this information. Please try again.",
  onRetry,
}: {
  title?: string
  description?: string
  onRetry?: () => void
}) {
  return (
    <section className="rounded-2xl border border-red-200 bg-red-50 px-6 py-10 text-center" role="alert">
      <AlertTriangle className="mx-auto size-9 text-red-600" aria-hidden="true" />
      <h2 className="mt-3 font-Geist-Bold text-lg text-red-950">{title}</h2>
      <p className="mx-auto mt-2 max-w-md text-sm leading-6 text-red-800">{description}</p>
      {onRetry ? (
        <Button type="button" variant="outline" className="mt-5 h-10 border-red-300 bg-white" onClick={onRetry}>
          <RotateCcw aria-hidden="true" /> Retry
        </Button>
      ) : null}
    </section>
  )
}
