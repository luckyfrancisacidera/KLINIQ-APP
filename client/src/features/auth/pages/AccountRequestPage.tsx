import { useState, type FormEvent, type HTMLInputTypeAttribute, type ReactNode } from "react"
import { useMutation } from "@tanstack/react-query"
import { CheckCircle2, FileCheck2, LocateFixed, Loader2, Stethoscope, type LucideIcon } from "lucide-react"
import { Link } from "react-router-dom"
import { accountRequestApi } from "@shared/api/accountRequest.api"
import { PublicHeader } from "@shared/components/navigation/PublicHeader"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import type { SubmitAccountRequestPayload } from "@shared/types/accountRequest.types"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

type TextFields = Omit<SubmitAccountRequestPayload, "specializations" | "clinicLatitude" | "clinicLongitude" | "prcLicense" | "governmentId" | "professionalPhoto" | "cv">
const initialText: TextFields = { firstName: "", lastName: "", email: "", licenseNumber: "", street: "", city: "", country: "Philippines", clinicName: "" }

export default function AccountRequestPage() {
  const [text, setText] = useState(initialText)
  const [specializations, setSpecializations] = useState("")
  const [latitude, setLatitude] = useState("")
  const [longitude, setLongitude] = useState("")
  const [files, setFiles] = useState<Record<"prcLicense" | "governmentId" | "professionalPhoto" | "cv", File | null>>({ prcLicense: null, governmentId: null, professionalPhoto: null, cv: null })
  const [locationMessage, setLocationMessage] = useState<string | null>(null)
  const mutation = useMutation({ mutationFn: accountRequestApi.submit })

  const setTextField = (key: keyof TextFields, value: string) => setText((current) => ({ ...current, [key]: value }))
  const useLocation = () => {
    if (!navigator.geolocation) return setLocationMessage("Geolocation is unavailable. Enter coordinates manually.")
    setLocationMessage("Finding the clinic location…")
    navigator.geolocation.getCurrentPosition((position) => { setLatitude(String(position.coords.latitude)); setLongitude(String(position.coords.longitude)); setLocationMessage("Coordinates captured. Review them before submitting.") }, () => setLocationMessage("Location permission was denied or unavailable. Enter coordinates manually."), { timeout: 10_000, maximumAge: 60_000 })
  }
  const submit = (event: FormEvent) => {
    event.preventDefault()
    const parsedLatitude = Number(latitude)
    const parsedLongitude = Number(longitude)
    const specializationList = specializations.split(",").map((item) => item.trim()).filter(Boolean)
    if (!files.prcLicense || !files.governmentId || !files.professionalPhoto || !files.cv || !specializationList.length || !Number.isFinite(parsedLatitude) || !Number.isFinite(parsedLongitude)) return
    mutation.mutate({ ...text, specializations: specializationList, clinicLatitude: parsedLatitude, clinicLongitude: parsedLongitude, prcLicense: files.prcLicense, governmentId: files.governmentId, professionalPhoto: files.professionalPhoto, cv: files.cv })
  }

  return (
    <div className="min-h-screen bg-surface">
      <PublicHeader />
      <main className="mx-auto max-w-5xl px-4 py-10 sm:px-6 lg:px-8">
        {mutation.isSuccess ? <section className="rounded-2xl border border-emerald-200 bg-white p-8 text-center shadow-sm"><CheckCircle2 className="mx-auto size-12 text-emerald-600" /><h1 className="mt-4 font-Geist-ExtraBold text-3xl text-gray-950">Application submitted</h1><p className="mx-auto mt-3 max-w-xl text-sm leading-6 text-gray-600">An administrator can now review your credentials. You will receive an email if the application is approved or rejected.</p><Button asChild variant="outline" className="mt-6 h-11"><Link to="/login">Return to sign in</Link></Button></section> : <>
          <div className="max-w-3xl"><p className="text-sm font-Geist-Bold uppercase tracking-[0.18em] text-brand-600">Practitioner onboarding</p><h1 className="mt-3 font-Geist-ExtraBold text-3xl tracking-tight text-gray-950 sm:text-4xl">Apply to join KLINIQ</h1><p className="mt-3 text-base leading-7 text-gray-600">Submit professional, clinic, and credential information for administrator review. Uploaded documents are validated by the backend and should contain no passwords or unrelated patient data.</p></div>
          {mutation.isError ? <p className="mt-6 rounded-xl bg-red-50 px-4 py-3 text-sm text-red-800" role="alert">{getApiErrorMessage(mutation.error)}</p> : null}
          <form onSubmit={submit} className="mt-7 space-y-6">
            <Section title="Professional information" icon={Stethoscope}>
              <div className="grid gap-4 sm:grid-cols-2"><Field id="firstName" label="First name" value={text.firstName} onChange={(value) => setTextField("firstName", value)} /><Field id="lastName" label="Last name" value={text.lastName} onChange={(value) => setTextField("lastName", value)} /><Field id="email" label="Email address" type="email" value={text.email} onChange={(value) => setTextField("email", value)} /><Field id="licenseNumber" label="PRC license number" value={text.licenseNumber} onChange={(value) => setTextField("licenseNumber", value)} /><div className="sm:col-span-2"><Field id="specializations" label="Specializations" value={specializations} onChange={setSpecializations} placeholder="Family Medicine, Pediatrics" /><p className="mt-2 text-xs text-gray-500">Separate multiple specialties with commas.</p></div></div>
            </Section>
            <Section title="Clinic and location" icon={LocateFixed}>
              <div className="grid gap-4 sm:grid-cols-2"><div className="sm:col-span-2"><Field id="clinicName" label="Clinic name" value={text.clinicName} onChange={(value) => setTextField("clinicName", value)} /></div><div className="sm:col-span-2"><Field id="street" label="Street address" value={text.street} onChange={(value) => setTextField("street", value)} /></div><Field id="city" label="City or municipality" value={text.city} onChange={(value) => setTextField("city", value)} /><Field id="country" label="Country" value={text.country} onChange={(value) => setTextField("country", value)} /><Field id="latitude" label="Latitude" type="number" value={latitude} onChange={setLatitude} /><Field id="longitude" label="Longitude" type="number" value={longitude} onChange={setLongitude} /></div><Button type="button" variant="outline" className="mt-4 h-11" onClick={useLocation}><LocateFixed /> Use current location</Button>{locationMessage ? <p className="mt-3 text-sm text-gray-600" role="status">{locationMessage}</p> : null}
            </Section>
            <Section title="Verification documents" icon={FileCheck2}>
              <div className="grid gap-4 sm:grid-cols-2"><FileField id="prcLicense" label="PRC license" onChange={(file) => setFiles((current) => ({ ...current, prcLicense: file }))} /><FileField id="governmentId" label="Government ID" onChange={(file) => setFiles((current) => ({ ...current, governmentId: file }))} /><FileField id="professionalPhoto" label="Professional photo" accept="image/png,image/jpeg" onChange={(file) => setFiles((current) => ({ ...current, professionalPhoto: file }))} /><FileField id="cv" label="Curriculum vitae" onChange={(file) => setFiles((current) => ({ ...current, cv: file }))} /></div><p className="mt-4 text-xs leading-5 text-gray-500">Use PDF, PNG, or JPEG as appropriate. The server applies file size, signature, and content-type checks before storage.</p>
            </Section>
            <div className="flex justify-end"><Button type="submit" className="h-12 bg-brand-600 px-6 text-white" disabled={mutation.isPending}>{mutation.isPending ? <><Loader2 className="animate-spin" /> Submitting…</> : "Submit application"}</Button></div>
          </form>
        </>}
      </main>
    </div>
  )
}

function Section({ title, icon: Icon, children }: { title: string; icon: LucideIcon; children: ReactNode }) { return <section className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm sm:p-6"><div className="mb-5 flex items-center gap-3"><span className="grid size-10 place-items-center rounded-xl bg-brand-50 text-brand-700"><Icon className="size-5" /></span><h2 className="font-Geist-Bold text-lg text-gray-950">{title}</h2></div>{children}</section> }
function Field({ id, label, value, onChange, type = "text", placeholder }: { id: string; label: string; value: string; onChange: (value: string) => void; type?: HTMLInputTypeAttribute; placeholder?: string }) { return <div><Label htmlFor={id} className="mb-2">{label}</Label><Input id={id} type={type} step={type === "number" ? "any" : undefined} value={value} onChange={(event) => onChange(event.target.value)} placeholder={placeholder} className="h-11" required /></div> }
function FileField({ id, label, accept = "application/pdf,image/png,image/jpeg", onChange }: { id: string; label: string; accept?: string; onChange: (file: File | null) => void }) { return <div><Label htmlFor={id} className="mb-2">{label}</Label><Input id={id} type="file" accept={accept} onChange={(event) => onChange(event.target.files?.[0] ?? null)} className="h-11" required /></div> }
