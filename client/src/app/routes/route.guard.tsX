import type { ReactNode } from "react"
import { Navigate, useLocation } from "react-router-dom"
import { useAuth } from "../providers/AuthProviders"
import { ROLES } from "../providers/auth.provider.type"
import type { AppRouteKey } from "./route.config"
import { routeAccess } from "./route.access"
import { FullPageLoader } from "@shared/components/feedback/FullPageLoader"

export const RequireAuth = ({ children }: { children: ReactNode }) => {
  const { isAuthenticated, isLoading } = useAuth()
  const location = useLocation()

  if (isLoading) return <FullPageLoader label="Restoring your secure session…" />
  if (!isAuthenticated) return <Navigate to="/login" replace state={{ from: location.pathname + location.search }} />
  return <>{children}</>
}

export const RequireRouteAccess = ({ routeKey, children }: { routeKey: AppRouteKey; children: ReactNode }) => {
  const { user, isLoading } = useAuth()
  if (isLoading) return <FullPageLoader />
  if (!user || !routeAccess[routeKey].includes(user.role)) return <Navigate to="/unauthorized" replace />
  return <>{children}</>
}

export const PublicRoute = ({ children }: { children: ReactNode }) => {
  const { isAuthenticated, isLoading, user } = useAuth()
  const location = useLocation()

  if (isLoading) return <FullPageLoader />

  if (isAuthenticated) {
    const requested = (location.state as { from?: string } | null)?.from
    if (requested?.startsWith("/")) return <Navigate to={requested} replace />
    if (user?.role === ROLES.ADMIN) return <Navigate to="/admin/dashboard" replace />
    if (user?.role === ROLES.PRACTITIONER) return <Navigate to="/practitioner/dashboard" replace />
    return <Navigate to="/patient/dashboard" replace />
  }

  return <>{children}</>
}
