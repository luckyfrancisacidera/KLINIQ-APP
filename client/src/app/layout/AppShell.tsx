import { Suspense, useState } from "react"
import { Outlet } from "react-router-dom"
import Topbar from "@shared/components/navbars/Topbar"
import { Sidebar } from "@shared/components/navbars/Sidebar"
import { FullPageLoader } from "@shared/components/feedback/FullPageLoader"

const AppShell = () => {
  const [sidebarOpen, setSidebarOpen] = useState(false)

  return (
    <div className="min-h-screen bg-surface">
      <a href="#main-content" className="sr-only z-[120] rounded-lg bg-white px-4 py-2 focus:not-sr-only focus:fixed focus:left-4 focus:top-4">Skip to content</a>
      <Sidebar open={sidebarOpen} onClose={() => setSidebarOpen(false)} />
      <div className="lg:pl-72">
        <Topbar onMenu={() => setSidebarOpen(true)} />
        <main id="main-content" className="mx-auto w-full max-w-[1440px] p-4 sm:p-6 lg:p-8">
          <Suspense fallback={<FullPageLoader label="Loading workspace…" />}>
            <Outlet />
          </Suspense>
        </main>
      </div>
    </div>
  )
}

export default AppShell
