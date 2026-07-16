import { useEffect, useRef, useState } from "react"
import { Wifi, WifiOff } from "lucide-react"
import { useNetworkStatus } from "@shared/hooks/useNetworkStatus"

export function NetworkStatus() {
  const online = useNetworkStatus()
  const wasOffline = useRef(!online)
  const [restored, setRestored] = useState(false)

  useEffect(() => {
    if (!online) {
      wasOffline.current = true
      setRestored(false)
      return
    }

    if (!wasOffline.current) return

    wasOffline.current = false
    setRestored(true)
    const timer = window.setTimeout(() => setRestored(false), 3500)
    return () => window.clearTimeout(timer)
  }, [online])

  if (!online) {
    return (
      <div className="fixed inset-x-0 top-0 z-[100] flex min-h-10 items-center justify-center gap-2 bg-gray-950 px-4 py-2 text-center text-sm text-white" role="status" aria-live="assertive">
        <WifiOff className="size-4" aria-hidden="true" />
        You are offline. Live schedules and appointment actions are unavailable.
      </div>
    )
  }

  if (!restored) return null

  return (
    <div className="fixed inset-x-0 top-0 z-[100] flex min-h-10 items-center justify-center gap-2 bg-emerald-700 px-4 py-2 text-center text-sm text-white" role="status" aria-live="polite">
      <Wifi className="size-4" aria-hidden="true" />
      Connection restored. Live KLINIQ information is available again.
    </div>
  )
}
