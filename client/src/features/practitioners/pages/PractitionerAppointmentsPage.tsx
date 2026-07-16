import { useState } from "react"
import { keepPreviousData, useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { CheckCircle2, ClipboardCheck, PlayCircle, UsersRound, XCircle } from "lucide-react"
import { appointmentApi } from "@shared/api/appointment.api"
import { practitionerApi } from "@shared/api/practitioner.api"
import { AppointmentCard } from "@shared/components/appointments/AppointmentCard"
import { Pagination } from "@shared/components/data/Pagination"
import { EmptyState } from "@shared/components/feedback/EmptyState"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@shared/components/ui/dialog"
import { Label } from "@shared/components/ui/label"
import { Textarea } from "@shared/components/ui/textarea"
import type { AppointmentStatus } from "@shared/types/appointment.types"
import { useNetworkStatus } from "@shared/hooks/useNetworkStatus"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

const statuses: Array<"All" | AppointmentStatus> = ["All", "Pending", "Confirmed", "InQueue", "InConsultation", "Completed", "Cancelled"]

const statusLabel = (status: "All" | AppointmentStatus) => {
  if (status === "InQueue") return "In queue"
  if (status === "InConsultation") return "Checkup in progress"
  return status
}

type AppointmentAction = "confirm" | "queue" | "start" | "cancel" | "complete"

export default function PractitionerAppointmentsPage() {
  const [page, setPage] = useState(1)
  const [status, setStatus] = useState<"All" | AppointmentStatus>("All")
  const [message, setMessage] = useState<string | null>(null)
  const [completionId, setCompletionId] = useState<string | null>(null)
  const [completionNotes, setCompletionNotes] = useState("")
  const queryClient = useQueryClient()
  const online = useNetworkStatus()
  const practitionerQuery = useQuery({ queryKey: ["practitioner", "me"], queryFn: ({ signal }) => practitionerApi.getCurrent(signal).then(({ data }) => data) })
  const appointmentsQuery = useQuery({
    queryKey: ["appointments", "practitioner", practitionerQuery.data?.id, page, status],
    queryFn: ({ signal }) => appointmentApi.getByPractitioner(practitionerQuery.data!.id, { page, pageSize: 10, status: status === "All" ? undefined : status }, signal).then(({ data }) => data),
    enabled: Boolean(practitionerQuery.data?.id),
    placeholderData: keepPreviousData,
  })

  const action = useMutation({
    mutationFn: ({ id, type, notes }: { id: string; type: AppointmentAction; notes?: string }) => {
      if (!online) throw new Error("Reconnect to the internet before updating an appointment.")
      if (type === "confirm") return appointmentApi.confirm(id)
      if (type === "queue") return appointmentApi.queue(id)
      if (type === "start") return appointmentApi.startConsultation(id)
      if (type === "complete") return appointmentApi.complete(id, notes)
      return appointmentApi.cancel(id)
    },
    onSuccess: async (_, variables) => {
      const messages: Record<AppointmentAction, string> = {
        confirm: "Appointment confirmed.",
        queue: "Patient added to the clinic queue.",
        start: "Checkup started.",
        complete: "Checkup completed.",
        cancel: "Appointment cancelled.",
      }
      setMessage(messages[variables.type])
      if (variables.type === "complete") {
        setCompletionId(null)
        setCompletionNotes("")
      }
      await queryClient.invalidateQueries({ queryKey: ["appointments", "practitioner"] })
    },
    onError: (error) => setMessage(getApiErrorMessage(error)),
  })

  const complete = () => {
    if (!completionId) return
    action.mutate({ id: completionId, type: "complete", notes: completionNotes.trim() || undefined })
  }

  const items = appointmentsQuery.data?.items ?? []

  return (
    <div className="space-y-6">
      <PageHeader title="Appointment and clinic queue" description="Move each visit through confirmation, queueing, active checkup, and completion. Every transition is enforced by the backend." />
      {!online ? <p className="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-900" role="status">Appointment processing is disabled while offline.</p> : null}
      {message ? <p className={`rounded-xl px-4 py-3 text-sm ${action.isError ? "bg-red-50 text-red-800" : "bg-emerald-50 text-emerald-800"}`} role="status">{message}</p> : null}
      <div className="flex gap-2 overflow-x-auto pb-1" aria-label="Appointment status filter">{statuses.map((item) => <button key={item} type="button" aria-pressed={status === item} onClick={() => { setStatus(item); setPage(1) }} className={`min-h-11 shrink-0 rounded-xl px-4 text-sm font-Geist-Semibold ${status === item ? "bg-brand-600 text-white" : "border border-gray-200 bg-white text-gray-600"}`}>{statusLabel(item)}</button>)}</div>
      {practitionerQuery.isError || appointmentsQuery.isError ? <ErrorState title="Appointments could not be loaded" onRetry={() => { practitionerQuery.refetch(); appointmentsQuery.refetch() }} /> : null}
      {practitionerQuery.isPending || (appointmentsQuery.isPending && practitionerQuery.data) ? <div className="space-y-4">{[1, 2, 3].map((item) => <div key={item} className="h-44 animate-pulse rounded-2xl bg-white" />)}</div> : null}
      {!appointmentsQuery.isPending && !appointmentsQuery.isError && items.length === 0 ? <EmptyState title="No appointments in this view" description={`No ${status === "All" ? "assigned" : statusLabel(status).toLowerCase()} appointments were found.`} /> : null}
      <div className="space-y-4">{items.map((appointment) => <AppointmentCard key={appointment.id} appointment={appointment} audience="practitioner" actions={<>
        {appointment.status === "Pending" ? <Button type="button" className="h-10 bg-brand-600 text-white" disabled={!online || action.isPending} onClick={() => action.mutate({ id: appointment.id, type: "confirm" })}><CheckCircle2 /> Confirm</Button> : null}
        {appointment.status === "Confirmed" ? <Button type="button" className="h-10 bg-violet-600 text-white hover:bg-violet-700" disabled={!online || action.isPending} onClick={() => action.mutate({ id: appointment.id, type: "queue" })}><UsersRound /> Add to queue</Button> : null}
        {appointment.status === "InQueue" ? <Button type="button" className="h-10 bg-cyan-600 text-white hover:bg-cyan-700" disabled={!online || action.isPending} onClick={() => action.mutate({ id: appointment.id, type: "start" })}><PlayCircle /> Start checkup</Button> : null}
        {appointment.status === "InConsultation" ? <Button type="button" className="h-10 bg-brand-600 text-white" disabled={!online || action.isPending} onClick={() => setCompletionId(appointment.id)}><ClipboardCheck /> Finish checkup</Button> : null}
        {(["Pending", "Confirmed", "InQueue"] as AppointmentStatus[]).includes(appointment.status) ? <Button type="button" variant="destructive" className="h-10" disabled={!online || action.isPending} onClick={() => { if (window.confirm("Cancel this appointment?")) action.mutate({ id: appointment.id, type: "cancel" }) }}><XCircle /> Cancel</Button> : null}
      </>} />)}</div>
      {appointmentsQuery.data ? <Pagination page={appointmentsQuery.data.page} totalPages={appointmentsQuery.data.totalPages} hasPreviousPage={appointmentsQuery.data.hasPreviousPage} hasNextPage={appointmentsQuery.data.hasNextPage} isLoading={appointmentsQuery.isFetching} onPageChange={setPage} /> : null}

      <Dialog open={Boolean(completionId)} onOpenChange={(open) => { if (!open && !action.isPending) { setCompletionId(null); setCompletionNotes("") } }}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader>
            <DialogTitle>Finish patient checkup</DialogTitle>
            <DialogDescription>Completing the checkup closes the active consultation. Add only the minimum appropriate clinical note for this appointment.</DialogDescription>
          </DialogHeader>
          <div>
            <Label htmlFor="completion-notes" className="mb-2 block">Clinical notes (optional)</Label>
            <Textarea id="completion-notes" className="min-h-32 resize-y" maxLength={1000} value={completionNotes} onChange={(event) => setCompletionNotes(event.target.value)} placeholder="Brief follow-up or visit summary" />
            <p className="mt-2 text-right text-xs text-gray-500">{completionNotes.length}/1000</p>
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" disabled={action.isPending} onClick={() => { setCompletionId(null); setCompletionNotes("") }}>Keep checkup open</Button>
            <Button type="button" className="bg-brand-600 text-white" disabled={!online || action.isPending} onClick={complete}>{action.isPending ? "Finishing…" : "Finish checkup"}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
