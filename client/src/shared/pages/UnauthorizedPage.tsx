import { ShieldX } from "lucide-react"
import { Link } from "react-router-dom"
import { Button } from "@shared/components/ui/button"

export default function UnauthorizedPage() {
  return <main className="grid min-h-screen place-items-center bg-surface px-4"><section className="max-w-lg rounded-2xl border border-gray-200 bg-white p-8 text-center shadow-sm"><ShieldX className="mx-auto size-12 text-red-600" /><h1 className="mt-4 font-Geist-ExtraBold text-3xl text-gray-950">Access not permitted</h1><p className="mt-3 text-sm leading-6 text-gray-600">You are signed in, but your role does not have permission to access this page. Backend authorization remains the final enforcement layer.</p><Button asChild variant="outline" className="mt-6 h-11"><Link to="/clinics">Return to clinic search</Link></Button></section></main>
}
