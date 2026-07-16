import { useState, type FormEvent } from "react"
import { useMutation } from "@tanstack/react-query"
import { CheckCircle2, KeyRound, Loader2 } from "lucide-react"
import { Link, useSearchParams } from "react-router-dom"
import { authApi } from "@shared/api/auth.api"
import { PublicHeader } from "@shared/components/navigation/PublicHeader"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

export default function SetPasswordPage() {
  const [params] = useSearchParams()
  const invitationToken = params.get("token") ?? ""
  const [password, setPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")
  const mutation = useMutation({ mutationFn: () => authApi.setPassword({ invitationToken, password, confirmPassword }) })

  const submit = (event: FormEvent) => { event.preventDefault(); mutation.mutate() }

  return (
    <div className="min-h-screen bg-surface">
      <PublicHeader />
      <main className="mx-auto flex max-w-5xl justify-center px-4 py-12 sm:px-6">
        <section className="w-full max-w-lg rounded-2xl border border-gray-200 bg-white p-6 shadow-sm sm:p-8">
          {mutation.isSuccess ? <div className="text-center"><CheckCircle2 className="mx-auto size-11 text-emerald-600" /><h1 className="mt-4 font-Geist-ExtraBold text-2xl text-gray-950">Password created</h1><p className="mt-2 text-sm leading-6 text-gray-600">{mutation.data.data.message}</p><Button asChild className="mt-6 h-11 bg-brand-600 px-5 text-white"><Link to="/login">Continue to sign in</Link></Button></div> : <>
            <div className="grid size-12 place-items-center rounded-xl bg-brand-50 text-brand-700"><KeyRound className="size-6" /></div>
            <h1 className="mt-5 font-Geist-ExtraBold text-2xl text-gray-950">Complete practitioner registration</h1>
            <p className="mt-2 text-sm leading-6 text-gray-600">Create the password for your approved practitioner invitation. Invitation links expire and can be used only once.</p>
            {!invitationToken ? <p className="mt-5 rounded-xl bg-red-50 px-4 py-3 text-sm text-red-800" role="alert">This invitation link is missing its token. Open the complete link from the approval email.</p> : null}
            {mutation.isError ? <p className="mt-5 rounded-xl bg-red-50 px-4 py-3 text-sm text-red-800" role="alert">{getApiErrorMessage(mutation.error)}</p> : null}
            <form onSubmit={submit} className="mt-6 space-y-5">
              <div><Label htmlFor="password" className="mb-2">Password</Label><Input id="password" type="password" autoComplete="new-password" value={password} onChange={(event) => setPassword(event.target.value)} minLength={8} className="h-11" required /><p className="mt-2 text-xs text-gray-500">Use at least 8 characters with uppercase, lowercase, number, and symbol.</p></div>
              <div><Label htmlFor="confirmPassword" className="mb-2">Confirm password</Label><Input id="confirmPassword" type="password" autoComplete="new-password" value={confirmPassword} onChange={(event) => setConfirmPassword(event.target.value)} minLength={8} className="h-11" required /></div>
              <Button type="submit" className="h-11 w-full bg-brand-600 text-white" disabled={!invitationToken || mutation.isPending || password !== confirmPassword}>{mutation.isPending ? <><Loader2 className="animate-spin" /> Creating password…</> : "Create password"}</Button>
            </form>
          </>}
        </section>
      </main>
    </div>
  )
}
