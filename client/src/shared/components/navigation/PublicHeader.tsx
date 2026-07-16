import { Menu, X } from "lucide-react"
import { useState } from "react"
import { Link, NavLink } from "react-router-dom"
import { useAuth } from "@app/providers/AuthProviders"
import { ROLES } from "@app/providers/auth.provider.type"

export function PublicHeader() {
  const [open, setOpen] = useState(false)
  const { isAuthenticated, user } = useAuth()
  const dashboardPath = user?.role === ROLES.ADMIN ? "/admin/dashboard" : user?.role === ROLES.PRACTITIONER ? "/practitioner/dashboard" : "/patient/dashboard"

  return (
    <header className="sticky top-0 z-40 border-b border-gray-200 bg-white/95 backdrop-blur">
      <div className="mx-auto flex min-h-16 max-w-[1440px] items-center justify-between px-4 sm:px-6 lg:px-8">
        <Link to="/clinics" className="flex min-h-11 items-center gap-2" onClick={() => setOpen(false)}>
          <img src="/logo.png" alt="" className="size-10 rounded-xl object-contain" />
          <span className="font-Geist-ExtraBold text-xl tracking-tight text-brand-800">KLINIQ</span>
        </Link>
        <nav className="hidden items-center gap-1 md:flex" aria-label="Public navigation">
          <NavLink to="/clinics" className="rounded-lg px-4 py-2 text-sm font-Geist-Semibold text-gray-600 hover:bg-gray-100 hover:text-gray-950">Find clinics</NavLink>
          <NavLink to="/practitioners" className="rounded-lg px-4 py-2 text-sm font-Geist-Semibold text-gray-600 hover:bg-gray-100 hover:text-gray-950">Find doctors</NavLink>
          <NavLink to="/symptom-search" className="rounded-lg px-4 py-2 text-sm font-Geist-Semibold text-gray-600 hover:bg-gray-100 hover:text-gray-950">Symptom assistant</NavLink>
          <NavLink to="/apply" className="rounded-lg px-4 py-2 text-sm font-Geist-Semibold text-gray-600 hover:bg-gray-100 hover:text-gray-950">For practitioners</NavLink>
          <Link to={isAuthenticated ? dashboardPath : "/login"} className="ml-2 inline-flex min-h-11 items-center rounded-xl bg-brand-600 px-5 text-sm font-Geist-Semibold text-white hover:bg-brand-700">
            {isAuthenticated ? "Open dashboard" : "Sign in"}
          </Link>
        </nav>
        <button type="button" className="grid size-11 place-items-center rounded-xl hover:bg-gray-100 md:hidden" aria-label="Toggle navigation" onClick={() => setOpen((value) => !value)}>
          {open ? <X className="size-5" /> : <Menu className="size-5" />}
        </button>
      </div>
      {open ? (
        <nav className="border-t border-gray-100 bg-white p-4 md:hidden" aria-label="Mobile public navigation">
          <div className="grid gap-1">
            <NavLink to="/clinics" className="min-h-11 rounded-xl px-3 py-3 text-sm font-Geist-Semibold" onClick={() => setOpen(false)}>Find clinics</NavLink>
            <NavLink to="/practitioners" className="min-h-11 rounded-xl px-3 py-3 text-sm font-Geist-Semibold" onClick={() => setOpen(false)}>Find doctors</NavLink>
            <NavLink to="/symptom-search" className="min-h-11 rounded-xl px-3 py-3 text-sm font-Geist-Semibold" onClick={() => setOpen(false)}>Symptom assistant</NavLink>
            <NavLink to="/apply" className="min-h-11 rounded-xl px-3 py-3 text-sm font-Geist-Semibold" onClick={() => setOpen(false)}>For practitioners</NavLink>
            <Link to={isAuthenticated ? dashboardPath : "/login"} className="mt-2 flex min-h-11 items-center justify-center rounded-xl bg-brand-600 px-4 text-sm font-Geist-Semibold text-white" onClick={() => setOpen(false)}>
              {isAuthenticated ? "Open dashboard" : "Sign in"}
            </Link>
          </div>
        </nav>
      ) : null}
    </header>
  )
}
