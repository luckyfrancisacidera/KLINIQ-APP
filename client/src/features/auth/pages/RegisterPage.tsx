import { useState, type ChangeEvent } from "react"
import { Link, useNavigate } from "react-router-dom"
import { useMutation } from "@tanstack/react-query"
import { Eye, EyeOff, Loader2, Check, CalendarIcon } from "lucide-react"
import { format } from "date-fns"
import { Button } from "@shared/components/ui/button"
import { Input } from "@shared/components/ui/input"
import { Label } from "@shared/components/ui/label"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@shared/components/ui/select"
import { Popover, PopoverContent, PopoverTrigger } from "@shared/components/ui/popover"
import { Calendar } from "@shared/components/ui/calendar"
import { cn } from "@shared/lib/utils"
import { authApi } from "@shared/api/auth.api"
import { Gender, type RegisterPayload } from "@shared/types/auth.types"
import { useAuth } from "@app/providers/AuthProviders"
import { type UserRole } from "@app/providers/auth.provider.type"
import { isValidEmail } from "@/shared/utils/validation.utils"
import { getApiErrorMessage } from "@/shared/utils/api.error.utils"


type FormData = Omit<RegisterPayload, "gender"> & { gender: string }

const initialForm: FormData = {
  email: "",
  password: "",
  confirmPassword: "",
  firstName: "",
  lastName: "",
  dateOfBirth: "",
  gender: "",
  street: "",
  city: "",
  country: "",
  phoneNumber: "",
  emergencyContact: "",
}

const steps = ["Account", "Personal", "Address"]

const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9])/

const StepIndicator = ({ current }: { current: number }) => (
  <div className="flex items-center mb-8">
    {steps.map((label, i) => (
      <div key={label} className="flex items-center flex-1 last:flex-none">
        <div className="flex flex-col items-center">
          <div className={`w-8 h-8 rounded-full flex items-center justify-center text-xs font-Geist-Semibold transition-colors
            ${i < current ? "bg-brand-500 text-white" :
              i === current ? "bg-brand-500 text-white ring-4 ring-brand-100" :
              "bg-gray-100 text-gray-400"}`}>
            {i < current ? <Check className="w-3.5 h-3.5" /> : i + 1}
          </div>
          <span className={`mt-1.5 text-xs font-Geist-Regular ${i <= current ? "text-brand-600" : "text-gray-400"}`}>
            {label}
          </span>
        </div>
        {i < steps.length - 1 && (
          <div className={`flex-1 h-px mx-2 mb-5 transition-colors ${i < current ? "bg-brand-400" : "bg-gray-200"}`} />
        )}
      </div>
    ))}
  </div>
)

