import { useState } from "react"
import { keepPreviousData, useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { Search, Trash2 } from "lucide-react"
import { practitionerApi } from "@shared/api/practitioner.api"
import { Pagination } from "@shared/components/data/Pagination"
import { EmptyState } from "@shared/components/feedback/EmptyState"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { useDebouncedValue } from "@shared/hooks/useDebouncedValue"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

export default function PractitionerManagementPage() {
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState("")
  const [message, setMessage] = useState<string | null>(null)
  const debouncedSearch = useDebouncedValue(search, 350)
  const queryClient = useQueryClient()
  const query = useQuery({
    queryKey: ["admin", "practitioners", page, debouncedSearch],
    queryFn: ({ signal }) => practitionerApi.getAll({ page, pageSize: 20, search: debouncedSearch || undefined }, signal).then(({ data }) => data),
    placeholderData: keepPreviousData,
  })
  const remove = useMutation({ mutationFn: practitionerApi.delete, onSuccess: async () => { setMessage("Practitioner profile removed."); await queryClient.invalidateQueries({ queryKey: ["admin", "practitioners"] }) }, onError: (error) => setMessage(getApiErrorMessage(error)) })

  return (
    <div className="space-y-6">
      <PageHeader title="Practitioner management" description="Search and manage verified practitioner profiles. Public search remains available separately from this administrative workspace." />
      {message ? <p className={`rounded-xl px-4 py-3 text-sm ${message.includes("removed") ? "bg-emerald-50 text-emerald-800" : "bg-red-50 text-red-800"}`} role="status">{message}</p> : null}
      <div className="relative max-w-md"><Search className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-gray-400" /><Input value={search} onChange={(event) => { setSearch(event.target.value); setPage(1) }} placeholder="Search name or specialty" className="h-11 pl-9" /></div>
      {query.isError ? <ErrorState title="Practitioners could not be loaded" onRetry={() => query.refetch()} /> : query.isPending ? <div className="h-96 animate-pulse rounded-2xl bg-white" /> : !query.data?.items.length ? <EmptyState title="No practitioners found" description="Try a broader search term." /> : <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">{query.data.items.map((practitioner) => <article key={practitioner.id} className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm"><div className="flex items-start justify-between gap-3"><div><h2 className="font-Geist-Bold text-lg text-gray-950">Dr. {practitioner.firstName} {practitioner.lastName}</h2><p className="mt-1 text-xs text-gray-500">License {practitioner.licenseNumber}</p></div><Button type="button" variant="destructive" size="icon" className="size-10" aria-label={`Delete Dr. ${practitioner.lastName}`} disabled={remove.isPending} onClick={() => { if (window.confirm("Delete this practitioner profile? Related appointments may prevent deletion.")) remove.mutate(practitioner.id) }}><Trash2 /></Button></div><div className="mt-4 flex flex-wrap gap-2">{practitioner.specializations.map((item) => <span key={item} className="rounded-full bg-brand-50 px-2.5 py-1 text-xs text-brand-800">{item}</span>)}</div><p className="mt-4 border-t border-gray-100 pt-4 text-sm text-gray-600">{practitioner.clinic?.name ?? "No clinic assigned"}</p></article>)}</div>}
      {query.data ? <Pagination page={query.data.page} totalPages={query.data.totalPages} hasPreviousPage={query.data.hasPreviousPage} hasNextPage={query.data.hasNextPage} isLoading={query.isFetching} onPageChange={setPage} /> : null}
    </div>
  )
}
