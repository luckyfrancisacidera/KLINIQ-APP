import { lazy, Suspense, type ReactNode } from "react"
import { Navigate, createBrowserRouter } from "react-router-dom"
import { FullPageLoader } from "@shared/components/feedback/FullPageLoader"
import { PublicRoute } from "./route.guard"
import { AppShellRoute } from "./appshell.route"
import { adminRoutes } from "./admin.routes"
import { patientRoutes } from "./patient.routes"
import { practitionerRoutes } from "./practitioner.routes"

const LoginPage = lazy(() => import("@features/auth/pages/LoginPage"))
const RegisterPage = lazy(() => import("@features/auth/pages/RegisterPage"))
const AccountRequestPage = lazy(() => import("@features/auth/pages/AccountRequestPage"))
const SetPasswordPage = lazy(() => import("@features/auth/pages/SetPasswordPage"))
const ForgotPasswordPage = lazy(() => import("@features/auth/pages/ForgotPasswordPage"))
const ResetPasswordPage = lazy(() => import("@features/auth/pages/ResetPasswordPage"))
const ClinicDiscoveryPage = lazy(() => import("@features/clinics/pages/ClinicDiscoveryPage"))
const ClinicDetailPage = lazy(() => import("@features/clinics/pages/ClinicDetailPage"))
const FindPractitionerPage = lazy(() => import("@features/patients/pages/FindPractitionerPage"))
const PractitionerDetailPage = lazy(() => import("@features/practitioners/pages/PractitionerDetailPage"))
const SymptomSearchPage = lazy(() => import("@features/symptom-search/pages/SymptomSearchPage"))
const UnauthorizedPage = lazy(() => import("@shared/pages/UnauthorizedPage"))
const NotFoundPage = lazy(() => import("@shared/pages/NotFoundPage"))

const withSuspense = (element: ReactNode) => (
  <Suspense fallback={<FullPageLoader label="Loading KLINIQ…" />}>{element}</Suspense>
)

export const router = createBrowserRouter([
  { path: "/", element: <Navigate to="/clinics" replace /> },
  {
    path: "/login",
    element: <PublicRoute>{withSuspense(<LoginPage />)}</PublicRoute>,
  },
  {
    path: "/register",
    element: <PublicRoute>{withSuspense(<RegisterPage />)}</PublicRoute>,
  },
  { path: "/apply", element: withSuspense(<AccountRequestPage />) },
  { path: "/set-password", element: withSuspense(<SetPasswordPage />) },
  { path: "/forgot-password", element: <PublicRoute>{withSuspense(<ForgotPasswordPage />)}</PublicRoute> },
  { path: "/reset-password", element: <PublicRoute>{withSuspense(<ResetPasswordPage />)}</PublicRoute> },
  { path: "/clinics", element: withSuspense(<ClinicDiscoveryPage />) },
  { path: "/clinics/:id", element: withSuspense(<ClinicDetailPage />) },
  { path: "/practitioners", element: withSuspense(<FindPractitionerPage />) },
  { path: "/symptom-search", element: withSuspense(<SymptomSearchPage />) },
  { path: "/practitioners/:id", element: withSuspense(<PractitionerDetailPage />) },
  { path: "/unauthorized", element: withSuspense(<UnauthorizedPage />) },
  {
    element: <AppShellRoute />,
    children: [...adminRoutes, ...patientRoutes, ...practitionerRoutes],
  },
  { path: "*", element: withSuspense(<NotFoundPage />) },
])
