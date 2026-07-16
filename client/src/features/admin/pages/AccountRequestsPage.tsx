import { useState } from "react"
import { keepPreviousData, useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { CheckCircle2, Search, XCircle } from "lucide-react"
import { accountRequestApi } from "@shared/api/accountRequest.api"
import { Pagination } from "@shared/components/data/Pagination"
import { EmptyState } from "@shared/components/feedback/EmptyState"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { useDebouncedValue } from "@shared/hooks/useDebouncedValue"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

export default function AccountRequestsPage() {
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState("")
  const [status, setStatus] = useState("")
  const [message, setMessage] = useState<string | null>(null)
  const debouncedSearch = useDebouncedValue(search, 350)
  const queryClient = useQueryClient()
  const query = useQuery({
    queryKey: ["admin", "account-requests", page, debouncedSearch, status],
    queryFn: ({ signal }) => accountRequestApi.getAll({ page, pageSize: 20, search: debouncedSearch || undefined, status: status || undefined }, signal).then(({ data }) => data),
    placeholderData: keepPreviousData,
  })
  const process = useMutation({
    mutationFn: ({ id, action, note }: { id: string; action: "approve" | "reject"; note: string }) => action === "approve" ? accountRequestApi.approve(id, { notes: note || undefined }) : accountRequestApi.reject(id, { reason: note }),
    onSuccess: async () => { setMessage("Application processed."); await queryClient.invalidateQueries({ queryKey: ["admin", "account-requests"] }) },
    onError: (error) => setMessage(getApiErrorMessage(error)),
  })

  const approve = (id: string) => {
    const notes = window.prompt("Optional approval note:", "")
    if (notes !== null) process.mutate({ id, action: "approve", note: notes })
  }
  const reject = (id: string) => {
    const reason = window.prompt("Provide the rejection reason. This is required and will be emailed to the applicant:", "")
    if (reason?.trim()) process.mutate({ id, action: "reject", note: reason.trim() })
  }

  return (
    <div className="space-y-6">
      <PageHeader title="Practitioner applications" description="Review paginated account requests. Approval creates a time-limited invitation; rejection requires a clear reason." />
      {message ? <p className={`rounded-xl px-4 py-3 text-sm ${message.includes("processed") ? "bg-emerald-50 text-emerald-800" : "bg-red-50 text-red-800"}`} role="status">{message}</p> : null}
      <div className="grid gap-3 sm:grid-cols-[minmax(0,1fr)_200px]">
        <div className="relative"><Search className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-gray-400" /><Input value={search} onChange={(event) => { setSearch(event.target.value); setPage(1) }} placeholder="Search applicant, email, clinic, or license" className="h-11 pl-9" /></div>
        <select value={status} onChange={(event) => { setStatus(event.target.value); setPage(1) }} className="h-11 rounded-lg border border-gray-300 bg-white px-3 text-sm" aria-label="Filter application status"><option value="">All statuses</option><option>Pending</option><option>Approved</option><option>Rejected</option></select>
      </div>
      {query.isError ? <ErrorState title="Applications could not be loaded" onRetry={() => query.refetch()} /> : query.isPending ? <div className="h-96 animate-pulse rounded-2xl bg-white" /> : !query.data?.items.length ? <EmptyState title="No applications found" description="No account requests match the selected search and status." /> : <div className="space-y-3">{query.data.items.map((request) => <article key={request.id} className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm"><div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between"><div><div className="flex flex-wrap items-center gap-2"><h2 className="font-Geist-Bold text-lg text-gray-950">{request.firstName} {request.lastName}</h2><span className={`rounded-full px-2.5 py-1 text-xs font-Geist-Semibold ${request.status === "Pending" ? "bg-amber-50 text-amber-800" : request.status === "Approved" ? "bg-emerald-50 text-emerald-800" : "bg-gray-100 text-gray-700"}`}>{request.status}</span></div><p className="mt-2 text-sm text-gray-600">{request.email} · {request.city}, {request.country}</p><div className="mt-3 flex flex-wrap gap-2">{request.specializations.map((item) => <span key={item} className="rounded-lg bg-brand-50 px-2.5 py-1 text-xs text-brand-800">{item}</span>)}</div><p className="mt-3 text-xs text-gray-500">Submitted {new Date(request.createdAtUtc).toLocaleString()}</p></div>{request.status === "Pending" ? <div className="flex shrink-0 flex-wrap gap-2"><Button type="button" className="h-10 bg-brand-600 text-white" disabled={process.isPending} onClick={() => approve(request.id)}><CheckCircle2 /> Approve</Button><Button type="button" variant="destructive" className="h-10" disabled={process.isPending} onClick={() => reject(request.id)}><XCircle /> Reject</Button></div> : null}</div></article>)}</div>}
      {query.data ? <Pagination page={query.data.page} totalPages={query.data.totalPages} hasPreviousPage={query.data.hasPreviousPage} hasNextPage={query.data.hasNextPage} isLoading={query.isFetching} onPageChange={setPage} /> : null}
    </div>
  )
}
