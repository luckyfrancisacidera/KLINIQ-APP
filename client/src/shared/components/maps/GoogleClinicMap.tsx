import { useEffect, useRef, useState } from "react"
import { MapPin, Navigation } from "lucide-react"
import env from "@shared/config/env"
import type { ClinicSummaryDto } from "@shared/types/clinic.types"
import { cn } from "@shared/lib/utils"

let mapsPromise: Promise<void> | null = null

function loadGoogleMaps(apiKey: string) {
  if (window.google?.maps) return Promise.resolve()
  if (mapsPromise) return mapsPromise

  mapsPromise = new Promise<void>((resolve, reject) => {
    const existing = document.querySelector<HTMLScriptElement>('script[data-kliniq-google-maps="true"]')
    if (existing) {
      existing.addEventListener("load", () => resolve(), { once: true })
      existing.addEventListener("error", () => reject(new Error("Map service could not be loaded.")), { once: true })
      return
    }

    const script = document.createElement("script")
    script.src = `https://maps.googleapis.com/maps/api/js?key=${encodeURIComponent(apiKey)}&v=weekly`
    script.async = true
    script.defer = true
    script.dataset.kliniqGoogleMaps = "true"
    script.onload = () => resolve()
    script.onerror = () => reject(new Error("Map service could not be loaded."))
    document.head.appendChild(script)
  })

  return mapsPromise
}

export function GoogleClinicMap({
  clinics,
  selectedClinicId,
  onSelect,
  userLocation,
  className,
}: {
  clinics: ClinicSummaryDto[]
  selectedClinicId: string | null
  onSelect: (id: string) => void
  userLocation?: { latitude: number; longitude: number } | null
  className?: string
}) {
  const containerRef = useRef<HTMLDivElement | null>(null)
  const mapRef = useRef<GoogleMap | null>(null)
  const markersRef = useRef<GoogleMarker[]>([])
  const [error, setError] = useState<string | null>(null)
  const [ready, setReady] = useState(false)

  useEffect(() => {
    if (!env.googleMapsApiKey) {
      setError("Map preview needs a Google Maps browser key. Clinic results remain fully available in the list.")
      return
    }

    let cancelled = false
    loadGoogleMaps(env.googleMapsApiKey)
      .then(() => {
        if (cancelled || !containerRef.current || !window.google?.maps) return
        mapRef.current = new window.google.maps.Map(containerRef.current, {
          center: { lat: 18.198, lng: 120.593 },
          zoom: 12,
          mapTypeControl: false,
          streetViewControl: false,
          fullscreenControl: true,
          gestureHandling: "greedy",
          clickableIcons: false,
        })
        setReady(true)
      })
      .catch((reason: unknown) => setError(reason instanceof Error ? reason.message : "Map service could not be loaded."))

    return () => {
      cancelled = true
      markersRef.current.forEach((marker) => marker.setMap(null))
      markersRef.current = []
      mapRef.current = null
    }
  }, [])

  useEffect(() => {
    const maps = window.google?.maps
    const map = mapRef.current
    if (!ready || !maps || !map) return

    markersRef.current.forEach((marker) => marker.setMap(null))
    markersRef.current = []
    const bounds = new maps.LatLngBounds()

    clinics.forEach((clinic) => {
      if (!Number.isFinite(clinic.latitude) || !Number.isFinite(clinic.longitude)) return
      const marker = new maps.Marker({
        map,
        position: { lat: clinic.latitude, lng: clinic.longitude },
        title: clinic.name,
        label: clinic.id === selectedClinicId ? "●" : undefined,
        zIndex: clinic.id === selectedClinicId ? 10 : 1,
      })
      marker.addListener("click", () => onSelect(clinic.id))
      markersRef.current.push(marker)
      bounds.extend({ lat: clinic.latitude, lng: clinic.longitude })
    })

    if (userLocation) bounds.extend({ lat: userLocation.latitude, lng: userLocation.longitude })
    if (!bounds.isEmpty()) map.fitBounds(bounds, 48)
  }, [clinics, onSelect, ready, selectedClinicId, userLocation])

  useEffect(() => {
    const selected = clinics.find((clinic) => clinic.id === selectedClinicId)
    if (selected && mapRef.current) {
      mapRef.current.setCenter({ lat: selected.latitude, lng: selected.longitude })
      mapRef.current.setZoom(15)
    }
  }, [clinics, selectedClinicId])

  if (error) {
    return (
      <div className={cn("flex min-h-[360px] items-center justify-center rounded-2xl border border-gray-200 bg-gray-50 p-8", className)}>
        <div className="max-w-sm text-center">
          <MapPin className="mx-auto size-10 text-brand-600" aria-hidden="true" />
          <h2 className="mt-4 font-Geist-Bold text-lg text-gray-950">Clinic map unavailable</h2>
          <p className="mt-2 text-sm leading-6 text-gray-600">{error}</p>
          {selectedClinicId ? (
            <a
              className="mt-5 inline-flex min-h-11 items-center gap-2 rounded-xl bg-brand-600 px-4 text-sm font-Geist-Semibold text-white hover:bg-brand-700"
              href={`https://www.google.com/maps/search/?api=1&query=${clinics.find((clinic) => clinic.id === selectedClinicId)?.latitude},${clinics.find((clinic) => clinic.id === selectedClinicId)?.longitude}`}
              target="_blank"
              rel="noreferrer"
            >
              <Navigation className="size-4" aria-hidden="true" /> Open directions
            </a>
          ) : null}
        </div>
      </div>
    )
  }

  return (
    <div className={cn("relative min-h-[360px] overflow-hidden rounded-2xl border border-gray-200 bg-gray-100", className)}>
      {!ready ? <div className="absolute inset-0 z-10 animate-pulse bg-gray-200" aria-label="Loading clinic map" /> : null}
      <div ref={containerRef} className="absolute inset-0" aria-label="Interactive map showing clinic locations" />
    </div>
  )
}
