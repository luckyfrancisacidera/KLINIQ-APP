import { useEffect, useState, type FormEvent } from "react"
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { Save, ShieldCheck } from "lucide-react"
import { patientApi } from "@shared/api/patient.api"
import { ErrorState } from "@shared/components/feedback/ErrorState"
import { PageHeader } from "@shared/components/navigation/PageHeader"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import type { UpdatePatientPayload } from "@shared/types/patient.types"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

const emptyForm: UpdatePatientPayload = { firstName: "", lastName: "", street: "", city: "", country: "", phoneNumber: "", emergencyContact: "" }

export default function PatientProfilePage() {
  const [form, setForm] = useState<UpdatePatientPayload>(emptyForm)
  const [message, setMessage] = useState<string | null>(null)
  const queryClient = useQueryClient()
  const query = useQuery({ queryKey: ["patient", "me"], queryFn: ({ signal }) => patientApi.getCurrent(signal).then(({ data }) => data) })

  useEffect(() => {
    if (!query.data) return
    setForm({ firstName: query.data.firstName, lastName: query.data.lastName, street: query.data.street, city: query.data.city, country: query.data.country, phoneNumber: query.data.phoneNumber ?? "", emergencyContact: query.data.emergencyContact ?? "" })
  }, [query.data])

  const mutation = useMutation({
    mutationFn: () => patientApi.update(query.data!.id, form),
    onSuccess: async ({ data }) => {
      queryClient.setQueryData(["patient", "me"], data)
      setMessage("Profile updated successfully.")
    },
    onError: (error) => setMessage(getApiErrorMessage(error)),
  })

  const update = (field: keyof UpdatePatientPayload, value: string) => setForm((current) => ({ ...current, [field]: value }))
  const submit = (event: FormEvent) => { event.preventDefault(); setMessage(null); mutation.mutate() }

  if (query.isError) return <ErrorState title="Your profile could not be loaded" onRetry={() => query.refetch()} />

  return (
    <div className="space-y-6">
      <PageHeader title="Patient profile" description="Keep your contact and address details accurate. Only you and authorized platform workflows can update this profile." />
      <div className="grid gap-6 lg:grid-cols-[1fr_320px]">
        <form onSubmit={submit} className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm sm:p-6">
          {query.isPending ? <div className="h-96 animate-pulse rounded-xl bg-gray-100" /> : (
            <div className="grid gap-5 sm:grid-cols-2">
              <Field label="First name" id="firstName" value={form.firstName} onChange={(value) => update("firstName", value)} required />
              <Field label="Last name" id="lastName" value={form.lastName} onChange={(value) => update("lastName", value)} required />
              <Field label="Phone number" id="phoneNumber" value={form.phoneNumber ?? ""} onChange={(value) => update("phoneNumber", value)} inputMode="tel" />
              <Field label="Emergency contact" id="emergencyContact" value={form.emergencyContact ?? ""} onChange={(value) => update("emergencyContact", value)} inputMode="tel" />
              <div className="sm:col-span-2"><Field label="Street address" id="street" value={form.street} onChange={(value) => update("street", value)} required /></div>
              <Field label="City or municipality" id="city" value={form.city} onChange={(value) => update("city", value)} required />
              <Field label="Country" id="country" value={form.country} onChange={(value) => update("country", value)} required />
              {message ? <p className={`sm:col-span-2 rounded-xl px-4 py-3 text-sm ${message.includes("successfully") ? "bg-emerald-50 text-emerald-800" : "bg-red-50 text-red-800"}`} role="status">{message}</p> : null}
              <div className="sm:col-span-2 flex justify-end"><Button type="submit" className="h-11 bg-brand-600 px-5 text-white" disabled={mutation.isPending}><Save /> {mutation.isPending ? "Saving…" : "Save changes"}</Button></div>
            </div>
          )}
        </form>
        <aside className="h-fit rounded-2xl border border-brand-100 bg-brand-50 p-5">
          <ShieldCheck className="size-7 text-brand-700" aria-hidden="true" />
          <h2 className="mt-3 font-Geist-Bold text-brand-950">Profile privacy</h2>
          <p className="mt-2 text-sm leading-6 text-brand-900">KLINIQ does not expose this patient profile through public clinic or practitioner search. Appointment access is enforced by backend ownership checks.</p>
          {query.data ? <dl className="mt-5 space-y-3 border-t border-brand-200 pt-4 text-sm"><div><dt className="text-brand-700">Date of birth</dt><dd className="mt-1 font-Geist-Semibold text-brand-950">{new Date(query.data.dateOfBirth).toLocaleDateString()}</dd></div><div><dt className="text-brand-700">Gender</dt><dd className="mt-1 font-Geist-Semibold text-brand-950">{query.data.gender}</dd></div></dl> : null}
        </aside>
      </div>
    </div>
  )
}

function Field({ label, id, value, onChange, required = false, inputMode }: { label: string; id: string; value: string; onChange: (value: string) => void; required?: boolean; inputMode?: "tel" }) {
  return <div><Label htmlFor={id} className="mb-2">{label}</Label><Input id={id} value={value} onChange={(event) => onChange(event.target.value)} required={required} inputMode={inputMode} className="h-11" /></div>
}
