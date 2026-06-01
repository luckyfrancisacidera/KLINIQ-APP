import { useState } from "react"
import { Link, useNavigate } from "react-router-dom"
import { useMutation } from "@tanstack/react-query"
import { Eye, EyeOff, Loader2, Calendar, Lock, Stethoscope } from "lucide-react"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import { authApi } from "@shared/api/auth.api"
import { useAuth } from "@app/providers/AuthProviders"
import { ROLES, type UserRole } from "@app/providers/auth.provider.type"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

const getHomeRoute = (role: UserRole): string => {
  if (role === ROLES.ADMIN) return "/admin/dashboard"
  if (role === ROLES.PRACTITIONER) return "/practitioner/dashboard"
  return "/patient/appointments"
}

const LoginPage = () => {
  const navigate = useNavigate()
  const { setUser } = useAuth()

  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [showPassword, setShowPassword] = useState(false)

  const { mutate: login, isPending, error } = useMutation({
    mutationFn: authApi.login,
    onSuccess: ({ data }) => {
      setUser({ userId: data.userId, email: data.email, role: data.role as UserRole })
      navigate(getHomeRoute(data.role as UserRole), { replace: true })
    },
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    login({ email, password })
  }

  const inputClass = "h-11 border-gray-200 focus-visible:ring-brand-500 focus-visible:border-brand-500"

  return (
    <div className="min-h-screen bg-surface flex flex-col">
      <div className="h-1 bg-brand-500 w-full" />

      <div className="flex flex-1 flex-col lg:flex-row">

        {/* Left panel */}
        <div className="hidden lg:flex lg:w-[50%] bg-brand-700 flex-col justify-between p-12 relative overflow-hidden">
          <div className="absolute -top-24 -right-24 w-96 h-96 rounded-full bg-brand-600 opacity-40" />
          <div className="absolute -bottom-32 -left-16 w-80 h-80 rounded-full bg-brand-800 opacity-60" />

          <div className="relative z-10">
            <div className="flex items-center gap-3 mb-16">
              <div className="w-9 h-9 rounded-lg bg-white flex items-center justify-center">
                <img src="/logo.png" alt="Kliniq Logo" className="w-10 h-10 object-contain"/>
              </div>
              <span className="text-white font-Geist-Bold text-xl tracking-tight">Kliniq</span>
            </div>
            <h1 className="text-white font-Geist-Bold text-5xl leading-tight mb-6">
              Healthcare,<br />
              <span className="text-brand-300">simplified.</span>
            </h1>
            <p className="font-Geist-Regular text-brand-200 text-lg leading-relaxed max-w-xs">
              Connect with trusted practitioners, manage your appointments, and take control of your health journey.
            </p>
          </div>

          <div className="font-Geist-Regular relative z-10 space-y-4">
            {[
              { icon: Calendar, text: "Book appointments in seconds" },
              { icon: Lock, text: "Your health data, always private" },
              { icon: Stethoscope, text: "Verified practitioners only" },
            ].map(({ icon: Icon, text }) => (
              <div key={text} className="flex items-center gap-3">
                <Icon className="w-4 h-4 text-brand-300 shrink-0" />
                <span className="text-brand-100 text-sm">{text}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Right panel */}
        <div className="flex-1 flex items-center justify-center px-6 py-12 lg:px-16">
          <div className="w-full max-w-md">

            <div className="flex items-center gap-2 mb-10 lg:hidden">
              <div className="w-8 h-8 rounded-lg bg-brand-500 flex items-center justify-center">
               <img src="/logo.png" alt="Kliniq Logo" className="w-10 h-10 object-contain"/>
              </div>
              <span className="font-Geist-Bold text-lg text-brand-700 tracking-tight">Kliniq</span>
            </div>

            <div className="mb-8">
              <h2 className="font-Geist-Bold text-2xl text-gray-900 mb-1">Welcome back</h2>
              <p className="font-Geist-Regular text-sm">Sign in to your account to continue</p>
            </div>

            {error && (
              <div className="mb-6 rounded-lg bg-red-50 border border-red-200 px-4 py-3">
                <p className="text-red-700 text-sm">{getApiErrorMessage(error)}</p>
              </div>
            )}

            <form onSubmit={handleSubmit} className="font-Geist-Semibold space-y-5">
              <div className="space-y-1.5">
                <Label htmlFor="email" className="text-sm font-Geist-Semibold text-gray-700">
                  Email Address
                </Label>
                <Input id="email" type="email" autoComplete="email"
                  placeholder="you@example.com" value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required className={inputClass} />
              </div>

              <div className=" space-y-1.5">
                <div className="flex items-center justify-between">
                  <Label htmlFor="password" className="text-sm font-Geist-Semibold text-gray-700">
                    Password
                  </Label>
                  <button type="button"
                    className="text-xs text-brand-600 hover:text-brand-700 font-Geist-Semibold">
                    Forgot password?
                  </button>
                </div>
                <div className="relative">
                  <Input id="password" type={showPassword ? "text" : "password"}
                    autoComplete="current-password" placeholder="••••••••"
                    value={password} onChange={(e) => setPassword(e.target.value)}
                    required className={`${inputClass} pr-10`} />
                  <button type="button" onClick={() => setShowPassword(p => !p)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                    aria-label={showPassword ? "Hide password" : "Show password"}>
                    {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                  </button>
                </div>
              </div>

              <Button type="submit" disabled={isPending}
                className="w-full h-11 bg-brand-500 hover:bg-brand-600 active:bg-brand-700 text-white font-Geist-Semibold transition-colors">
                {isPending
                  ? <span className="flex items-center gap-2"><Loader2 className="w-4 h-4 animate-spin" />Signing in…</span>
                  : "Sign in"}
              </Button>
            </form>

            <p className="font-Geist-Regular mt-6 text-center text-sm text-muted">
              Don't have an account?{" "}
              <Link to="/register" className="text-brand-600 hover:text-brand-700 font-Geist-Semibold">Create one</Link>
            </p>
            <div className="mt-4 text-center">
              <span className="font-Geist-Regular text-muted text-xs">Are you a practitioner? </span>
              <Link to="/apply" className="text-xs text-brand-600 hover:text-brand-700 font-Geist-Semibold">Apply here</Link>
            </div>

          </div>
        </div>
      </div>
    </div>
  )
}

export default LoginPage