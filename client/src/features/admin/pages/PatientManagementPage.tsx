import { useState } from "react"
import { keepPreviousData, useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { Search, Trash2 } from "lucide-react"
import { patientApi } from "@shared/api/patient.api"
import { Pagination } from "@shared/components/data/Pagination"
import { EmptyState } from "@shared/components/feedback/EmptyState"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"
import { useDebouncedValue } from "@shared/hooks/useDebouncedValue"

export default function PatientManagementPage() {
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState("")
  const [message, setMessage] = useState<string | null>(null)
  const queryClient = useQueryClient()
  const debouncedSearch = useDebouncedValue(search, 350)
  const query = useQuery({ queryKey: ["admin", "patients", page, debouncedSearch], queryFn: ({ signal }) => patientApi.getAll({ page, pageSize: 20, search: debouncedSearch || undefined }, signal).then(({ data }) => data), placeholderData: keepPreviousData })
  const remove = useMutation({ mutationFn: patientApi.delete, onSuccess: async () => { setMessage("Patient profile removed."); await queryClient.invalidateQueries({ queryKey: ["admin", "patients"] }) }, onError: (error) => setMessage(getApiErrorMessage(error)) })
  const items = query.data?.items ?? []

  return (
    <div className="space-y-6">
      <PageHeader title="Patient management" description="Paginated patient profiles. Destructive actions are restricted to the platform administrator backend role." />
      {message ? <p className={`rounded-xl px-4 py-3 text-sm ${message.includes("removed") ? "bg-emerald-50 text-emerald-800" : "bg-red-50 text-red-800"}`} role="status">{message}</p> : null}
      <div className="relative max-w-md"><Search className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-gray-400" /><Input value={search} onChange={(event) => { setSearch(event.target.value); setPage(1) }} placeholder="Search patients" className="h-11 pl-9" aria-label="Search patients" /></div>
      {query.isError ? <ErrorState title="Patients could not be loaded" onRetry={() => query.refetch()} /> : query.isPending ? <div className="h-96 animate-pulse rounded-2xl bg-white" /> : items.length === 0 ? <EmptyState title="No patients found" description="No patient profiles match this view." /> : <div className="overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-sm"><div className="overflow-x-auto"><table className="w-full min-w-[720px] text-left text-sm"><thead className="bg-gray-50 text-xs uppercase tracking-wide text-gray-500"><tr><th className="px-5 py-4">Patient</th><th className="px-5 py-4">Contact</th><th className="px-5 py-4">Location</th><th className="px-5 py-4">Age</th><th className="px-5 py-4 text-right">Action</th></tr></thead><tbody className="divide-y divide-gray-100">{items.map((patient) => <tr key={patient.id}><td className="px-5 py-4"><p className="font-Geist-Semibold text-gray-950">{patient.firstName} {patient.lastName}</p><p className="mt-1 text-xs text-gray-500">{patient.gender}</p></td><td className="px-5 py-4 text-gray-600">{patient.phoneNumber || "Not provided"}</td><td className="px-5 py-4 text-gray-600">{patient.city}, {patient.country}</td><td className="px-5 py-4 text-gray-600">{patient.age}</td><td className="px-5 py-4 text-right"><Button type="button" variant="destructive" size="icon" className="size-10" aria-label={`Delete ${patient.firstName} ${patient.lastName}`} disabled={remove.isPending} onClick={() => { if (window.confirm("Delete this patient profile? This action may be blocked when related records exist.")) remove.mutate(patient.id) }}><Trash2 /></Button></td></tr>)}</tbody></table></div></div>}
      {query.data ? <Pagination page={query.data.page} totalPages={query.data.totalPages} hasPreviousPage={query.data.hasPreviousPage} hasNextPage={query.data.hasNextPage} isLoading={query.isFetching} onPageChange={setPage} /> : null}
    </div>
  )
}
