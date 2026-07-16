import { useQuery } from "@tanstack/react-query"
import { ArrowLeft, Building2, MapPin, Navigation, Stethoscope } from "lucide-react"
import { Link, useParams } from "react-router-dom"
import { clinicApi } from "@shared/api/clinic.api"
import { GoogleClinicMap } from "@shared/components/maps/GoogleClinicMap"
import { PublicHeader } from "@shared/components/navigation/PublicHeader"
import { EmptyState } from "@shared/components/feedback/EmptyState"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { FullPageLoader } from "@shared/components/feedback/FullPageLoader"

export default function ClinicDetailPage() {
  const { id = "" } = useParams()
  const query = useQuery({
    queryKey: ["clinic", id],
    queryFn: ({ signal }) => clinicApi.getById(id, signal).then(({ data }) => data),
    enabled: Boolean(id),
  })

  if (query.isPending) return <><PublicHeader /><FullPageLoader label="Loading clinic details…" /></>
  if (query.isError || !query.data) return <><PublicHeader /><main className="mx-auto max-w-4xl p-6"><ErrorState onRetry={() => query.refetch()} /></main></>

  const clinic = query.data
  const mapClinic = [{ ...clinic, distanceKm: null, practitionerCount: clinic.practitioners.length, specializations: [...new Set(clinic.practitioners.flatMap((practitioner) => practitioner.specializations))] }]

  return (
    <div className="min-h-screen bg-surface">
      <PublicHeader />
      <main className="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
        <Link to="/clinics" className="inline-flex min-h-11 items-center gap-2 text-sm font-Geist-Semibold text-brand-700 hover:text-brand-900"><ArrowLeft className="size-4" /> Back to clinic search</Link>
        <div className="mt-5 grid gap-6 lg:grid-cols-[1fr_0.85fr]">
          <section className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm">
            <div className="flex items-start gap-4">
              <div className="grid size-14 shrink-0 place-items-center rounded-2xl bg-brand-50 text-brand-700"><Building2 className="size-7" /></div>
              <div>
                <h1 className="font-Geist-ExtraBold text-3xl tracking-tight text-gray-950">{clinic.name}</h1>
                <p className="mt-2 flex items-center gap-2 text-sm text-gray-600"><MapPin className="size-4 text-brand-600" /> Verified clinic coordinates</p>
              </div>
            </div>
            <a href={`https://www.google.com/maps/dir/?api=1&destination=${clinic.latitude},${clinic.longitude}`} target="_blank" rel="noreferrer" className="mt-6 inline-flex min-h-11 items-center gap-2 rounded-xl border border-gray-200 px-4 text-sm font-Geist-Semibold text-gray-700 hover:bg-gray-50"><Navigation className="size-4" /> Get directions</a>

            <div className="mt-8 border-t border-gray-100 pt-6">
              <h2 className="font-Geist-Bold text-xl text-gray-950">Healthcare professionals</h2>
              <p className="mt-1 text-sm text-gray-600">Choose a practitioner to view availability and book securely.</p>
              {clinic.practitioners.length === 0 ? (
                <div className="mt-5"><EmptyState title="No providers are currently available" description="This clinic does not yet have a public practitioner schedule." /></div>
              ) : (
                <div className="mt-5 grid gap-3">
                  {clinic.practitioners.map((practitioner) => (
                    <article key={practitioner.id} className="flex flex-col gap-4 rounded-xl border border-gray-200 p-4 sm:flex-row sm:items-center sm:justify-between">
                      <div className="flex items-start gap-3">
                        <div className="grid size-11 place-items-center rounded-xl bg-gray-100 text-gray-600"><Stethoscope className="size-5" /></div>
                        <div>
                          <h3 className="font-Geist-Bold text-gray-950">Dr. {practitioner.firstName} {practitioner.lastName}</h3>
                          <p className="mt-1 text-sm text-gray-600">{practitioner.specializations.join(" · ") || "General practice"}</p>
                        </div>
                      </div>
                      <Link to={`/practitioners/${practitioner.id}`} className="inline-flex min-h-11 items-center justify-center rounded-xl bg-brand-600 px-4 text-sm font-Geist-Semibold text-white hover:bg-brand-700">View availability</Link>
                    </article>
                  ))}
                </div>
              )}
            </div>
          </section>
          <GoogleClinicMap clinics={mapClinic} selectedClinicId={clinic.id} onSelect={() => undefined} className="min-h-[480px] lg:sticky lg:top-24" />
        </div>
      </main>
    </div>
  )
}
