import { BrainCircuit, CalendarDays, ClipboardList, LayoutDashboard, Search, Settings, Stethoscope, UserRound, Users, X } from "lucide-react"
import { NavLink } from "react-router-dom"
import { useAuth } from "@app/providers/AuthProviders"
import { ROLES } from "@app/providers/auth.provider.type"
import { cn } from "@shared/lib/utils"

const patientLinks = [
  { to: "/patient/dashboard", label: "Overview", icon: LayoutDashboard },
  { to: "/clinics", label: "Find care", icon: Search },
  { to: "/symptom-search", label: "Symptom assistant", icon: BrainCircuit },
  { to: "/patient/appointments", label: "Appointments", icon: CalendarDays },
  { to: "/patient/profile", label: "Profile", icon: UserRound },
  { to: "/patient/settings", label: "Settings", icon: Settings },
]

const practitionerLinks = [
  { to: "/practitioner/dashboard", label: "Overview", icon: LayoutDashboard },
  { to: "/practitioner/appointments", label: "Appointments", icon: CalendarDays },
  { to: "/practitioner/schedule", label: "Availability", icon: ClipboardList },
  { to: "/practitioner/profile", label: "Profile", icon: Stethoscope },
  { to: "/practitioner/settings", label: "Settings", icon: Settings },
]

const adminLinks = [
  { to: "/admin/dashboard", label: "Overview", icon: LayoutDashboard },
  { to: "/admin/patients", label: "Patients", icon: Users },
  { to: "/admin/practitioners", label: "Practitioners", icon: Stethoscope },
  { to: "/admin/account-requests", label: "Applications", icon: ClipboardList },
]

export function Sidebar({ open, onClose }: { open: boolean; onClose: () => void }) {
  const { user } = useAuth()
  const links = user?.role === ROLES.ADMIN ? adminLinks : user?.role === ROLES.PRACTITIONER ? practitionerLinks : patientLinks

  return (
    <>
      {open ? <button type="button" className="fixed inset-0 z-40 bg-gray-950/35 lg:hidden" aria-label="Close navigation" onClick={onClose} /> : null}
      <aside className={cn(
        "fixed inset-y-0 left-0 z-50 flex w-72 flex-col border-r border-gray-200 bg-white transition-transform duration-200 lg:translate-x-0",
        open ? "translate-x-0" : "-translate-x-full",
      )}>
        <div className="flex h-18 items-center justify-between border-b border-gray-100 px-5">
          <NavLink to="/clinics" className="flex min-h-11 items-center gap-3" onClick={onClose}>
            <img src="/logo.png" alt="" className="size-10 rounded-xl object-contain" />
            <div>
              <span className="block font-Geist-ExtraBold text-lg tracking-tight text-brand-800">KLINIQ</span>
              <span className="block text-[11px] text-gray-500">Care, made easier</span>
            </div>
          </NavLink>
          <button type="button" className="grid size-11 place-items-center rounded-xl hover:bg-gray-100 lg:hidden" aria-label="Close navigation" onClick={onClose}>
            <X className="size-5" aria-hidden="true" />
          </button>
        </div>

        <nav className="flex-1 space-y-1 overflow-y-auto p-4" aria-label="Primary navigation">
          {links.map(({ to, label, icon: Icon }) => (
            <NavLink
              key={to}
              to={to}
              onClick={onClose}
              className={({ isActive }) => cn(
                "flex min-h-11 items-center gap-3 rounded-xl px-3 text-sm font-Geist-Semibold transition-colors",
                isActive ? "bg-brand-50 text-brand-800" : "text-gray-600 hover:bg-gray-100 hover:text-gray-950",
              )}
            >
              <Icon className="size-5" aria-hidden="true" />
              {label}
            </NavLink>
          ))}
        </nav>

        <div className="border-t border-gray-100 p-4">
          <div className="rounded-xl bg-gray-50 p-3">
            <p className="truncate text-sm font-Geist-Semibold text-gray-900">{user?.email}</p>
            <p className="mt-0.5 text-xs text-gray-500">{user?.role}</p>
          </div>
        </div>
      </aside>
    </>
  )
}
