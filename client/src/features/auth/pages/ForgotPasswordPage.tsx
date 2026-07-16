import { useState, type FormEvent } from "react"
import { useMutation } from "@tanstack/react-query"
import { ArrowLeft, Mail } from "lucide-react"
import { Link } from "react-router-dom"
import { authApi } from "@shared/api/auth.api"
import { PublicHeader } from "@shared/components/navigation/PublicHeader"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState("")
  const mutation = useMutation({ mutationFn: () => authApi.forgotPassword({ email }) })
  const submit = (event: FormEvent) => { event.preventDefault(); mutation.mutate() }

  return <div className="min-h-screen bg-surface"><PublicHeader /><main className="mx-auto flex max-w-5xl justify-center px-4 py-14"><section className="w-full max-w-lg rounded-2xl border border-gray-200 bg-white p-6 shadow-sm sm:p-8"><Mail className="size-8 text-brand-600" /><h1 className="mt-4 font-Geist-ExtraBold text-2xl text-gray-950">Reset your password</h1><p className="mt-2 text-sm leading-6 text-gray-600">Enter your account email. To prevent account discovery, KLINIQ returns the same confirmation whether or not the email exists.</p>{mutation.isSuccess ? <div className="mt-6 rounded-xl bg-emerald-50 px-4 py-4 text-sm leading-6 text-emerald-800">{mutation.data.data.message}</div> : <form onSubmit={submit} className="mt-6 space-y-5">{mutation.isError ? <p className="rounded-xl bg-red-50 px-4 py-3 text-sm text-red-800" role="alert">{getApiErrorMessage(mutation.error)}</p> : null}<div><Label htmlFor="email" className="mb-2">Email address</Label><Input id="email" type="email" autoComplete="email" value={email} onChange={(event) => setEmail(event.target.value)} className="h-11" required /></div><Button type="submit" className="h-11 w-full bg-brand-600 text-white" disabled={mutation.isPending}>{mutation.isPending ? "Sending…" : "Send reset link"}</Button></form>}<Link to="/login" className="mt-6 inline-flex min-h-11 items-center gap-2 text-sm font-Geist-Semibold text-brand-700"><ArrowLeft className="size-4" /> Back to sign in</Link></section></main></div>
}
