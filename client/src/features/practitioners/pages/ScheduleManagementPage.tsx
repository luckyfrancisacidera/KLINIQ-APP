import { useState, type FormEvent } from "react"
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { Clock3, Plus, Trash2 } from "lucide-react"
import { practitionerApi } from "@shared/api/practitioner.api"
import { EmptyState } from "@shared/components/feedback/EmptyState"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]
const initialForm = { day: "Monday", startTime: "08:00", endTime: "17:00", appointmentLengthMinutes: 30 }

export default function ScheduleManagementPage() {
  const [form, setForm] = useState(initialForm)
  const [message, setMessage] = useState<string | null>(null)
  const queryClient = useQueryClient()
  const practitionerQuery = useQuery({ queryKey: ["practitioner", "me"], queryFn: ({ signal }) => practitionerApi.getCurrent(signal).then(({ data }) => data) })

  const createMutation = useMutation({
    mutationFn: () => practitionerApi.createSchedule(practitionerQuery.data!.id, form),
    onSuccess: async () => { setMessage("Availability added."); await queryClient.invalidateQueries({ queryKey: ["practitioner", "me"] }) },
    onError: (error) => setMessage(getApiErrorMessage(error)),
  })
  const deleteMutation = useMutation({
    mutationFn: (scheduleId: string) => practitionerApi.deleteSchedule(practitionerQuery.data!.id, scheduleId),
    onSuccess: async () => { setMessage("Availability removed."); await queryClient.invalidateQueries({ queryKey: ["practitioner", "me"] }) },
    onError: (error) => setMessage(getApiErrorMessage(error)),
  })

  const submit = (event: FormEvent) => { event.preventDefault(); setMessage(null); createMutation.mutate() }

  if (practitionerQuery.isError) return <ErrorState title="Availability could not be loaded" onRetry={() => practitionerQuery.refetch()} />

  return (
    <div className="space-y-6">
      <PageHeader title="Availability" description="Define recurring weekly schedules used by the backend to calculate live appointment slots and prevent bookings outside your working hours." />
      {message ? <p className={`rounded-xl px-4 py-3 text-sm ${message.endsWith(".") && (message.includes("added") || message.includes("removed")) ? "bg-emerald-50 text-emerald-800" : "bg-red-50 text-red-800"}`} role="status">{message}</p> : null}
      <div className="grid gap-6 xl:grid-cols-[360px_1fr]">
        <form onSubmit={submit} className="h-fit rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
          <div className="flex items-center gap-3"><span className="grid size-10 place-items-center rounded-xl bg-brand-50 text-brand-700"><Plus className="size-5" /></span><div><h2 className="font-Geist-Bold text-lg text-gray-950">Add weekly hours</h2><p className="text-xs text-gray-500">Overlapping schedules are rejected.</p></div></div>
          <div className="mt-5 space-y-4">
            <div><Label htmlFor="day" className="mb-2">Day</Label><select id="day" value={form.day} onChange={(event) => setForm((current) => ({ ...current, day: event.target.value }))} className="h-11 w-full rounded-lg border border-gray-300 bg-white px-3 text-sm">{days.map((day) => <option key={day}>{day}</option>)}</select></div>
            <div className="grid grid-cols-2 gap-3"><div><Label htmlFor="start" className="mb-2">Start</Label><Input id="start" type="time" value={form.startTime} onChange={(event) => setForm((current) => ({ ...current, startTime: event.target.value }))} className="h-11" required /></div><div><Label htmlFor="end" className="mb-2">End</Label><Input id="end" type="time" value={form.endTime} onChange={(event) => setForm((current) => ({ ...current, endTime: event.target.value }))} className="h-11" required /></div></div>
            <div><Label htmlFor="duration" className="mb-2">Appointment duration</Label><select id="duration" value={form.appointmentLengthMinutes} onChange={(event) => setForm((current) => ({ ...current, appointmentLengthMinutes: Number(event.target.value) }))} className="h-11 w-full rounded-lg border border-gray-300 bg-white px-3 text-sm"><option value={15}>15 minutes</option><option value={20}>20 minutes</option><option value={30}>30 minutes</option><option value={45}>45 minutes</option><option value={60}>60 minutes</option></select></div>
            <Button type="submit" className="h-11 w-full bg-brand-600 text-white" disabled={!practitionerQuery.data || createMutation.isPending}><Plus /> {createMutation.isPending ? "Adding…" : "Add availability"}</Button>
          </div>
        </form>

        <section>
          <h2 className="font-Geist-Bold text-xl text-gray-950">Weekly schedule</h2><p className="mt-1 text-sm text-gray-600">Patients see generated slots only when a schedule is available.</p>
          {practitionerQuery.isPending ? <div className="mt-4 h-72 animate-pulse rounded-2xl bg-white" /> : practitionerQuery.data?.schedules.length ? <div className="mt-4 grid gap-3 sm:grid-cols-2">{[...practitionerQuery.data.schedules].sort((a, b) => days.indexOf(a.day) - days.indexOf(b.day)).map((schedule) => <article key={schedule.id} className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
            <div className="flex items-start justify-between gap-3"><div><p className="font-Geist-Bold text-lg text-gray-950">{schedule.day}</p><p className="mt-2 flex items-center gap-2 text-sm text-gray-600"><Clock3 className="size-4 text-brand-600" /> {schedule.startTime} – {schedule.endTime}</p><p className="mt-1 text-xs text-gray-500">{schedule.appointmentDurationMinutes}-minute slots · {schedule.isAvailable ? "Available" : "Unavailable"}</p></div><Button type="button" variant="destructive" size="icon" className="size-10" aria-label={`Delete ${schedule.day} schedule`} disabled={deleteMutation.isPending} onClick={() => { if (window.confirm(`Remove the ${schedule.day} schedule?`)) deleteMutation.mutate(schedule.id) }}><Trash2 /></Button></div>
            {schedule.breaks.length ? <div className="mt-4 border-t border-gray-100 pt-3"><p className="text-xs font-Geist-Semibold uppercase tracking-wide text-gray-500">Breaks</p><div className="mt-2 flex flex-wrap gap-2">{schedule.breaks.map((item) => <span key={item.id} className="rounded-lg bg-gray-100 px-2.5 py-1 text-xs text-gray-700">{item.startTime}–{item.endTime}</span>)}</div></div> : null}
          </article>)}</div> : <div className="mt-4"><EmptyState title="No weekly availability" description="Add your first recurring schedule to begin publishing live appointment slots." icon={Clock3} /></div>}
        </section>
      </div>
    </div>
  )
}
