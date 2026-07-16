import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { ReactQueryDevtools } from "@tanstack/react-query-devtools"
import "./index.css"
import App from "./App"
import { NetworkStatus } from "@shared/components/pwa/NetworkStatus"
import { PwaUpdatePrompt } from "@shared/components/pwa/PwaUpdatePrompt"

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 60_000,
      gcTime: 10 * 60_000,
      retry: (failureCount, error: unknown) => {
        const status = typeof error === "object" && error && "response" in error
          ? (error as { response?: { status?: number } }).response?.status
          : undefined
        return status && status >= 400 && status < 500 ? false : failureCount < 2
      },
      refetchOnWindowFocus: false,
    },
    mutations: { retry: false },
  },
})

if ("serviceWorker" in navigator && import.meta.env.PROD) {
  window.addEventListener("load", async () => {
    let registration: ServiceWorkerRegistration
    try {
      registration = await navigator.serviceWorker.register("/sw.js", { scope: "/" })
    } catch (error) {
      console.error("KLINIQ service worker registration failed", error)
      return
    }
    registration.addEventListener("updatefound", () => {
      const worker = registration.installing
      worker?.addEventListener("statechange", () => {
        if (worker.state === "installed" && navigator.serviceWorker.controller) {
          window.dispatchEvent(new CustomEvent("kliniq:pwa-update", { detail: registration }))
        }
      })
    })

    let refreshing = false
    navigator.serviceWorker.addEventListener("controllerchange", () => {
      if (refreshing) return
      refreshing = true
      window.location.reload()
    })
  })
}

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <NetworkStatus />
      <App />
      <PwaUpdatePrompt />
      {import.meta.env.DEV ? <ReactQueryDevtools initialIsOpen={false} /> : null}
    </QueryClientProvider>
  </StrictMode>,
)
