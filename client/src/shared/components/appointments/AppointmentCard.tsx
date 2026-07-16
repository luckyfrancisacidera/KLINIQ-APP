import { CalendarDays, Clock3, FileText, MapPin, Stethoscope, UsersRound } from "lucide-react"
import type { ReactNode } from "react"
import { StatusBadge } from "@shared/components/data/StatusBadge"
import type { AppointmentDto } from "@shared/types/appointment.types"

const dateTime = new Intl.DateTimeFormat(undefined, { dateStyle: "medium", timeStyle: "short" })

function WorkflowProgress({ appointment }: { appointment: AppointmentDto }) {
  const steps = [
    { label: "Confirmed", active: ["Confirmed", "InQueue", "InConsultation", "Completed"].includes(appointment.status) },
    { label: "In queue", active: ["InQueue", "InConsultation", "Completed"].includes(appointment.status) },
    { label: "Checkup", active: ["InConsultation", "Completed"].includes(appointment.status) },
    { label: "Done", active: appointment.status === "Completed" },
  ]

  if (appointment.status === "Pending" || appointment.status === "Cancelled") return null

  return (
    <div className="mt-4" aria-label="Appointment workflow progress">
      <div className="grid grid-cols-4 gap-2">
        {steps.map((step) => (
          <div key={step.label} className="min-w-0">
            <div className={`h-1.5 rounded-full ${step.active ? "bg-brand-600" : "bg-gray-200"}`} />
            <p className={`mt-1.5 truncate text-[11px] ${step.active ? "font-Geist-Semibold text-brand-800" : "text-gray-500"}`}>{step.label}</p>
          </div>
        ))}
      </div>
    </div>
  )
}

export function AppointmentCard({ appointment, actions, audience }: { appointment: AppointmentDto; actions?: ReactNode; audience: "patient" | "practitioner" }) {
  return (
    <article className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2">
            <StatusBadge status={appointment.status} />
            <span className="text-xs text-gray-500">Reference {appointment.id.slice(0, 8).toUpperCase()}</span>
          </div>
          <h2 className="mt-3 font-Geist-Bold text-lg text-gray-950">
            {audience === "patient" ? "Healthcare appointment" : "Patient appointment"}
          </h2>
          <div className="mt-3 grid gap-2 text-sm text-gray-600 sm:grid-cols-2">
            <p className="flex items-center gap-2"><CalendarDays className="size-4 text-brand-600" aria-hidden="true" />{dateTime.format(new Date(appointment.scheduledAt))}</p>
            <p className="flex items-center gap-2"><Clock3 className="size-4 text-brand-600" aria-hidden="true" />{appointment.durationMinutes} minutes</p>
            <p className="flex items-center gap-2"><MapPin className="size-4 text-brand-600" aria-hidden="true" />Clinic {appointment.clinicId.slice(0, 8)}</p>
            <p className="flex items-center gap-2"><FileText className="size-4 text-brand-600" aria-hidden="true" />{appointment.reason || "No reason provided"}</p>
            {appointment.queuedAtUtc ? <p className="flex items-center gap-2"><UsersRound className="size-4 text-violet-600" aria-hidden="true" />Queued {dateTime.format(new Date(appointment.queuedAtUtc))}</p> : null}
            {appointment.consultationStartedAtUtc ? <p className="flex items-center gap-2"><Stethoscope className="size-4 text-cyan-600" aria-hidden="true" />Started {dateTime.format(new Date(appointment.consultationStartedAtUtc))}</p> : null}
          </div>
          <WorkflowProgress appointment={appointment} />
          {appointment.notes ? <p className="mt-3 rounded-xl bg-gray-50 px-3 py-2 text-sm text-gray-600"><strong className="text-gray-800">Clinical note:</strong> {appointment.notes}</p> : null}
        </div>
        {actions ? <div className="flex shrink-0 flex-wrap gap-2">{actions}</div> : null}
      </div>
    </article>
  )
}
