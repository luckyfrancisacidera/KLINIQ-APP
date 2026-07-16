import type { AppointmentStatus } from "@shared/types/appointment.types"
import { cn } from "@shared/lib/utils"

const styles: Record<AppointmentStatus, string> = {
  Pending: "bg-amber-50 text-amber-800 ring-amber-200",
  Confirmed: "bg-emerald-50 text-emerald-800 ring-emerald-200",
  InQueue: "bg-violet-50 text-violet-800 ring-violet-200",
  InConsultation: "bg-cyan-50 text-cyan-800 ring-cyan-200",
  Cancelled: "bg-gray-100 text-gray-700 ring-gray-200",
  Completed: "bg-blue-50 text-blue-800 ring-blue-200",
}

const labels: Record<AppointmentStatus, string> = {
  Pending: "Pending",
  Confirmed: "Confirmed",
  InQueue: "In queue",
  InConsultation: "Checkup in progress",
  Cancelled: "Cancelled",
  Completed: "Checkup completed",
}

export function StatusBadge({ status }: { status: AppointmentStatus }) {
  return (
    <span className={cn("inline-flex items-center rounded-full px-2.5 py-1 text-xs font-Geist-Semibold ring-1 ring-inset", styles[status])}>
      {labels[status]}
    </span>
  )
}
