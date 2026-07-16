import { SearchX } from "lucide-react"
import { Link } from "react-router-dom"
import { PublicHeader } from "@shared/components/navigation/PublicHeader"
import { Button } from "@shared/components/ui/button"

export default function NotFoundPage() {
  return <div className="min-h-screen bg-surface"><PublicHeader /><main className="mx-auto max-w-xl px-4 py-24 text-center"><SearchX className="mx-auto size-12 text-gray-400" /><p className="mt-5 text-sm font-Geist-Bold uppercase tracking-widest text-brand-600">404</p><h1 className="mt-2 font-Geist-ExtraBold text-3xl text-gray-950">Page not found</h1><p className="mt-3 text-sm leading-6 text-gray-600">The page may have moved or the address may be incorrect.</p><Button asChild className="mt-6 h-11 bg-brand-600 px-5 text-white"><Link to="/clinics">Find clinics</Link></Button></main></div>
}
