import type { LucideIcon } from "lucide-react"

export function StatCard({ label, value, helper, icon: Icon }: { label: string; value: string | number; helper?: string; icon: LucideIcon }) {
  return (
    <article className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
      <div className="flex items-start justify-between gap-4">
        <div>
          <p className="text-sm font-Geist-Semibold text-gray-600">{label}</p>
          <p className="mt-2 font-Geist-ExtraBold text-3xl tracking-tight text-gray-950">{value}</p>
          {helper ? <p className="mt-2 text-xs leading-5 text-gray-500">{helper}</p> : null}
        </div>
        <span className="grid size-11 place-items-center rounded-xl bg-brand-50 text-brand-700"><Icon className="size-5" aria-hidden="true" /></span>
      </div>
    </article>
  )
}