const RegisterPage = () => {
  const navigate = useNavigate()
  const { setUser } = useAuth()

  const [step, setStep] = useState(0)
  const [form, setForm] = useState<FormData>(initialForm)
  const [showPassword, setShowPassword] = useState(false)
  const [showConfirm, setShowConfirm] = useState(false)
  const [fieldError, setFieldError] = useState<string | null>(null)
  const [dobOpen, setDobOpen] = useState(false)

  const set = (field: keyof FormData) => (e: ChangeEvent<HTMLInputElement>) =>
    setForm(f => ({ ...f, [field]: e.target.value }))

  const { mutate: register, isPending, error: apiError } = useMutation({
    mutationFn: (payload: RegisterPayload) => authApi.register(payload),
    onSuccess: ({ data }) => {
      setUser({ userId: data.userId, email: data.email, role: data.role as UserRole })
      navigate("/patient/appointments", { replace: true })
    },
  })

  const validateStep = (): boolean => {
    setFieldError(null)
    if (step === 0) {
      if (!isValidEmail(form.email)) {
        setFieldError("Please enter a valid email address."); return false
      }
      if (!form.email || !form.password || !form.confirmPassword) {
        setFieldError("Please fill in all fields."); return false
      }
      if (!passwordRegex.test(form.password)) {
        setFieldError("Password must include uppercase, lowercase, a number, and a special character."); return false
      }
      if (form.password.length < 8) {
        setFieldError("Password must be at least 8 characters."); return false
      }
      if (form.password !== form.confirmPassword) {
        setFieldError("Passwords do not match."); return false
      }
    }
    if (step === 1) {
      if (!form.firstName || !form.lastName || !form.dateOfBirth || !form.gender) {
        setFieldError("Please fill in all required fields."); return false
      }
    }
    if (step === 2) {
      if (!form.street || !form.city || !form.country || !form.phoneNumber) {
        setFieldError("Please fill in all required fields."); return false
      }
    }
    return true
  }

  const next = () => { if (validateStep()) setStep(s => s + 1) }
  const back = () => { setFieldError(null); setStep(s => s - 1) }
  const submit = () => {
    if (!validateStep()) return
    register({ ...form, gender: Number(form.gender) as Gender })
  }

  const inputClass = "h-11 border-gray-200 focus-visible:ring-brand-500 focus-visible:border-brand-500"
  const labelClass = "text-sm font-Geist-Semibold text-gray-700"

  return (
    <div className="min-h-screen bg-surface flex flex-col">
      <div className="h-1 bg-brand-500 w-full" />
      <div className="flex flex-1 flex-col lg:flex-row">

        {/* Left panel */}
        <div className="hidden lg:flex lg:w-[50%] bg-brand-700 flex-col justify-between p-12 relative overflow-hidden">
          <div className="absolute -top-20 -right-20 w-80 h-80 rounded-full bg-brand-600 opacity-40" />
          <div className="absolute -bottom-24 -left-12 w-72 h-72 rounded-full bg-brand-800 opacity-60" />

          <div className="relative z-10">
            <div className="flex items-center gap-3 mb-16">
              <div className="w-9 h-9 rounded-lg bg-white flex items-center justify-center">
               <img src="/logo.png" alt="Kliniq Logo" className="w-10 h-10 object-contain"/>
              </div>
              <span className="text-white font-Geist-Bold text-xl tracking-tight">Kliniq</span>
            </div>
            <h1 className="text-white font-Geist-Bold text-5xl leading-tight mb-4">
              Your health journey<br />
              <span className="text-brand-300">starts here.</span>
            </h1>
            <p className="font-Geist-Regular text-brand-200 text-lg leading-relaxed max-w-xs">
              Create your patient account in minutes and get access to verified practitioners near you.
            </p>
          </div>

          <div className="relative z-10 space-y-3">
            {steps.map((label, i) => (
              <div key={label} className={`flex items-center gap-3 transition-opacity ${i === step ? "opacity-100" : "opacity-40"}`}>
                <div className={`w-6 h-6 rounded-full flex items-center justify-center text-xs font-Geist-Bold
                  ${i < step ? "bg-brand-300 text-brand-800" :
                    i === step ? "bg-white text-brand-700" :
                    "bg-brand-600 text-brand-300"}`}>
                  {i < step ? <Check className="w-3 h-3" /> : i + 1}
                </div>
                <span className="text-brand-100 text-sm font-Geist-Regular">{label}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Right panel */}
        <div className="flex-1 flex items-center justify-center px-6 py-12 lg:px-16">
          <div className="w-full max-w-md">

            <div className="flex items-center gap-2 mb-8 lg:hidden">
              <div className="w-8 h-8 rounded-lg bg-brand-500 flex items-center justify-center">
               <img src="/logo.png" alt="Kliniq Logo" className="w-10 h-10 object-contain"/>
              </div>
              <span className="font-Geist-Bold text-lg text-brand-700 tracking-tight">Kliniq</span>
            </div>

            <div className="mb-6">
              <h2 className="font-Geist-Bold text-2xl text-gray-900 mb-1">Create your account</h2>
              <p className="font-Geist-Regular text-muted text-sm">Step {step + 1} of {steps.length}  -  {steps[step]}</p>
            </div>

            <StepIndicator current={step} />

            {(fieldError ?? apiError) && (
              <div className="font-Geist-Regular mb-5 rounded-lg bg-red-50 border border-red-200 px-4 py-3">
                <p className="text-red-700 text-sm">
                  {fieldError ?? getApiErrorMessage(apiError)}
                </p>
              </div>
            )}

            {/* Step 0 — Account */}
            {step === 0 && (
              <div className="font-Geist-Regular space-y-4">
                <div className="space-y-1.5">
                  <Label htmlFor="email" className={labelClass}>Email address</Label>
                  <Input id="email" type="email" autoComplete="email"
                    placeholder="you@example.com" value={form.email}
                    onChange={set("email")} className={inputClass} />
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="password" className={labelClass}>Password</Label>
                  <div className="relative">
                    <Input id="password" type={showPassword ? "text" : "password"}
                      placeholder="Min. 8 chars, uppercase, number, symbol"
                      value={form.password} onChange={set("password")}
                      className={`${inputClass} pr-10`} />
                    <button type="button" onClick={() => setShowPassword(p => !p)}
                      className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                      aria-label="Toggle password">
                      {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                    </button>
                  </div>
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="confirmPassword" className={labelClass}>Confirm password</Label>
                  <div className="relative">
                    <Input id="confirmPassword" type={showConfirm ? "text" : "password"}
                      placeholder="Re-enter your password"
                      value={form.confirmPassword} onChange={set("confirmPassword")}
                      className={`${inputClass} pr-10`} />
                    <button type="button" onClick={() => setShowConfirm(p => !p)}
                      className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                      aria-label="Toggle confirm password">
                      {showConfirm ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                    </button>
                  </div>
                </div>
              </div>
            )}

            {/* Step 1 — Personal */}
            {step === 1 && (
              <div className="space-y-4">
                <div className="grid grid-cols-2 gap-3">
                  <div className="space-y-1.5">
                    <Label htmlFor="firstName" className={labelClass}>First name</Label>
                    <Input id="firstName" placeholder="Juan" value={form.firstName}
                      onChange={set("firstName")} className={inputClass} />
                  </div>
                  <div className="space-y-1.5">
                    <Label htmlFor="lastName" className={labelClass}>Last name</Label>
                    <Input id="lastName" placeholder="dela Cruz" value={form.lastName}
                      onChange={set("lastName")} className={inputClass} />
                  </div>
                </div>

                <div className="space-y-1.5">
                  <Label className={labelClass}>Date of birth</Label>
                  <Popover open={dobOpen} onOpenChange={setDobOpen}>
                    <PopoverTrigger asChild>
                      <Button
                        variant="outline"
                        className={cn(
                          "w-full h-11 justify-start text-left font-normal border-gray-200 hover:bg-gray-50",
                          !form.dateOfBirth && "text-gray-400"
                        )}
                      >
                        <CalendarIcon className="mr-2 h-4 w-4 text-gray-400" />
                        {form.dateOfBirth
                          ? format(new Date(form.dateOfBirth), "MMMM d, yyyy")
                          : "Pick your date of birth"}
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="w-auto p-0" align="start">
                      <Calendar
                        mode="single"
                        selected={form.dateOfBirth ? new Date(form.dateOfBirth) : undefined}
                        onSelect={(date) => {
                          setForm(f => ({
                            ...f,
                            dateOfBirth: date ? format(date, "yyyy-MM-dd") : ""
                          }))
                          setDobOpen(false)
                        }}
                        captionLayout="dropdown"
                        startMonth={new Date(1900, 0)}
                        endMonth={new Date()}
                        disabled={(date) => date > new Date()}
                      />
                    </PopoverContent>
                  </Popover>
                </div>

                <div className="space-y-1.5">
                  <Label className={labelClass}>Gender</Label>
                  <Select value={form.gender} onValueChange={v => setForm(f => ({ ...f, gender: v }))}>
                    <SelectTrigger className="h-11 border-gray-200 focus:ring-brand-500">
                      <SelectValue placeholder="Select gender" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value={String(Gender.Male)}>Male</SelectItem>
                      <SelectItem value={String(Gender.Female)}>Female</SelectItem>
                      <SelectItem value={String(Gender.Other)}>Other</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>
            )}

            {/* Step 2 — Address */}
            {step === 2 && (
              <div className="space-y-4">
                <div className="space-y-1.5">
                  <Label htmlFor="street" className={labelClass}>Street address</Label>
                  <Input id="street" placeholder="123 Rizal St." value={form.street}
                    onChange={set("street")} className={inputClass} />
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <div className="space-y-1.5">
                    <Label htmlFor="city" className={labelClass}>City</Label>
                    <Input id="city" placeholder="Laoag" value={form.city}
                      onChange={set("city")} className={inputClass} />
                  </div>
                  <div className="space-y-1.5">
                    <Label htmlFor="country" className={labelClass}>Country</Label>
                    <Input id="country" placeholder="Philippines" value={form.country}
                      onChange={set("country")} className={inputClass} />
                  </div>
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="phone" className={labelClass}>Phone number</Label>
                  <Input id="phone" type="tel" placeholder="+63 912 345 6789"
                    value={form.phoneNumber} onChange={set("phoneNumber")} className={inputClass} />
                </div>
                <div className="space-y-1.5">
                  <Label htmlFor="emergency" className={labelClass}>
                    Emergency contact{" "}
                    <span className="text-gray-400 font-Geist-Regular">(optional)</span>
                  </Label>
                  <Input id="emergency" placeholder="+63 912 000 0000"
                    value={form.emergencyContact} onChange={set("emergencyContact")}
                    className={inputClass} />
                </div>
              </div>
            )}

            {/* Navigation */}
            <div className="flex gap-3 mt-8">
              {step > 0 && (
                <Button type="button" variant="outline" onClick={back}
                  className="flex-1 h-11 border-gray-200 text-gray-700 hover:bg-gray-50">
                  Back
                </Button>
              )}
              {step < steps.length - 1 ? (
                <Button type="button" onClick={next}
                  className="font-Geist-Semibold flex-1 h-11 bg-brand-500 hover:bg-brand-600 text-white">
                  Continue
                </Button>
              ) : (
                <Button type="button" onClick={submit} disabled={isPending}
                  className="font-Geist-Semibold flex-1 h-11 bg-brand-500 hover:bg-brand-600 text-white">
                  {isPending
                    ? <span className="flex items-center gap-2"><Loader2 className="w-4 h-4 animate-spin" />Creating account…</span>
                    : "Create account"}
                </Button>
              )}
            </div>

            <p className="font-Geist-Regular mt-6 text-center text-sm text-muted">
              Already have an account?{" "}
              <Link to="/login" className="text-brand-600 hover:text-brand-700 font-Geist-Semibold">Sign in</Link>
            </p>

          </div>
        </div>
      </div>
    </div>
  )
}

export default RegisterPage