export {}

declare global {
  interface Window {
    google?: {
      maps: {
        Map: new (element: HTMLElement, options: Record<string, unknown>) => GoogleMap
        Marker: new (options: Record<string, unknown>) => GoogleMarker
        LatLngBounds: new () => GoogleBounds
        event: { clearInstanceListeners(instance: unknown): void }
      }
    }
  }

  interface GoogleMap {
    fitBounds(bounds: GoogleBounds, padding?: number): void
    setCenter(center: { lat: number; lng: number }): void
    setZoom(zoom: number): void
  }

  interface GoogleMarker {
    addListener(eventName: string, callback: () => void): void
    setMap(map: GoogleMap | null): void
  }

  interface GoogleBounds {
    extend(position: { lat: number; lng: number }): void
    isEmpty(): boolean
  }
}
