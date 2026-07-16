import type { LucideIcon } from "lucide-react"
import { SearchX } from "lucide-react"
import type { ReactNode } from "react"

export function EmptyState({
  title,
  description,
  icon: Icon = SearchX,
  action,
}: {
  title: string
  description: string
  icon?: LucideIcon
  action?: ReactNode
}) {
  return (
    <section className="rounded-2xl border border-dashed border-gray-300 bg-white px-6 py-12 text-center">
      <Icon className="mx-auto size-10 text-gray-400" aria-hidden="true" />
      <h2 className="mt-4 font-Geist-Bold text-lg text-gray-900">{title}</h2>
      <p className="mx-auto mt-2 max-w-md text-sm leading-6 text-gray-600">{description}</p>
      {action ? <div className="mt-5 flex justify-center">{action}</div> : null}
    </section>
  )
}
