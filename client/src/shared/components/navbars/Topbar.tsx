import { LogOut, Menu } from "lucide-react"
import { useNavigate } from "react-router-dom"
import { useAuth } from "@app/providers/AuthProviders"

const Topbar = ({ onMenu }: { onMenu: () => void }) => {
  const { logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = async () => {
    await logout()
    navigate("/login", { replace: true })
  }

  return (
    <header className="sticky top-0 z-30 flex min-h-16 items-center justify-between border-b border-gray-200 bg-white/95 px-4 backdrop-blur sm:px-6 lg:px-8">
      <button type="button" className="grid size-11 place-items-center rounded-xl text-gray-700 hover:bg-gray-100 lg:hidden" aria-label="Open navigation" onClick={onMenu}>
        <Menu className="size-5" aria-hidden="true" />
      </button>
      <div className="hidden lg:block">
        <p className="text-sm text-gray-500">Secure healthcare workspace</p>
      </div>
      <button type="button" className="inline-flex min-h-11 items-center gap-2 rounded-xl px-3 text-sm font-Geist-Semibold text-gray-600 hover:bg-gray-100 hover:text-gray-950" onClick={handleLogout}>
        <LogOut className="size-4" aria-hidden="true" />
        <span className="hidden sm:inline">Sign out</span>
      </button>
    </header>
  )
}

export default Topbar
