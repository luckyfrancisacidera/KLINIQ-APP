import { useMemo, useState } from "react"
import { useQuery } from "@tanstack/react-query"
import { ArrowLeft, CalendarDays, Clock, MapPin, ShieldCheck, Stethoscope } from "lucide-react"
import { Link, useParams } from "react-router-dom"
import { addDays, format } from "date-fns"
import { useAuth } from "@app/providers/AuthProviders"
import { ROLES } from "@app/providers/auth.provider.type"
import { practitionerApi } from "@shared/api/practitioner.api"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { FullPageLoader } from "@shared/components/feedback/FullPageLoader"
import { PublicHeader } from "@shared/components/navigation/PublicHeader"

export default function PractitionerDetailPage() {
  const { id = "" } = useParams()
  const { user } = useAuth()
  const today = useMemo(() => new Date(), [])
  const from = format(today, "yyyy-MM-dd")
  const to = format(addDays(today, 13), "yyyy-MM-dd")
  const [selectedDate, setSelectedDate] = useState<string | null>(null)

  const practitionerQuery = useQuery({ queryKey: ["practitioner", id], queryFn: ({ signal }) => practitionerApi.getById(id, signal).then(({ data }) => data), enabled: Boolean(id) })
  const slotsQuery = useQuery({ queryKey: ["practitioner-slots", id, from, to], queryFn: ({ signal }) => practitionerApi.getAvailableSlots(id, from, to, signal).then(({ data }) => data), enabled: Boolean(id) })

  if (practitionerQuery.isPending) return <><PublicHeader /><FullPageLoader label="Loading practitioner profile…" /></>
  if (practitionerQuery.isError || !practitionerQuery.data) return <><PublicHeader /><main className="mx-auto max-w-4xl p-6"><ErrorState onRetry={() => practitionerQuery.refetch()} /></main></>

  const practitioner = practitionerQuery.data
  const days = slotsQuery.data ?? []
  const activeDate = selectedDate ?? days.find((day) => day.slots.length > 0)?.date ?? days[0]?.date ?? null
  const activeDay = days.find((day) => day.date === activeDate)

  return (
    <div className="min-h-screen bg-surface">
      <PublicHeader />
      <main className="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
        <Link to="/practitioners" className="inline-flex min-h-11 items-center gap-2 text-sm font-Geist-Semibold text-brand-700"><ArrowLeft className="size-4" /> Back to doctors</Link>
        <div className="mt-5 grid gap-6 lg:grid-cols-[0.8fr_1.2fr]">
          <section className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm">
            <div className="grid size-16 place-items-center rounded-2xl bg-brand-50 text-brand-700"><Stethoscope className="size-8" /></div>
            <h1 className="mt-5 font-Geist-ExtraBold text-3xl tracking-tight text-gray-950">Dr. {practitioner.firstName} {practitioner.lastName}</h1>
            <p className="mt-2 flex items-center gap-2 text-sm text-gray-600"><ShieldCheck className="size-4 text-brand-600" /> License {practitioner.licenseNumber}</p>
            <div className="mt-5 flex flex-wrap gap-2">{practitioner.specializations.map((item) => <span key={item} className="rounded-full bg-brand-50 px-3 py-1.5 text-sm text-brand-800">{item}</span>)}</div>
            <div className="mt-7 border-t border-gray-100 pt-5"><p className="flex items-start gap-2 text-sm leading-6 text-gray-600"><MapPin className="mt-1 size-4 shrink-0 text-brand-600" /> {practitioner.clinic?.name ?? "Clinic assignment is not yet available."}</p></div>
          </section>

          <section className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm">
            <div className="flex items-center gap-3"><CalendarDays className="size-6 text-brand-600" /><div><h2 className="font-Geist-Bold text-xl text-gray-950">Available appointments</h2><p className="text-sm text-gray-600">Live slots for the next 14 days</p></div></div>
            {slotsQuery.isPending ? <div className="mt-6 h-52 animate-pulse rounded-xl bg-gray-100" /> : null}
            {slotsQuery.isError ? <div className="mt-6"><ErrorState title="Slots could not be loaded" onRetry={() => slotsQuery.refetch()} /></div> : null}
            {slotsQuery.data && slotsQuery.data.length === 0 ? <p className="mt-6 rounded-xl bg-gray-50 p-5 text-sm text-gray-600">No appointment slots were found.</p> : null}
            {days.length > 0 ? (
              <>
                <div className="mt-6 flex gap-2 overflow-x-auto pb-2" role="tablist" aria-label="Appointment dates">
                  {days.map((day) => (
                    <button key={day.date} type="button" role="tab" aria-selected={day.date === activeDate} onClick={() => setSelectedDate(day.date)} className={`min-h-16 min-w-24 rounded-xl border px-3 py-2 text-center ${day.date === activeDate ? "border-brand-500 bg-brand-50 text-brand-900" : "border-gray-200 text-gray-600 hover:bg-gray-50"}`}>
                      <span className="block text-xs">{day.dayOfWeek.slice(0, 3)}</span><span className="mt-1 block font-Geist-Bold">{format(new Date(`${day.date}T00:00:00`), "MMM d")}</span>
                    </button>
                  ))}
                </div>
                <div className="mt-6">
                  <h3 className="flex items-center gap-2 font-Geist-Bold text-gray-950"><Clock className="size-4 text-brand-600" /> Choose a time</h3>
                  {activeDay && activeDay.slots.length > 0 ? (
                    <div className="mt-3 grid grid-cols-2 gap-2 sm:grid-cols-3 lg:grid-cols-4">
                      {activeDay.slots.map((slot) => {
                        const destination = user?.role === ROLES.PATIENT
                          ? `/patient/book?practitionerId=${id}&scheduleId=${activeDay.scheduleId}&date=${activeDay.date}&time=${encodeURIComponent(slot)}`
                          : `/login`
                        return <Link key={slot} to={destination} state={!user ? { from: `/practitioners/${id}` } : undefined} className="inline-flex min-h-11 items-center justify-center rounded-xl border border-brand-200 bg-white text-sm font-Geist-Semibold text-brand-800 hover:bg-brand-50">{slot}</Link>
                      })}
                    </div>
                  ) : <p className="mt-3 rounded-xl bg-gray-50 p-4 text-sm text-gray-600">No slots are open on this date.</p>}
                </div>
              </>
            ) : null}
          </section>
        </div>
      </main>
    </div>
  )
}
