import { useQuery } from "@tanstack/react-query"
import { CalendarCheck2, CalendarClock, Search, UserRound } from "lucide-react"
import { Link } from "react-router-dom"
import { appointmentApi } from "@shared/api/appointment.api"
import { patientApi } from "@shared/api/patient.api"
import { AppointmentCard } from "@shared/components/appointments/AppointmentCard"
import { StatCard } from "@shared/components/dashboard/StatCard"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"

export default function PatientDashboardPage() {
  const patientQuery = useQuery({ queryKey: ["patient", "me"], queryFn: ({ signal }) => patientApi.getCurrent(signal).then(({ data }) => data) })
  const appointmentsQuery = useQuery({
    queryKey: ["appointments", "patient", patientQuery.data?.id, "overview"],
    queryFn: ({ signal }) => appointmentApi.getByPatient(patientQuery.data!.id, { page: 1, pageSize: 20 }, signal).then(({ data }) => data),
    enabled: Boolean(patientQuery.data?.id),
  })

  if (patientQuery.isError || appointmentsQuery.isError) return <ErrorState title="Your overview could not be loaded" onRetry={() => { patientQuery.refetch(); appointmentsQuery.refetch() }} />

  const now = Date.now()
  const appointments = appointmentsQuery.data?.items ?? []
  const upcoming = appointments.filter((item) => new Date(item.scheduledAt).getTime() > now && item.status !== "Cancelled" && item.status !== "Completed").sort((a, b) => Date.parse(a.scheduledAt) - Date.parse(b.scheduledAt))
  const completed = appointments.filter((item) => item.status === "Completed").length

  return (
    <div className="space-y-7">
      <PageHeader title={patientQuery.data ? `Welcome, ${patientQuery.data.firstName}` : "Patient overview"} description="Find care, keep track of your upcoming visits, and manage your KLINIQ profile." actions={<Button asChild className="h-11 bg-brand-600 px-4 text-white"><Link to="/clinics"><Search /> Find care</Link></Button>} />
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
        <StatCard label="Upcoming" value={upcoming.length} helper="Active appointments in the loaded history" icon={CalendarClock} />
        <StatCard label="Completed visits" value={completed} helper="Appointments marked completed" icon={CalendarCheck2} />
        <StatCard label="Profile status" value={patientQuery.data?.phoneNumber ? "Ready" : "Review"} helper={patientQuery.data?.phoneNumber ? "Contact information is available" : "Add a phone number for easier coordination"} icon={UserRound} />
      </div>
      <section>
        <div className="mb-4 flex items-center justify-between"><div><h2 className="font-Geist-Bold text-xl text-gray-950">Next appointment</h2><p className="mt-1 text-sm text-gray-600">Your nearest active booking</p></div><Link to="/patient/appointments" className="text-sm font-Geist-Semibold text-brand-700 hover:underline">View all</Link></div>
        {patientQuery.isPending || appointmentsQuery.isPending ? <div className="h-44 animate-pulse rounded-2xl bg-white" /> : upcoming[0] ? <AppointmentCard appointment={upcoming[0]} audience="patient" /> : <div className="rounded-2xl border border-dashed border-gray-300 bg-white p-8 text-center"><p className="font-Geist-Bold text-gray-900">No upcoming appointment</p><p className="mt-2 text-sm text-gray-600">Search clinics and choose a live slot when you are ready.</p><Button asChild variant="outline" className="mt-5 h-11"><Link to="/clinics">Browse clinics</Link></Button></div>}
      </section>
    </div>
  )
}
