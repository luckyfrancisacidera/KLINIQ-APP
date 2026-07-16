import { useState, type FormEvent } from "react"
import { useMutation } from "@tanstack/react-query"
import { CheckCircle2, KeyRound } from "lucide-react"
import { Link, useSearchParams } from "react-router-dom"
import { authApi } from "@shared/api/auth.api"
import { PublicHeader } from "@shared/components/navigation/PublicHeader"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

export default function ResetPasswordPage() {
  const [params] = useSearchParams()
  const email = params.get("email") ?? ""
  const token = params.get("token") ?? ""
  const [password, setPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")
  const mutation = useMutation({ mutationFn: () => authApi.resetPassword({ email, token, password, confirmPassword }) })
  const submit = (event: FormEvent) => { event.preventDefault(); mutation.mutate() }

  return <div className="min-h-screen bg-surface"><PublicHeader /><main className="mx-auto flex max-w-5xl justify-center px-4 py-14"><section className="w-full max-w-lg rounded-2xl border border-gray-200 bg-white p-6 shadow-sm sm:p-8">{mutation.isSuccess ? <div className="text-center"><CheckCircle2 className="mx-auto size-11 text-emerald-600" /><h1 className="mt-4 font-Geist-ExtraBold text-2xl text-gray-950">Password reset</h1><p className="mt-2 text-sm text-gray-600">Your previous sessions were revoked. Sign in with the new password.</p><Button asChild className="mt-6 h-11 bg-brand-600 px-5 text-white"><Link to="/login">Sign in</Link></Button></div> : <><KeyRound className="size-8 text-brand-600" /><h1 className="mt-4 font-Geist-ExtraBold text-2xl text-gray-950">Choose a new password</h1><p className="mt-2 text-sm leading-6 text-gray-600">Use at least 8 characters with uppercase, lowercase, a number, and a symbol.</p>{(!email || !token) ? <p className="mt-5 rounded-xl bg-red-50 px-4 py-3 text-sm text-red-800">This reset link is incomplete. Request a new link.</p> : null}<form onSubmit={submit} className="mt-6 space-y-5">{mutation.isError ? <p className="rounded-xl bg-red-50 px-4 py-3 text-sm text-red-800" role="alert">{getApiErrorMessage(mutation.error)}</p> : null}<div><Label htmlFor="password" className="mb-2">New password</Label><Input id="password" type="password" autoComplete="new-password" minLength={8} value={password} onChange={(event) => setPassword(event.target.value)} className="h-11" required /></div><div><Label htmlFor="confirm" className="mb-2">Confirm new password</Label><Input id="confirm" type="password" autoComplete="new-password" minLength={8} value={confirmPassword} onChange={(event) => setConfirmPassword(event.target.value)} className="h-11" required /></div><Button type="submit" className="h-11 w-full bg-brand-600 text-white" disabled={!email || !token || password !== confirmPassword || mutation.isPending}>{mutation.isPending ? "Resetting…" : "Reset password"}</Button></form></>}</section></main></div>
}
