import { useEffect, useState, type FormEvent } from "react"
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { MapPin, Save, ShieldCheck } from "lucide-react"
import { practitionerApi } from "@shared/api/practitioner.api"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import type { UpdatePractitionerPayload } from "@shared/types/practitioner.types"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

export default function PractitionerProfilePage() {
  const [form, setForm] = useState<UpdatePractitionerPayload>({ firstName: "", lastName: "", specializations: [] })
  const [specializations, setSpecializations] = useState("")
  const [message, setMessage] = useState<string | null>(null)
  const queryClient = useQueryClient()
  const query = useQuery({ queryKey: ["practitioner", "me"], queryFn: ({ signal }) => practitionerApi.getCurrent(signal).then(({ data }) => data) })

  useEffect(() => {
    if (!query.data) return
    setForm({ firstName: query.data.firstName, lastName: query.data.lastName, specializations: query.data.specializations })
    setSpecializations(query.data.specializations.join(", "))
  }, [query.data])

  const mutation = useMutation({
    mutationFn: () => practitionerApi.update(query.data!.id, { ...form, specializations: specializations.split(",").map((item) => item.trim()).filter(Boolean) }),
    onSuccess: async ({ data }) => { queryClient.setQueryData(["practitioner", "me"], data); setMessage("Profile updated successfully.") },
    onError: (error) => setMessage(getApiErrorMessage(error)),
  })

  const submit = (event: FormEvent) => { event.preventDefault(); setMessage(null); mutation.mutate() }
  if (query.isError) return <ErrorState title="Practitioner profile could not be loaded" onRetry={() => query.refetch()} />

  return (
    <div className="space-y-6">
      <PageHeader title="Professional profile" description="Update the public name and specialties patients use to discover your care. License and clinic assignment remain protected administrative data." />
      <div className="grid gap-6 lg:grid-cols-[1fr_340px]">
        <form onSubmit={submit} className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm sm:p-6">
          {query.isPending ? <div className="h-80 animate-pulse rounded-xl bg-gray-100" /> : <div className="grid gap-5 sm:grid-cols-2">
            <div><Label htmlFor="firstName" className="mb-2">First name</Label><Input id="firstName" value={form.firstName} onChange={(event) => setForm((current) => ({ ...current, firstName: event.target.value }))} className="h-11" required /></div>
            <div><Label htmlFor="lastName" className="mb-2">Last name</Label><Input id="lastName" value={form.lastName} onChange={(event) => setForm((current) => ({ ...current, lastName: event.target.value }))} className="h-11" required /></div>
            <div className="sm:col-span-2"><Label htmlFor="specializations" className="mb-2">Specializations</Label><Input id="specializations" value={specializations} onChange={(event) => setSpecializations(event.target.value)} className="h-11" placeholder="Family Medicine, Pediatrics" required /><p className="mt-2 text-xs text-gray-500">Separate multiple specialties with commas.</p></div>
            {message ? <p className={`sm:col-span-2 rounded-xl px-4 py-3 text-sm ${message.includes("successfully") ? "bg-emerald-50 text-emerald-800" : "bg-red-50 text-red-800"}`} role="status">{message}</p> : null}
            <div className="sm:col-span-2 flex justify-end"><Button type="submit" className="h-11 bg-brand-600 px-5 text-white" disabled={mutation.isPending}><Save /> {mutation.isPending ? "Saving…" : "Save profile"}</Button></div>
          </div>}
        </form>
        <aside className="h-fit space-y-4">
          <section className="rounded-2xl border border-brand-100 bg-brand-50 p-5"><ShieldCheck className="size-7 text-brand-700" /><h2 className="mt-3 font-Geist-Bold text-brand-950">Verified credentials</h2><dl className="mt-4 space-y-3 text-sm"><div><dt className="text-brand-700">License number</dt><dd className="mt-1 font-Geist-Semibold text-brand-950">{query.data?.licenseNumber ?? "Loading…"}</dd></div></dl></section>
          <section className="rounded-2xl border border-gray-200 bg-white p-5"><MapPin className="size-6 text-brand-600" /><h2 className="mt-3 font-Geist-Bold text-gray-950">Clinic assignment</h2><p className="mt-2 text-sm leading-6 text-gray-600">{query.data?.clinic?.name ?? "No clinic is currently assigned. Contact a platform administrator to update this protected relationship."}</p></section>
        </aside>
      </div>
    </div>
  )
}
