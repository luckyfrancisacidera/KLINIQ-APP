import { useMemo, useState } from "react"
import { keepPreviousData, useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { addDays, format } from "date-fns"
import { CalendarCheck2, ChevronLeft, Clock3, Stethoscope } from "lucide-react"
import { Link, useNavigate, useSearchParams } from "react-router-dom"
import { appointmentApi } from "@shared/api/appointment.api"
import { patientApi } from "@shared/api/patient.api"
import { practitionerApi } from "@shared/api/practitioner.api"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"
import { Label } from "@shared/components/ui/label"
import { Textarea } from "@shared/components/ui/textarea"
import { useNetworkStatus } from "@shared/hooks/useNetworkStatus"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

export default function BookAppointmentPage() {
  const [params, setParams] = useSearchParams()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const online = useNetworkStatus()
  const practitionerId = params.get("practitionerId") ?? ""
  const appointmentId = params.get("appointmentId")
  const [reason, setReason] = useState("")
  const [message, setMessage] = useState<string | null>(null)
  const from = useMemo(() => format(new Date(), "yyyy-MM-dd"), [])
  const to = useMemo(() => format(addDays(new Date(), 29), "yyyy-MM-dd"), [])

  const practitionerQuery = useQuery({
    queryKey: ["practitioner", practitionerId],
    queryFn: ({ signal }) => practitionerApi.getById(practitionerId, signal).then(({ data }) => data),
    enabled: Boolean(practitionerId),
  })
  const slotsQuery = useQuery({
    queryKey: ["practitioner-slots", practitionerId, from, to],
    queryFn: ({ signal }) => practitionerApi.getAvailableSlots(practitionerId, from, to, signal).then(({ data }) => data),
    enabled: Boolean(practitionerId),
    placeholderData: keepPreviousData,
  })
  const patientQuery = useQuery({
    queryKey: ["patient", "me"],
    queryFn: ({ signal }) => patientApi.getCurrent(signal).then(({ data }) => data),
  })

  const selectedDate = params.get("date") ?? slotsQuery.data?.find((entry) => entry.slots.length > 0)?.date ?? ""
  const selectedDay = slotsQuery.data?.find((entry) => entry.date === selectedDate)
  const selectedTime = params.get("time") ?? ""
  const selectedScheduleId = params.get("scheduleId") ?? selectedDay?.scheduleId ?? ""

  const updateSelection = (key: string, value: string, scheduleId?: string) => {
    setParams((current) => {
      const next = new URLSearchParams(current)
      next.set(key, value)
      if (key === "date") next.delete("time")
      if (scheduleId) next.set("scheduleId", scheduleId)
      return next
    }, { replace: true })
  }

  const mutation = useMutation({
    mutationFn: async () => {
      if (!online) throw new Error("Reconnect to the internet before changing an appointment.")
      if (!selectedScheduleId || !selectedDate || !selectedTime) throw new Error("Choose an available date and time.")
      const payload = { scheduleId: selectedScheduleId, appointmentDate: selectedDate, slotTime: selectedTime }
      return appointmentId
        ? appointmentApi.reschedule(appointmentId, payload)
        : appointmentApi.book({ ...payload, reason: reason.trim() || undefined })
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["appointments"] })
      await queryClient.invalidateQueries({ queryKey: ["practitioner-slots", practitionerId] })
      navigate("/patient/appointments", { replace: true, state: { notice: appointmentId ? "Appointment rescheduled." : "Appointment booked." } })
    },
    onError: (error) => setMessage(getApiErrorMessage(error)),
  })

  if (!practitionerId) {
    return (
      <div className="space-y-6">
        <PageHeader title="Book an appointment" description="Select a clinic or healthcare professional before choosing a live appointment slot." />
        <section className="rounded-2xl border border-gray-200 bg-white p-8 text-center shadow-sm">
          <Stethoscope className="mx-auto size-10 text-brand-600" aria-hidden="true" />
          <h2 className="mt-4 font-Geist-Bold text-xl text-gray-950">Choose who you would like to see</h2>
          <p className="mx-auto mt-2 max-w-lg text-sm leading-6 text-gray-600">Browse nearby clinics or search by healthcare professional and specialty. Available slots are always loaded from the server.</p>
          <div className="mt-6 flex flex-wrap justify-center gap-3">
            <Button asChild className="h-11 bg-brand-600 px-5 text-white"><Link to="/clinics">Find clinics</Link></Button>
            <Button asChild variant="outline" className="h-11 px-5"><Link to="/practitioners">Find practitioners</Link></Button>
          </div>
        </section>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title={appointmentId ? "Reschedule appointment" : "Book an appointment"}
        description="Select an available slot, review the patient information, and confirm. The backend revalidates the slot before saving."
        actions={<Button asChild variant="outline" className="h-11"><Link to={`/practitioners/${practitionerId}`}><ChevronLeft /> Provider profile</Link></Button>}
      />

      {!online ? <p className="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-900" role="status">You are offline. You may review this page, but live availability and appointment submission require an internet connection.</p> : null}

      {practitionerQuery.isError || slotsQuery.isError || patientQuery.isError ? (
        <ErrorState title="Booking information could not be loaded" description="Refresh the availability and try again." onRetry={() => { practitionerQuery.refetch(); slotsQuery.refetch(); patientQuery.refetch() }} />
      ) : (
        <div className="grid gap-6 xl:grid-cols-[1.1fr_0.9fr]">
          <section className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm sm:p-6">
            <h2 className="font-Geist-Bold text-xl text-gray-950">1. Choose a live slot</h2>
            {practitionerQuery.isPending || slotsQuery.isPending ? <div className="mt-5 h-64 animate-pulse rounded-xl bg-gray-100" /> : (
              <>
                <div className="mt-4 rounded-xl bg-brand-50 p-4">
                  <p className="font-Geist-Bold text-brand-950">Dr. {practitionerQuery.data?.firstName} {practitionerQuery.data?.lastName}</p>
                  <p className="mt-1 text-sm text-brand-800">{practitionerQuery.data?.specializations.join(" • ")} · {practitionerQuery.data?.clinic?.name ?? "Clinic not assigned"}</p>
                </div>
                <div className="mt-5 flex gap-2 overflow-x-auto pb-2" role="tablist" aria-label="Available appointment dates">
                  {(slotsQuery.data ?? []).map((entry) => (
                    <button
                      key={entry.date}
                      type="button"
                      role="tab"
                      aria-selected={selectedDate === entry.date}
                      disabled={entry.slots.length === 0}
                      onClick={() => updateSelection("date", entry.date, entry.scheduleId)}
                      className={`min-h-16 min-w-24 rounded-xl border px-3 py-2 text-center disabled:opacity-40 ${selectedDate === entry.date ? "border-brand-500 bg-brand-50 text-brand-900" : "border-gray-200 text-gray-600 hover:bg-gray-50"}`}
                    >
                      <span className="block text-xs">{entry.dayOfWeek.slice(0, 3)}</span>
                      <span className="mt-1 block font-Geist-Bold">{format(new Date(`${entry.date}T00:00:00`), "MMM d")}</span>
                    </button>
                  ))}
                </div>
                <h3 className="mt-5 flex items-center gap-2 font-Geist-Bold text-gray-900"><Clock3 className="size-4 text-brand-600" /> Available times</h3>
                {selectedDay?.slots.length ? (
                  <div className="mt-3 grid grid-cols-2 gap-2 sm:grid-cols-3 md:grid-cols-4">
                    {selectedDay.slots.map((slot) => (
                      <button key={slot} type="button" aria-pressed={selectedTime === slot} onClick={() => updateSelection("time", slot, selectedDay.scheduleId)} className={`min-h-11 rounded-xl border px-3 text-sm font-Geist-Semibold ${selectedTime === slot ? "border-brand-600 bg-brand-600 text-white" : "border-brand-200 text-brand-800 hover:bg-brand-50"}`}>{slot}</button>
                    ))}
                  </div>
                ) : <p className="mt-3 rounded-xl bg-gray-50 p-4 text-sm text-gray-600">Select a date with open slots.</p>}
              </>
            )}
          </section>

          <section className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm sm:p-6">
            <h2 className="font-Geist-Bold text-xl text-gray-950">2. Review and confirm</h2>
            <div className="mt-5 space-y-4">
              <div className="rounded-xl border border-gray-200 p-4">
                <p className="text-xs font-Geist-Semibold uppercase tracking-wide text-gray-500">Patient</p>
                <p className="mt-1 font-Geist-Bold text-gray-950">{patientQuery.data ? `${patientQuery.data.firstName} ${patientQuery.data.lastName}` : "Loading patient profile…"}</p>
                <p className="mt-1 text-sm text-gray-600">{patientQuery.data?.phoneNumber || "No phone number on file"}</p>
              </div>
              <div className="grid gap-3 sm:grid-cols-2">
                <div className="rounded-xl border border-gray-200 p-4"><p className="text-xs text-gray-500">Date</p><p className="mt-1 font-Geist-Bold text-gray-900">{selectedDate ? format(new Date(`${selectedDate}T00:00:00`), "MMMM d, yyyy") : "Not selected"}</p></div>
                <div className="rounded-xl border border-gray-200 p-4"><p className="text-xs text-gray-500">Time</p><p className="mt-1 font-Geist-Bold text-gray-900">{selectedTime || "Not selected"}</p></div>
              </div>
              {!appointmentId ? <div><Label htmlFor="reason" className="mb-2">Reason for visit <span className="font-normal text-gray-500">(optional)</span></Label><Textarea id="reason" value={reason} onChange={(event) => setReason(event.target.value)} maxLength={500} className="min-h-28" placeholder="Briefly describe what you would like help with." /></div> : null}
              {message ? <p className="rounded-xl bg-red-50 px-4 py-3 text-sm text-red-800" role="alert">{message}</p> : null}
              <Button type="button" className="h-12 w-full bg-brand-600 text-white hover:bg-brand-700" disabled={!online || !selectedScheduleId || !selectedDate || !selectedTime || mutation.isPending} onClick={() => mutation.mutate()}>
                <CalendarCheck2 /> {mutation.isPending ? "Confirming availability…" : appointmentId ? "Confirm reschedule" : "Confirm appointment"}
              </Button>
              <p className="text-xs leading-5 text-gray-500">Submitting does not rely on the browser’s cached state. KLINIQ checks the slot again and reports a conflict if another patient has already taken it.</p>
            </div>
          </section>
        </div>
      )}
    </div>
  )
}
