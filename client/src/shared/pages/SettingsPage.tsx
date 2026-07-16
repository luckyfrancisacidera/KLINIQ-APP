import { useEffect, useState, type FormEvent } from "react"
import { useMutation } from "@tanstack/react-query"
import { Download, KeyRound, ShieldCheck, WifiOff } from "lucide-react"
import { useNavigate } from "react-router-dom"
import { useAuth } from "@app/providers/AuthProviders"
import { authApi } from "@shared/api/auth.api"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

interface InstallPromptEvent extends Event {
  prompt: () => Promise<void>
  userChoice: Promise<{ outcome: "accepted" | "dismissed" }>
}

export default function SettingsPage() {
  const [installPrompt, setInstallPrompt] = useState<InstallPromptEvent | null>(null)
  const [installed, setInstalled] = useState(window.matchMedia("(display-mode: standalone)").matches)
  const [currentPassword, setCurrentPassword] = useState("")
  const [newPassword, setNewPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")
  const { setUser } = useAuth()
  const navigate = useNavigate()

  useEffect(() => {
    const onPrompt = (event: Event) => { event.preventDefault(); setInstallPrompt(event as InstallPromptEvent) }
    const onInstalled = () => { setInstalled(true); setInstallPrompt(null) }
    window.addEventListener("beforeinstallprompt", onPrompt)
    window.addEventListener("appinstalled", onInstalled)
    return () => { window.removeEventListener("beforeinstallprompt", onPrompt); window.removeEventListener("appinstalled", onInstalled) }
  }, [])

  const passwordMutation = useMutation({
    mutationFn: () => authApi.changePassword({ currentPassword, newPassword, confirmPassword }),
    onSuccess: () => {
      setUser(null)
      navigator.serviceWorker?.controller?.postMessage({ type: "CLEAR_USER_CACHES" })
      navigate("/login", { replace: true, state: { passwordChanged: true } })
    },
  })

  const install = async () => {
    if (!installPrompt) return
    await installPrompt.prompt()
    await installPrompt.userChoice
    setInstallPrompt(null)
  }
  const changePassword = (event: FormEvent) => { event.preventDefault(); passwordMutation.mutate() }

  return (
    <div className="space-y-6">
      <PageHeader title="Settings" description="Manage password security, installability, and offline behavior for this KLINIQ account." />
      <div className="grid gap-4 lg:grid-cols-2">
        <section className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm"><Download className="size-7 text-brand-600" /><h2 className="mt-3 font-Geist-Bold text-lg text-gray-950">Install KLINIQ</h2><p className="mt-2 text-sm leading-6 text-gray-600">Install the app on supported browsers for a standalone experience. Live availability, authentication, and appointment changes still require a network connection.</p><div className="mt-5">{installed ? <span className="inline-flex rounded-full bg-emerald-50 px-3 py-1.5 text-sm font-Geist-Semibold text-emerald-800">Installed</span> : installPrompt ? <Button type="button" className="h-11 bg-brand-600 px-5 text-white" onClick={install}><Download /> Install app</Button> : <p className="text-sm text-gray-500">Use your browser’s Install App or Add to Home Screen option when available.</p>}</div></section>
        <section className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm"><WifiOff className="size-7 text-brand-600" /><h2 className="mt-3 font-Geist-Bold text-lg text-gray-950">Offline behavior</h2><p className="mt-2 text-sm leading-6 text-gray-600">The public application shell and offline explanation remain available. KLINIQ never queues bookings silently and does not cache authentication or appointment mutation responses.</p></section>
        <section className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm lg:col-span-2">
          <div className="flex items-center gap-3"><KeyRound className="size-7 text-brand-600" /><div><h2 className="font-Geist-Bold text-lg text-gray-950">Change password</h2><p className="text-sm text-gray-600">All refresh sessions are revoked after a successful change.</p></div></div>
          {passwordMutation.isError ? <p className="mt-4 rounded-xl bg-red-50 px-4 py-3 text-sm text-red-800" role="alert">{getApiErrorMessage(passwordMutation.error)}</p> : null}
          <form onSubmit={changePassword} className="mt-5 grid gap-4 sm:grid-cols-3"><div><Label htmlFor="currentPassword" className="mb-2">Current password</Label><Input id="currentPassword" type="password" autoComplete="current-password" value={currentPassword} onChange={(event) => setCurrentPassword(event.target.value)} className="h-11" required /></div><div><Label htmlFor="newPassword" className="mb-2">New password</Label><Input id="newPassword" type="password" autoComplete="new-password" value={newPassword} onChange={(event) => setNewPassword(event.target.value)} className="h-11" minLength={8} required /></div><div><Label htmlFor="confirmPassword" className="mb-2">Confirm password</Label><Input id="confirmPassword" type="password" autoComplete="new-password" value={confirmPassword} onChange={(event) => setConfirmPassword(event.target.value)} className="h-11" minLength={8} required /></div><div className="sm:col-span-3 flex justify-end"><Button type="submit" className="h-11 bg-brand-600 px-5 text-white" disabled={passwordMutation.isPending || newPassword !== confirmPassword}>{passwordMutation.isPending ? "Changing…" : "Change password"}</Button></div></form>
        </section>
        <section className="rounded-2xl border border-brand-100 bg-brand-50 p-5 lg:col-span-2"><ShieldCheck className="size-7 text-brand-700" /><h2 className="mt-3 font-Geist-Bold text-lg text-brand-950">Account security</h2><p className="mt-2 max-w-3xl text-sm leading-6 text-brand-900">Authentication uses HttpOnly cookies, short-lived access tokens, refresh-token rotation, logout revocation, and backend role and ownership checks. Password reset responses do not reveal whether an account exists.</p></section>
      </div>
    </div>
  )
}
