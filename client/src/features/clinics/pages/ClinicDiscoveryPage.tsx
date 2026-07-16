import { useCallback, useEffect, useMemo, useState } from "react"
import { keepPreviousData, useQuery } from "@tanstack/react-query"
import { Filter, List, LocateFixed, Map as MapIcon, Search, SlidersHorizontal, X } from "lucide-react"
import { useSearchParams } from "react-router-dom"
import { clinicApi } from "@shared/api/clinic.api"
import { GoogleClinicMap } from "@shared/components/maps/GoogleClinicMap"
import { PublicHeader } from "@shared/components/navigation/PublicHeader"
import { Pagination } from "@shared/components/data/Pagination"
import { EmptyState } from "@shared/components/feedback/EmptyState"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import { useDebouncedValue } from "@shared/hooks/useDebouncedValue"
import type { ClinicSearchParams } from "@shared/types/clinic.types"
import { ClinicCard } from "../components/ClinicCard"

const PAGE_SIZE = 10

type LocationState = { latitude: number; longitude: number } | null

export default function ClinicDiscoveryPage() {
  const [searchParams, setSearchParams] = useSearchParams()
  const [search, setSearch] = useState(searchParams.get("search") ?? "")
  const [specialization, setSpecialization] = useState(searchParams.get("specialization") ?? "")
  const [selectedClinicId, setSelectedClinicId] = useState<string | null>(null)
  const [mobileView, setMobileView] = useState<"list" | "map">("list")
  const [filtersOpen, setFiltersOpen] = useState(false)
  const [location, setLocation] = useState<LocationState>(null)
  const [locationError, setLocationError] = useState<string | null>(null)
  const [locating, setLocating] = useState(false)

  const debouncedSearch = useDebouncedValue(search)
  const debouncedSpecialization = useDebouncedValue(specialization)
  const page = Math.max(1, Number(searchParams.get("page") ?? 1))
  const sortBy = (searchParams.get("sort") as ClinicSearchParams["sortBy"]) ?? "name"
  const radiusKm = Math.max(1, Number(searchParams.get("radius") ?? 20))

  useEffect(() => {
    setSearchParams((current) => {
      const next = new URLSearchParams(current)
      debouncedSearch ? next.set("search", debouncedSearch) : next.delete("search")
      debouncedSpecialization ? next.set("specialization", debouncedSpecialization) : next.delete("specialization")
      next.delete("page")
      return next
    }, { replace: true })
  }, [debouncedSearch, debouncedSpecialization, setSearchParams])

  const queryParams = useMemo<ClinicSearchParams>(() => ({
    search: searchParams.get("search") || undefined,
    specialization: searchParams.get("specialization") || undefined,
    latitude: location?.latitude,
    longitude: location?.longitude,
    radiusKm: location ? radiusKm : undefined,
    sortBy: location && sortBy === "nearest" ? "nearest" : sortBy,
    page,
    pageSize: PAGE_SIZE,
  }), [location, page, radiusKm, searchParams, sortBy])

  const clinicsQuery = useQuery({
    queryKey: ["clinics", queryParams],
    queryFn: ({ signal }) => clinicApi.search(queryParams, signal).then(({ data }) => data),
    placeholderData: keepPreviousData,
  })

  const clinics = clinicsQuery.data?.items ?? []

  useEffect(() => {
    if (clinics.length === 0) setSelectedClinicId(null)
    else if (!selectedClinicId || !clinics.some((clinic) => clinic.id === selectedClinicId)) setSelectedClinicId(clinics[0].id)
  }, [clinics, selectedClinicId])

  const updateParam = (name: string, value?: string) => {
    setSearchParams((current) => {
      const next = new URLSearchParams(current)
      value ? next.set(name, value) : next.delete(name)
      if (name !== "page") next.delete("page")
      return next
    })
  }

  const selectClinic = useCallback((id: string) => setSelectedClinicId(id), [])

  const useMyLocation = () => {
    if (!navigator.geolocation) {
      setLocationError("Location services are not supported by this browser.")
      return
    }

    setLocating(true)
    setLocationError(null)
    navigator.geolocation.getCurrentPosition(
      (position) => {
        setLocation({ latitude: position.coords.latitude, longitude: position.coords.longitude })
        updateParam("sort", "nearest")
        setLocating(false)
      },
      (error) => {
        setLocationError(error.code === error.PERMISSION_DENIED
          ? "Location permission was denied. You can still search manually."
          : "Your location could not be determined. Please try again or search manually.")
        setLocating(false)
      },
      { enableHighAccuracy: false, timeout: 10_000, maximumAge: 5 * 60_000 },
    )
  }

  const clearFilters = () => {
    setSearch("")
    setSpecialization("")
    setLocation(null)
    setLocationError(null)
    setSearchParams({}, { replace: true })
  }

  const filterPanel = (
    <div className="grid gap-4 lg:grid-cols-[minmax(0,1fr)_220px_180px_auto] lg:items-end">
      <div>
        <Label htmlFor="clinic-search" className="mb-1.5 block text-sm text-gray-700">Clinic, doctor, or specialty</Label>
        <div className="relative">
          <Search className="pointer-events-none absolute left-3 top-1/2 size-5 -translate-y-1/2 text-gray-400" aria-hidden="true" />
          <Input id="clinic-search" value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search clinics or doctors" className="h-12 pl-10" />
        </div>
      </div>
      <div>
        <Label htmlFor="specialization" className="mb-1.5 block text-sm text-gray-700">Specialty</Label>
        <Input id="specialization" value={specialization} onChange={(event) => setSpecialization(event.target.value)} placeholder="e.g. Cardiology" className="h-12" />
      </div>
      <div>
        <Label htmlFor="sort" className="mb-1.5 block text-sm text-gray-700">Sort results</Label>
        <select id="sort" value={sortBy} onChange={(event) => updateParam("sort", event.target.value)} className="h-12 w-full rounded-lg border border-gray-300 bg-white px-3 text-sm focus:border-brand-500">
          <option value="name">Clinic name</option>
          <option value="name-desc">Clinic name Z–A</option>
          <option value="nearest" disabled={!location}>Nearest</option>
        </select>
      </div>
      <Button type="button" variant="outline" className="h-12 px-4" onClick={clearFilters}><X aria-hidden="true" /> Clear</Button>
      {location ? (
        <div className="lg:col-span-4 flex flex-wrap items-center gap-3 rounded-xl bg-brand-50 px-4 py-3">
          <LocateFixed className="size-5 text-brand-700" aria-hidden="true" />
          <span className="text-sm font-Geist-Semibold text-brand-900">Nearby search is active.</span>
          <label className="ml-auto flex items-center gap-2 text-sm text-brand-900">
            Radius
            <select value={radiusKm} onChange={(event) => updateParam("radius", event.target.value)} className="h-9 rounded-lg border border-brand-200 bg-white px-2">
              <option value="5">5 km</option>
              <option value="10">10 km</option>
              <option value="20">20 km</option>
              <option value="50">50 km</option>
            </select>
          </label>
        </div>
      ) : null}
    </div>
  )

  return (
    <div className="min-h-screen bg-surface">
      <PublicHeader />
      <main>
        <section className="border-b border-gray-200 bg-white">
          <div className="mx-auto max-w-[1440px] px-4 py-10 sm:px-6 lg:px-8 lg:py-14">
            <div className="max-w-3xl">
              <p className="text-sm font-Geist-Bold uppercase tracking-[0.18em] text-brand-600">Find care near you</p>
              <h1 className="mt-3 font-Geist-ExtraBold text-3xl tracking-tight text-gray-950 sm:text-5xl">Clinics and healthcare professionals, clearly located.</h1>
              <p className="mt-4 max-w-2xl text-base leading-7 text-gray-600">Search verified clinic locations, compare available practitioners, and move directly into appointment booking.</p>
            </div>
            <div className="mt-7 flex flex-wrap gap-3">
              <Button type="button" className="h-12 bg-brand-600 px-5 text-white hover:bg-brand-700" onClick={useMyLocation} disabled={locating}>
                <LocateFixed aria-hidden="true" /> {locating ? "Finding your location…" : "Use my location"}
              </Button>
              <Button type="button" variant="outline" className="h-12 px-5 lg:hidden" onClick={() => setFiltersOpen((value) => !value)}>
                <Filter aria-hidden="true" /> Filters
              </Button>
            </div>
            {locationError ? <p className="mt-3 text-sm text-amber-800" role="status">{locationError}</p> : null}
          </div>
        </section>

        <div className="mx-auto max-w-[1440px] px-4 py-6 sm:px-6 lg:px-8">
          <section className="hidden rounded-2xl border border-gray-200 bg-white p-5 shadow-sm lg:block" aria-label="Search filters">
            {filterPanel}
          </section>
          {filtersOpen ? <section className="mb-5 rounded-2xl border border-gray-200 bg-white p-5 shadow-sm lg:hidden">{filterPanel}</section> : null}

          <div className="mb-4 flex items-center justify-between gap-4 lg:mt-6">
            <div>
              <p className="text-sm text-gray-600" aria-live="polite">
                {clinicsQuery.data ? <><strong className="text-gray-950">{clinicsQuery.data.totalItems}</strong> clinics found</> : "Searching clinics…"}
              </p>
              {clinicsQuery.isFetching && clinicsQuery.data ? <p className="text-xs text-brand-700">Updating results…</p> : null}
            </div>
            <div className="flex rounded-xl border border-gray-200 bg-white p-1 lg:hidden">
              <button type="button" className={`inline-flex min-h-10 items-center gap-2 rounded-lg px-3 text-sm font-Geist-Semibold ${mobileView === "list" ? "bg-brand-50 text-brand-800" : "text-gray-600"}`} onClick={() => setMobileView("list")}><List className="size-4" /> List</button>
              <button type="button" className={`inline-flex min-h-10 items-center gap-2 rounded-lg px-3 text-sm font-Geist-Semibold ${mobileView === "map" ? "bg-brand-50 text-brand-800" : "text-gray-600"}`} onClick={() => setMobileView("map")}><MapIcon className="size-4" /> Map</button>
            </div>
          </div>

          {clinicsQuery.isError ? (
            <ErrorState title="Clinic search could not be loaded" description="The clinic list is temporarily unavailable. Check your connection and try again." onRetry={() => clinicsQuery.refetch()} />
          ) : clinicsQuery.isPending ? (
            <div className="grid gap-4 lg:grid-cols-2"><ClinicSkeleton /><ClinicSkeleton /><ClinicSkeleton /><ClinicSkeleton /></div>
          ) : clinics.length === 0 ? (
            <EmptyState title="No clinics were found" description="Try a broader specialty, remove nearby filtering, or clear the current search." action={<Button type="button" variant="outline" className="h-11" onClick={clearFilters}><SlidersHorizontal /> Clear filters</Button>} />
          ) : (
            <div className="grid gap-6 lg:grid-cols-[minmax(380px,0.9fr)_minmax(500px,1.1fr)]">
              <section className={`${mobileView === "map" ? "hidden" : "block"} space-y-4 lg:block`} aria-label="Clinic results">
                {clinics.map((clinic) => (
                  <ClinicCard key={clinic.id} clinic={clinic} selected={clinic.id === selectedClinicId} onSelect={() => selectClinic(clinic.id)} />
                ))}
                <Pagination
                  page={clinicsQuery.data.page}
                  totalPages={clinicsQuery.data.totalPages}
                  hasPreviousPage={clinicsQuery.data.hasPreviousPage}
                  hasNextPage={clinicsQuery.data.hasNextPage}
                  isLoading={clinicsQuery.isFetching}
                  onPageChange={(nextPage) => updateParam("page", String(nextPage))}
                />
              </section>
              <section className={`${mobileView === "list" ? "hidden" : "block"} lg:sticky lg:top-22 lg:block lg:h-[calc(100vh-7rem)]`} aria-label="Clinic map">
                <GoogleClinicMap clinics={clinics} selectedClinicId={selectedClinicId} onSelect={selectClinic} userLocation={location} className="h-[70vh] min-h-[440px] lg:h-full" />
                {selectedClinicId ? <p className="sr-only" aria-live="polite">Selected clinic: {clinics.find((clinic) => clinic.id === selectedClinicId)?.name}</p> : null}
              </section>
            </div>
          )}
        </div>
      </main>
    </div>
  )
}

function ClinicSkeleton() {
  return <div className="h-64 animate-pulse rounded-2xl border border-gray-200 bg-white p-5"><div className="h-6 w-2/3 rounded bg-gray-200" /><div className="mt-4 h-4 w-1/2 rounded bg-gray-100" /><div className="mt-8 h-10 rounded bg-gray-100" /><div className="mt-10 h-11 rounded bg-gray-200" /></div>
}
