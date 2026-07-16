import { Loader2 } from "lucide-react"

export function FullPageLoader({ label = "Loading KLINIQ…" }: { label?: string }) {
  return (
    <div className="flex min-h-[40vh] items-center justify-center p-8" role="status" aria-live="polite">
      <div className="flex items-center gap-3 rounded-xl border border-gray-200 bg-white px-5 py-4 shadow-sm">
        <Loader2 className="size-5 animate-spin text-brand-600" aria-hidden="true" />
        <span className="text-sm font-Geist-Semibold text-gray-700">{label}</span>
      </div>
    </div>
  )
}
