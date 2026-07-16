import { useState } from "react"
import { keepPreviousData, useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { CalendarPlus, ExternalLink, RefreshCcw, XCircle } from "lucide-react"
import { Link, useLocation } from "react-router-dom"
import { appointmentApi } from "@shared/api/appointment.api"
import { patientApi } from "@shared/api/patient.api"
import { AppointmentCard } from "@shared/components/appointments/AppointmentCard"
import { Pagination } from "@shared/components/data/Pagination"
import { EmptyState } from "@shared/components/feedback/EmptyState"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"
import type { AppointmentStatus } from "@shared/types/appointment.types"
import { useNetworkStatus } from "@shared/hooks/useNetworkStatus"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

const filters: Array<{ label: string; value: "All" | AppointmentStatus }> = [
  { label: "All", value: "All" },
  { label: "Upcoming", value: "Confirmed" },
  { label: "Pending", value: "Pending" },
  { label: "In queue", value: "InQueue" },
  { label: "Checkup in progress", value: "InConsultation" },
  { label: "Completed", value: "Completed" },
  { label: "Cancelled", value: "Cancelled" },
]

export default function PatientAppointmentsPage() {
  const [page, setPage] = useState(1)
  const [status, setStatus] = useState<"All" | AppointmentStatus>("All")
  const [actionError, setActionError] = useState<string | null>(null)
  const queryClient = useQueryClient()
  const online = useNetworkStatus()
  const location = useLocation()
  const notice = (location.state as { notice?: string } | null)?.notice

  const patientQuery = useQuery({ queryKey: ["patient", "me"], queryFn: ({ signal }) => patientApi.getCurrent(signal).then(({ data }) => data) })
  const appointmentsQuery = useQuery({
    queryKey: ["appointments", "patient", patientQuery.data?.id, page, status],
    queryFn: ({ signal }) => appointmentApi.getByPatient(patientQuery.data!.id, { page, pageSize: 10, status: status === "All" ? undefined : status }, signal).then(({ data }) => data),
    enabled: Boolean(patientQuery.data?.id),
    placeholderData: keepPreviousData,
  })

  const cancelMutation = useMutation({
    mutationFn: (id: string) => {
      if (!online) throw new Error("Reconnect to the internet before cancelling an appointment.")
      return appointmentApi.cancel(id)
    },
    onSuccess: async () => {
      setActionError(null)
      await queryClient.invalidateQueries({ queryKey: ["appointments", "patient"] })
    },
    onError: (error) => setActionError(getApiErrorMessage(error)),
  })

  const items = appointmentsQuery.data?.items ?? []

  return (
    <div className="space-y-6">
      <PageHeader title="My appointments" description="Review upcoming and past visits, reschedule eligible bookings, or cancel without deleting the appointment history." actions={<Button asChild className="h-11 bg-brand-600 px-4 text-white"><Link to="/patient/book"><CalendarPlus /> Book appointment</Link></Button>} />
      {notice ? <p className="rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-800" role="status">{notice}</p> : null}
      {!online ? <p className="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-900" role="status">Appointment changes are disabled while offline.</p> : null}
      {actionError ? <p className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800" role="alert">{actionError}</p> : null}

      <div className="flex gap-2 overflow-x-auto pb-1" aria-label="Appointment status filter">
        {filters.map((filter) => <button key={filter.value} type="button" aria-pressed={status === filter.value} onClick={() => { setStatus(filter.value); setPage(1) }} className={`min-h-11 shrink-0 rounded-xl px-4 text-sm font-Geist-Semibold ${status === filter.value ? "bg-brand-600 text-white" : "border border-gray-200 bg-white text-gray-600 hover:bg-gray-50"}`}>{filter.label}</button>)}
      </div>

      {patientQuery.isError || appointmentsQuery.isError ? <ErrorState title="Appointments could not be loaded" onRetry={() => { patientQuery.refetch(); appointmentsQuery.refetch() }} /> : null}
      {patientQuery.isPending || (appointmentsQuery.isPending && patientQuery.data) ? <div className="space-y-4">{[1, 2, 3].map((item) => <div key={item} className="h-44 animate-pulse rounded-2xl bg-white" />)}</div> : null}
      {!appointmentsQuery.isPending && !appointmentsQuery.isError && items.length === 0 ? <EmptyState title="No appointments in this view" description={status === "All" ? "You do not have any appointments yet." : `No ${status.toLowerCase()} appointments were found.`} action={<Button asChild variant="outline" className="h-11"><Link to="/clinics">Find a clinic</Link></Button>} /> : null}

      <div className="space-y-4">
        {items.map((appointment) => {
          const canChange = appointment.status === "Pending" || appointment.status === "Confirmed"
          return (
            <AppointmentCard
              key={appointment.id}
              appointment={appointment}
              audience="patient"
              actions={canChange ? <>
                <Button asChild variant="outline" className="h-10"><Link to={`/patient/book?appointmentId=${appointment.id}&practitionerId=${appointment.practitionerId}`}><RefreshCcw /> Reschedule</Link></Button>
                <Button type="button" variant="destructive" className="h-10" disabled={!online || cancelMutation.isPending} onClick={() => { if (window.confirm("Cancel this appointment? The record will remain in your history.")) cancelMutation.mutate(appointment.id) }}><XCircle /> Cancel</Button>
                <Button asChild variant="ghost" className="h-10"><Link to={`/clinics/${appointment.clinicId}`}><ExternalLink /> Clinic</Link></Button>
              </> : undefined}
            />
          )
        })}
      </div>

      {appointmentsQuery.data ? <Pagination page={appointmentsQuery.data.page} totalPages={appointmentsQuery.data.totalPages} hasPreviousPage={appointmentsQuery.data.hasPreviousPage} hasNextPage={appointmentsQuery.data.hasNextPage} isLoading={appointmentsQuery.isFetching} onPageChange={setPage} /> : null}
    </div>
  )
}
