import { useState } from "react"
import { useMutation } from "@tanstack/react-query"
import { AlertTriangle, ArrowRight, BrainCircuit, Loader2, MapPin, ShieldCheck, Sparkles, Stethoscope } from "lucide-react"
import { Link } from "react-router-dom"
import { symptomSearchApi } from "@shared/api/symptom-search.api"
import { Pagination } from "@shared/components/data/Pagination"
import { PublicHeader } from "@shared/components/navigation/PublicHeader"
import { Button } from "@shared/components/ui/button"
import { Label } from "@shared/components/ui/label"
import { Textarea } from "@shared/components/ui/textarea"
import { useNetworkStatus } from "@shared/hooks/useNetworkStatus"
import type { SymptomUrgency } from "@shared/types/symptom-search.types"
import { getApiErrorMessage } from "@shared/utils/api.error.utils"

const fuzzySignalPrefix = "Did you mean"

const urgencyStyles: Record<SymptomUrgency, string> = {
  Routine: "border-brand-200 bg-brand-50 text-brand-950",
  Urgent: "border-amber-300 bg-amber-50 text-amber-950",
  Emergency: "border-red-300 bg-red-50 text-red-950",
}

export default function SymptomSearchPage() {
  const [symptoms, setSymptoms] = useState("")
  const [submittedSymptoms, setSubmittedSymptoms] = useState("")
  const online = useNetworkStatus()

  const search = useMutation({
    mutationFn: ({ text, page }: { text: string; page: number }) => symptomSearchApi.search({ symptoms: text, page, pageSize: 6 }).then(({ data }) => data),
  })

  const runSearch = (page = 1) => {
    const text = page === 1 ? symptoms.trim() : submittedSymptoms
    if (text.length < 10 || !online) return
    if (page === 1) setSubmittedSymptoms(text)
    search.mutate({ text, page })
  }

  const result = search.data

  return (
    <div className="min-h-screen bg-surface">
      <PublicHeader />
      <main>
        <section className="border-b border-gray-200 bg-white">
          <div className="mx-auto grid max-w-6xl gap-8 px-4 py-10 sm:px-6 lg:grid-cols-[1.15fr_.85fr] lg:px-8 lg:py-14">
            <div>
              <p className="flex items-center gap-2 text-sm font-Geist-Bold uppercase tracking-[0.16em] text-brand-600"><Sparkles className="size-4" /> AI-assisted care search</p>
              <h1 className="mt-3 max-w-3xl font-Geist-ExtraBold text-3xl tracking-tight text-gray-950 sm:text-5xl">Describe what you are feeling. Find the right type of physician.</h1>
              <p className="mt-4 max-w-2xl text-base leading-7 text-gray-600">KLINIQ analyzes symptom keywords, checks for urgent warning signs, suggests relevant medical specialties, and finds matching verified practitioners.</p>
            </div>
            <aside className="rounded-2xl border border-brand-100 bg-brand-50 p-5">
              <ShieldCheck className="size-8 text-brand-700" />
              <h2 className="mt-3 font-Geist-Bold text-lg text-brand-950">Privacy and safety first</h2>
              <p className="mt-2 text-sm leading-6 text-brand-900">Your description is used only to produce this response. It is not saved to your profile or written to application logs. This tool suggests where to start; it does not diagnose or replace a clinician.</p>
            </aside>
          </div>
        </section>

        <section className="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
          <div className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm sm:p-7">
            <div className="flex items-start gap-3">
              <div className="grid size-11 shrink-0 place-items-center rounded-xl bg-brand-50 text-brand-700"><BrainCircuit className="size-6" /></div>
              <div><h2 className="font-Geist-Bold text-xl text-gray-950">Tell KLINIQ about your symptoms</h2><p className="mt-1 text-sm text-gray-600">Include where you feel it, how long it has lasted, severity, and any related symptoms. Do not include your name, address, or account details.</p></div>
            </div>
            <div className="mt-6">
              <Label htmlFor="symptom-description" className="mb-2 block">Symptom description</Label>
              <Textarea id="symptom-description" className="min-h-40 resize-y p-4 leading-6" maxLength={1500} value={symptoms} onChange={(event) => setSymptoms(event.target.value)} placeholder="Example: I have had an itchy red rash on both arms for three days. It is spreading slowly, but I do not have trouble breathing or facial swelling." aria-describedby="symptom-help" />
              <div id="symptom-help" className="mt-2 flex justify-between gap-4 text-xs text-gray-500"><span>Minimum 10 characters. Maximum 1,500.</span><span>{symptoms.length}/1500</span></div>
            </div>
            {!online ? <p className="mt-4 rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-900" role="status">Reconnect to the internet to use symptom-assisted search.</p> : null}
            {search.isError ? <p className="mt-4 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800" role="alert">{getApiErrorMessage(search.error)}</p> : null}
            <Button type="button" className="mt-5 h-12 bg-brand-600 px-6 text-white" disabled={!online || symptoms.trim().length < 10 || search.isPending} onClick={() => runSearch()}>{search.isPending ? <><Loader2 className="animate-spin" /> Matching care…</> : <><Sparkles /> Suggest physicians</>}</Button>
          </div>

          {result ? (
            <div className="mt-8 space-y-6" aria-live="polite">
              <section className={`rounded-2xl border p-5 ${urgencyStyles[result.urgency]}`}>
                <div className="flex items-start gap-3">
                  {result.urgency === "Emergency" ? <AlertTriangle className="mt-0.5 size-6 shrink-0" /> : <ShieldCheck className="mt-0.5 size-6 shrink-0" />}
                  <div><p className="font-Geist-Bold">{result.urgency} guidance</p><p className="mt-1 text-sm leading-6">{result.guidance}</p></div>
                </div>
              </section>

              <section>
                <h2 className="font-Geist-Bold text-2xl text-gray-950">Suggested specialties</h2>
                <p className="mt-1 text-sm text-gray-600">Matches are explainable and based on signals found in your description.</p>
                <div className="mt-4 grid gap-4 md:grid-cols-3">
                  {result.suggestedSpecialties.map((item) => (
                    <article key={item.specialty} className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
                      <div className="flex items-center justify-between gap-3"><Stethoscope className="size-6 text-brand-600" /><span className="rounded-full bg-brand-50 px-2.5 py-1 text-xs font-Geist-Semibold text-brand-800">{item.matchScore}% match</span></div>
                      <h3 className="mt-4 font-Geist-Bold text-lg text-gray-950">{item.specialty}</h3>
                      <p className="mt-3 text-xs font-Geist-Semibold uppercase tracking-wide text-gray-500">Matched details</p>
                      <div className="mt-2 flex flex-wrap gap-2">{item.matchedSignals.map((signal) => {
                        const isFuzzy = signal.startsWith(fuzzySignalPrefix)
                        return <span key={signal} className={isFuzzy ? "rounded-full border border-amber-300 bg-amber-50 px-2.5 py-1 text-xs text-amber-900" : "rounded-full bg-gray-100 px-2.5 py-1 text-xs text-gray-700"}>{signal}</span>
                      })}</div>
                    </article>
                  ))}
                </div>
              </section>

              {result.urgency !== "Emergency" ? (
                <section>
                  <div className="flex flex-col gap-2 sm:flex-row sm:items-end sm:justify-between"><div><h2 className="font-Geist-Bold text-2xl text-gray-950">Matching physicians</h2><p className="mt-1 text-sm text-gray-600">Review each physician’s profile and available schedule before booking.</p></div><Link to={`/practitioners?specialization=${encodeURIComponent(result.suggestedSpecialties[0]?.specialty ?? "")}`} className="text-sm font-Geist-Semibold text-brand-700 hover:text-brand-900">Browse the full directory</Link></div>
                  {result.practitioners.items.length === 0 ? <div className="mt-4 rounded-2xl border border-gray-200 bg-white p-6"><p className="font-Geist-Bold text-gray-950">No exact physician match is listed yet.</p><p className="mt-2 text-sm text-gray-600">Try the suggested specialty in the provider directory or start with a general practitioner.</p></div> : <div className="mt-4 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">{result.practitioners.items.map((practitioner) => <article key={practitioner.id} className="flex flex-col rounded-2xl border border-gray-200 bg-white p-5 shadow-sm"><div className="flex items-start justify-between gap-3"><div className="grid size-11 place-items-center rounded-xl bg-brand-50 text-brand-700"><Stethoscope className="size-6" /></div><span className="rounded-full bg-emerald-50 px-2.5 py-1 text-xs font-Geist-Semibold text-emerald-800">{practitioner.matchScore}% match</span></div><h3 className="mt-4 font-Geist-Bold text-lg text-gray-950">Dr. {practitioner.firstName} {practitioner.lastName}</h3><p className="mt-1 text-xs text-gray-500">License {practitioner.licenseNumber}</p><div className="mt-3 flex flex-wrap gap-2">{practitioner.specializations.map((specialty) => <span key={specialty} className="rounded-full bg-gray-100 px-2.5 py-1 text-xs text-gray-700">{specialty}</span>)}</div><p className="mt-4 flex items-start gap-2 text-sm text-gray-600"><MapPin className="mt-0.5 size-4 shrink-0 text-brand-600" />{practitioner.clinicName ?? "Clinic assignment pending"}</p><Button asChild className="mt-5 h-11 bg-brand-600 text-white"><Link to={`/practitioners/${practitioner.id}`}>View physician <ArrowRight /></Link></Button></article>)}</div>}
                  {result.practitioners.totalPages > 1 ? <div className="mt-7"><Pagination page={result.practitioners.page} totalPages={result.practitioners.totalPages} hasPreviousPage={result.practitioners.hasPreviousPage} hasNextPage={result.practitioners.hasNextPage} isLoading={search.isPending} onPageChange={runSearch} /></div> : null}
                </section>
              ) : null}

              <p className="rounded-xl bg-gray-100 px-4 py-3 text-xs leading-5 text-gray-600">{result.disclaimer}</p>
            </div>
          ) : null}
        </section>
      </main>
    </div>
  )
}
