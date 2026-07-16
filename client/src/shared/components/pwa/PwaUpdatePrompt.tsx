import { useEffect, useState } from "react"
import { RefreshCw, X } from "lucide-react"
import { Button } from "@shared/components/ui/button"

export function PwaUpdatePrompt() {
  const [registration, setRegistration] = useState<ServiceWorkerRegistration | null>(null)

  useEffect(() => {
    const onUpdate = (event: Event) => {
      const detail = (event as CustomEvent<ServiceWorkerRegistration>).detail
      setRegistration(detail)
    }
    window.addEventListener("kliniq:pwa-update", onUpdate)
    return () => window.removeEventListener("kliniq:pwa-update", onUpdate)
  }, [])

  if (!registration?.waiting) return null

  const update = () => {
    registration.waiting?.postMessage({ type: "SKIP_WAITING" })
  }

  return (
    <div className="fixed bottom-4 left-4 right-4 z-[100] mx-auto flex max-w-xl items-center gap-3 rounded-2xl border border-brand-200 bg-white p-4 shadow-xl" role="status">
      <RefreshCw className="size-5 shrink-0 text-brand-600" aria-hidden="true" />
      <p className="min-w-0 flex-1 text-sm text-gray-700"><strong className="text-gray-950">A newer KLINIQ version is available.</strong> Refresh when you are ready.</p>
      <Button type="button" className="h-10 bg-brand-600 text-white hover:bg-brand-700" onClick={update}>Refresh</Button>
      <button type="button" className="grid size-10 place-items-center rounded-lg hover:bg-gray-100" aria-label="Dismiss update" onClick={() => setRegistration(null)}>
        <X className="size-4" aria-hidden="true" />
      </button>
    </div>
  )
}
