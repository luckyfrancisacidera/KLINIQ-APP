import { useEffect, useState } from "react"
import { keepPreviousData, useQuery } from "@tanstack/react-query"
import { MapPin, Search, Stethoscope } from "lucide-react"
import { Link, useSearchParams } from "react-router-dom"
import { practitionerApi } from "@shared/api/practitioner.api"
import { Pagination } from "@shared/components/data/Pagination"
import { EmptyState } from "@shared/components/feedback/EmptyState"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PublicHeader } from "@shared/components/navigation/PublicHeader"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import { useDebouncedValue } from "@shared/hooks/useDebouncedValue"

export default function FindPractitionerPage() {
  const [params, setParams] = useSearchParams()
  const [search, setSearch] = useState(params.get("search") ?? "")
  const [specialization, setSpecialization] = useState(params.get("specialization") ?? "")
  const debouncedSearch = useDebouncedValue(search)
  const debouncedSpecialization = useDebouncedValue(specialization)
  const page = Math.max(1, Number(params.get("page") ?? 1))

  useEffect(() => {
    setParams((current) => {
      const next = new URLSearchParams(current)
      debouncedSearch ? next.set("search", debouncedSearch) : next.delete("search")
      debouncedSpecialization ? next.set("specialization", debouncedSpecialization) : next.delete("specialization")
      next.delete("page")
      return next
    }, { replace: true })
  }, [debouncedSearch, debouncedSpecialization, setParams])

  const query = useQuery({
    queryKey: ["practitioners", debouncedSearch, debouncedSpecialization, page],
    queryFn: ({ signal }) => practitionerApi.getAll({ search: debouncedSearch || undefined, specialization: debouncedSpecialization || undefined, page, pageSize: 12 }, signal).then(({ data }) => data),
    placeholderData: keepPreviousData,
  })

  const setPage = (nextPage: number) => setParams((current) => {
    const next = new URLSearchParams(current)
    next.set("page", String(nextPage))
    return next
  })

  return (
    <div className="min-h-screen bg-surface">
      <PublicHeader />
      <main>
        <section className="border-b border-gray-200 bg-white">
          <div className="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
            <p className="text-sm font-Geist-Bold uppercase tracking-[0.18em] text-brand-600">Provider directory</p>
            <h1 className="mt-3 font-Geist-ExtraBold text-3xl tracking-tight text-gray-950 sm:text-5xl">Find a healthcare professional.</h1>
            <p className="mt-4 max-w-2xl text-base leading-7 text-gray-600">Search by doctor name, license number, or specialty, then review verified clinic and schedule information.</p>
            <div className="mt-8 grid gap-4 rounded-2xl border border-gray-200 bg-white p-4 shadow-sm sm:grid-cols-2">
              <div>
                <Label htmlFor="doctor-search" className="mb-1.5 block text-sm">Doctor or license</Label>
                <div className="relative"><Search className="absolute left-3 top-1/2 size-5 -translate-y-1/2 text-gray-400" /><Input id="doctor-search" className="h-12 pl-10" value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search doctors" /></div>
              </div>
              <div>
                <Label htmlFor="doctor-specialty" className="mb-1.5 block text-sm">Specialty</Label>
                <Input id="doctor-specialty" className="h-12" value={specialization} onChange={(event) => setSpecialization(event.target.value)} placeholder="e.g. Pediatrics" />
              </div>
            </div>
          </div>
        </section>

        <section className="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
          {query.isError ? <ErrorState onRetry={() => query.refetch()} /> : null}
          {query.isPending ? <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">{Array.from({ length: 6 }).map((_, index) => <div key={index} className="h-60 animate-pulse rounded-2xl border border-gray-200 bg-white" />)}</div> : null}
          {query.data && query.data.items.length === 0 ? <EmptyState title="No providers are currently available" description="Try a different name or a broader specialty." /> : null}
          {query.data && query.data.items.length > 0 ? (
            <>
              <p className="mb-5 text-sm text-gray-600"><strong className="text-gray-950">{query.data.totalItems}</strong> practitioners found</p>
              <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
                {query.data.items.map((practitioner) => (
                  <article key={practitioner.id} className="flex flex-col rounded-2xl border border-gray-200 bg-white p-5 shadow-sm transition hover:border-brand-300 hover:shadow-md">
                    <div className="flex items-start gap-3">
                      <div className="grid size-12 place-items-center rounded-xl bg-brand-50 text-brand-700"><Stethoscope className="size-6" /></div>
                      <div className="min-w-0"><h2 className="font-Geist-Bold text-lg text-gray-950">Dr. {practitioner.firstName} {practitioner.lastName}</h2><p className="mt-1 text-xs text-gray-500">License {practitioner.licenseNumber}</p></div>
                    </div>
                    <div className="mt-4 flex flex-wrap gap-2">{practitioner.specializations.map((item) => <span key={item} className="rounded-full bg-gray-100 px-2.5 py-1 text-xs text-gray-700">{item}</span>)}</div>
                    <p className="mt-5 flex items-start gap-2 text-sm text-gray-600"><MapPin className="mt-0.5 size-4 shrink-0 text-brand-600" /> {practitioner.clinic?.name ?? "Clinic assignment pending"}</p>
                    <Link to={`/practitioners/${practitioner.id}`} className="mt-5 inline-flex min-h-11 items-center justify-center rounded-xl bg-brand-600 px-4 text-sm font-Geist-Semibold text-white hover:bg-brand-700">View profile and slots</Link>
                  </article>
                ))}
              </div>
              <div className="mt-8"><Pagination page={query.data.page} totalPages={query.data.totalPages} hasPreviousPage={query.data.hasPreviousPage} hasNextPage={query.data.hasNextPage} isLoading={query.isFetching} onPageChange={setPage} /></div>
            </>
          ) : null}
        </section>
      </main>
    </div>
  )
}
