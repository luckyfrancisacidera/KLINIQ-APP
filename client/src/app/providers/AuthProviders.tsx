import { createContext, useContext, useEffect, useMemo, useState, type PropsWithChildren } from "react"
import type { AuthContextValue, AuthUser, UserRole } from "./auth.provider.type"
import { ROLES } from "./auth.provider.type"
import { authApi } from "@shared/api/auth.api"
import { SESSION_EXPIRED_EVENT } from "@shared/api/axios"

const AuthContext = createContext<AuthContextValue | null>(null)

const isUserRole = (role: string): role is UserRole => Object.values(ROLES).includes(role as UserRole)

export const AuthProvider = ({ children }: PropsWithChildren) => {
  const [user, setUser] = useState<AuthUser | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    let mounted = true

    authApi.me()
      .then(({ data }) => {
        if (mounted && isUserRole(data.role)) {
          setUser({ userId: data.userId, email: data.email, role: data.role })
        }
      })
      .catch(() => mounted && setUser(null))
      .finally(() => mounted && setIsLoading(false))

    const expireSession = () => setUser(null)
    window.addEventListener(SESSION_EXPIRED_EVENT, expireSession)

    return () => {
      mounted = false
      window.removeEventListener(SESSION_EXPIRED_EVENT, expireSession)
    }
  }, [])

  const logout = async () => {
    try {
      await authApi.logout()
    } finally {
      setUser(null)
      navigator.serviceWorker?.controller?.postMessage({ type: "CLEAR_USER_CACHES" })
    }
  }

  const value = useMemo<AuthContextValue>(() => ({
    user,
    isAuthenticated: user !== null,
    isLoading,
    setUser,
    logout,
  }), [user, isLoading])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export const useAuth = () => {
  const context = useContext(AuthContext)
  if (!context) throw new Error("useAuth must be used within an AuthProvider")
  return context
}
