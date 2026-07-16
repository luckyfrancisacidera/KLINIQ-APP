import { useQuery } from "@tanstack/react-query"
import { CalendarCheck2, CalendarClock, Clock3, UsersRound } from "lucide-react"
import { Link } from "react-router-dom"
import { appointmentApi } from "@shared/api/appointment.api"
import { practitionerApi } from "@shared/api/practitioner.api"
import { AppointmentCard } from "@shared/components/appointments/AppointmentCard"
import { StatCard } from "@shared/components/dashboard/StatCard"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"

export default function PractitionerDashboardPage() {
  const practitionerQuery = useQuery({ queryKey: ["practitioner", "me"], queryFn: ({ signal }) => practitionerApi.getCurrent(signal).then(({ data }) => data) })
  const appointmentsQuery = useQuery({
    queryKey: ["appointments", "practitioner", practitionerQuery.data?.id, "overview"],
    queryFn: ({ signal }) => appointmentApi.getByPractitioner(practitionerQuery.data!.id, { page: 1, pageSize: 30 }, signal).then(({ data }) => data),
    enabled: Boolean(practitionerQuery.data?.id),
  })

  if (practitionerQuery.isError || appointmentsQuery.isError) return <ErrorState title="Practitioner overview could not be loaded" onRetry={() => { practitionerQuery.refetch(); appointmentsQuery.refetch() }} />

  const appointments = appointmentsQuery.data?.items ?? []
  const today = new Date().toDateString()
  const todayAppointments = appointments.filter((item) => new Date(item.scheduledAt).toDateString() === today && item.status !== "Cancelled")
  const inQueue = appointments.filter((item) => item.status === "InQueue").length
  const completed = appointments.filter((item) => item.status === "Completed").length
  const next = appointments.filter((item) => Date.parse(item.scheduledAt) > Date.now() && item.status !== "Cancelled" && item.status !== "Completed").sort((a, b) => Date.parse(a.scheduledAt) - Date.parse(b.scheduledAt))[0]

  return (
    <div className="space-y-7">
      <PageHeader title={practitionerQuery.data ? `Welcome, Dr. ${practitionerQuery.data.lastName}` : "Practitioner overview"} description="Manage your availability and process appointments assigned to your practitioner profile." actions={<Button asChild variant="outline" className="h-11"><Link to="/practitioner/schedule"><Clock3 /> Manage availability</Link></Button>} />
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <StatCard label="Today" value={todayAppointments.length} helper="Non-cancelled appointments" icon={CalendarClock} />
        <StatCard label="In queue" value={inQueue} helper="Patients waiting for checkup" icon={UsersRound} />
        <StatCard label="Completed" value={completed} helper="Completed in loaded history" icon={CalendarCheck2} />
        <StatCard label="Weekly schedules" value={practitionerQuery.data?.schedules.length ?? 0} helper="Configured recurring availability" icon={Clock3} />
      </div>
      <section>
        <div className="mb-4 flex items-center justify-between"><div><h2 className="font-Geist-Bold text-xl text-gray-950">Next appointment</h2><p className="mt-1 text-sm text-gray-600">Nearest assigned active booking</p></div><Link className="text-sm font-Geist-Semibold text-brand-700 hover:underline" to="/practitioner/appointments">View all</Link></div>
        {practitionerQuery.isPending || appointmentsQuery.isPending ? <div className="h-44 animate-pulse rounded-2xl bg-white" /> : next ? <AppointmentCard appointment={next} audience="practitioner" /> : <div className="rounded-2xl border border-dashed border-gray-300 bg-white p-8 text-center"><p className="font-Geist-Bold text-gray-900">No upcoming appointment</p><p className="mt-2 text-sm text-gray-600">Your next confirmed or pending booking will appear here.</p></div>}
      </section>
    </div>
  )
}
