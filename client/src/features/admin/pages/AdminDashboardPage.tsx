import { useQuery } from "@tanstack/react-query"
import { ClipboardList, Stethoscope, UserRoundCheck, Users } from "lucide-react"
import { Link } from "react-router-dom"
import { accountRequestApi } from "@shared/api/accountRequest.api"
import { patientApi } from "@shared/api/patient.api"
import { practitionerApi } from "@shared/api/practitioner.api"
import { StatCard } from "@shared/components/dashboard/StatCard"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"

export default function AdminDashboardPage() {
  const patients = useQuery({ queryKey: ["admin", "patients", "count"], queryFn: ({ signal }) => patientApi.getAll(1, 1, signal).then(({ data }) => data) })
  const practitioners = useQuery({ queryKey: ["admin", "practitioners", "count"], queryFn: ({ signal }) => practitionerApi.getAll({ page: 1, pageSize: 1 }, signal).then(({ data }) => data) })
  const requests = useQuery({ queryKey: ["admin", "account-requests", "count"], queryFn: ({ signal }) => accountRequestApi.getAll({ page: 1, pageSize: 5 }, signal).then(({ data }) => data) })

  if (patients.isError || practitioners.isError || requests.isError) return <ErrorState title="Administrative overview could not be loaded" onRetry={() => { patients.refetch(); practitioners.refetch(); requests.refetch() }} />
  const pending = requests.data?.items.filter((request) => request.status === "Pending").length ?? 0

  return (
    <div className="space-y-7">
      <PageHeader title="Platform overview" description="Review KLINIQ account activity and manage the verified patient and practitioner directories." actions={<Button asChild variant="outline" className="h-11"><Link to="/admin/account-requests"><ClipboardList /> Review applications</Link></Button>} />
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <StatCard label="Patients" value={patients.data?.totalItems ?? "—"} helper="Registered patient profiles" icon={Users} />
        <StatCard label="Practitioners" value={practitioners.data?.totalItems ?? "—"} helper="Verified practitioner profiles" icon={Stethoscope} />
        <StatCard label="Applications" value={requests.data?.totalItems ?? "—"} helper="All practitioner account requests" icon={ClipboardList} />
        <StatCard label="Pending in preview" value={pending} helper="Pending applications among latest five" icon={UserRoundCheck} />
      </div>
      <section className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm sm:p-6">
        <div className="flex items-center justify-between gap-4"><div><h2 className="font-Geist-Bold text-xl text-gray-950">Recent practitioner applications</h2><p className="mt-1 text-sm text-gray-600">Latest submitted requests, ordered by creation date</p></div><Link to="/admin/account-requests" className="text-sm font-Geist-Semibold text-brand-700 hover:underline">Open queue</Link></div>
        <div className="mt-5 divide-y divide-gray-100">{requests.isPending ? <div className="h-48 animate-pulse rounded-xl bg-gray-100" /> : requests.data?.items.length ? requests.data.items.map((request) => <div key={request.id} className="flex flex-col gap-2 py-4 first:pt-0 sm:flex-row sm:items-center sm:justify-between"><div><p className="font-Geist-Semibold text-gray-950">{request.firstName} {request.lastName}</p><p className="mt-1 text-sm text-gray-600">{request.email} · {request.specializations.join(", ")}</p></div><span className={`w-fit rounded-full px-2.5 py-1 text-xs font-Geist-Semibold ${request.status === "Pending" ? "bg-amber-50 text-amber-800" : request.status === "Approved" ? "bg-emerald-50 text-emerald-800" : "bg-gray-100 text-gray-700"}`}>{request.status}</span></div>) : <p className="py-8 text-center text-sm text-gray-600">No applications have been submitted.</p>}</div>
      </section>
    </div>
  )
}
