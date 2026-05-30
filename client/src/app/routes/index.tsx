import { createBrowserRouter } from "react-router-dom"
import { patientRoutes } from "./patient.routes";
import { AppShellRoute } from "./appshell.route";
import { lazy, Suspense } from "react";
import { PublicRoute } from "./route.guard";
import { adminRoutes } from "./admin.routes";
import { practitionerRoutes } from "./practioner.routes";

// AUTH PAGES
const LoginPage = lazy(() => import("@features/auth/pages/LoginPage"));
const RegisterPage = lazy(() => import("@features/auth/pages/RegisterPage"));

// PUBLIC PAGES
const FindPractitionerPage = lazy(() => import("@features/patients/pages/FindPractitionerPage"));
const PractitionerDetailPage = lazy(() => import("@features/practitioners/pages/PractitionerDetailPage"));

const AccountRequestPage = lazy(() => import("@features/auth/pages/AccountRequestPage"));
const SetPasswordPage = lazy(() => import("@features/auth/pages/SetPasswordPage"));

const UnauthorizedPage = lazy(() => import("@shared/pages/UnauthorizedPage"));
const NotFoundPage = lazy(() => import("@shared/pages/NotFoundPage"));

// ROUTER
export const router = createBrowserRouter([
    {
        path: "/login",
        element: (
            <PublicRoute>
                <Suspense>
                    <LoginPage/>
                </Suspense>
            </PublicRoute>
        ),
    },
    {
        path: "/register",
        element: (
            <PublicRoute>
                <Suspense>
                    <RegisterPage/>
                </Suspense>
            </PublicRoute>
        ),
    },
    {
        path: "/practitioners",
        element: (
            <Suspense>
                <FindPractitionerPage/>
            </Suspense>
        ),
    },
    {
        path: "/practitioners/:id",
        element: (
            <Suspense>
                <PractitionerDetailPage/>
            </Suspense>
        ),
    },
    {   
        path: "apply",
        element: (
            <Suspense>
                <AccountRequestPage/>
            </Suspense>
        ),
    },
    {
        path: "set-password",
        element: (
            <Suspense>
                <SetPasswordPage/>
            </Suspense>
        ),
    },
    {
        path: "unauthorized",
        element: (
            <Suspense>
                <UnauthorizedPage/>
            </Suspense>
        ),  
    },
    {
        path: "/",
        element: <AppShellRoute/>,  
        children: [
            ...adminRoutes,
            ...patientRoutes,
            ...practitionerRoutes, 
        ],
    },
    {
        path: "*",
        element: (
            <Suspense>
                <NotFoundPage/>
            </Suspense>
        ),
    }
    
]);

