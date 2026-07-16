import { Building2, MapPin, Navigation, Stethoscope } from "lucide-react"
import { Link } from "react-router-dom"
import type { ClinicSummaryDto } from "@shared/types/clinic.types"
import { cn } from "@shared/lib/utils"

export function ClinicCard({ clinic, selected, onSelect }: { clinic: ClinicSummaryDto; selected: boolean; onSelect: () => void }) {
  const directionsUrl = `https://www.google.com/maps/dir/?api=1&destination=${clinic.latitude},${clinic.longitude}`

  return (
    <article className={cn(
      "rounded-2xl border bg-white p-5 shadow-sm transition",
      selected ? "border-brand-500 ring-2 ring-brand-100" : "border-gray-200 hover:border-brand-300 hover:shadow-md",
    )}>
      <button type="button" className="w-full text-left" onClick={onSelect} aria-pressed={selected}>
        <div className="flex items-start gap-4">
          <div className="grid size-12 shrink-0 place-items-center rounded-xl bg-brand-50 text-brand-700">
            <Building2 className="size-6" aria-hidden="true" />
          </div>
          <div className="min-w-0 flex-1">
            <div className="flex flex-wrap items-start justify-between gap-2">
              <h2 className="font-Geist-Bold text-lg text-gray-950">{clinic.name}</h2>
              {clinic.distanceKm !== null ? <span className="rounded-full bg-brand-50 px-2.5 py-1 text-xs font-Geist-Semibold text-brand-800">{clinic.distanceKm} km</span> : null}
            </div>
            <p className="mt-1 flex items-center gap-2 text-sm text-gray-600"><MapPin className="size-4 text-brand-600" aria-hidden="true" /> Verified map location</p>
            <p className="mt-2 flex items-center gap-2 text-sm text-gray-600"><Stethoscope className="size-4 text-gray-400" aria-hidden="true" /> {clinic.practitionerCount} practitioner{clinic.practitionerCount === 1 ? "" : "s"}</p>
          </div>
        </div>
        {clinic.specializations.length > 0 ? (
          <div className="mt-4 flex flex-wrap gap-2">
            {clinic.specializations.slice(0, 4).map((specialization) => <span key={specialization} className="rounded-full bg-gray-100 px-2.5 py-1 text-xs text-gray-700">{specialization}</span>)}
          </div>
        ) : null}
      </button>
      <div className="mt-5 grid grid-cols-2 gap-2 border-t border-gray-100 pt-4">
        <Link to={`/clinics/${clinic.id}`} className="inline-flex min-h-11 items-center justify-center rounded-xl bg-brand-600 px-3 text-sm font-Geist-Semibold text-white hover:bg-brand-700">View clinic</Link>
        <a href={directionsUrl} target="_blank" rel="noreferrer" className="inline-flex min-h-11 items-center justify-center gap-2 rounded-xl border border-gray-200 px-3 text-sm font-Geist-Semibold text-gray-700 hover:bg-gray-50">
          <Navigation className="size-4" aria-hidden="true" /> Directions
        </a>
      </div>
    </article>
  )
}
