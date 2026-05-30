import {lazy} from "react";
import { Outlet, type RouteObject} from "react-router-dom";
import {RequireAuth, RequireRouteAccess} from "./route.guard";

const PatientDashboardPage = lazy(() => import("@features/patients/pages/PatientDashboardPage"));
const BookAppointmentPage = lazy(() => import("@features/patients/pages/BookAppointmentPage"));
const PatientAppointmentsPage = lazy(() => import("@features/patients/pages/PatientAppointmentsPage"));
const PatientProfilePage = lazy(() => import("@features/patients/pages/PatientProfilePage"));
const SettingsPage = lazy(() => import("@shared/pages/SettingsPage"));

const PatientRouteLayout = () => (
    <RequireAuth>
        <Outlet/>
    </RequireAuth>
);

export const patientRoutes : RouteObject[] = [
    {
        path: "patient",
        element: <PatientRouteLayout/>,
        children: [
            {
                path: "dashboard",
                element: (
                    <RequireRouteAccess routeKey="patientDashboard">
                        <PatientDashboardPage/>
                    </RequireRouteAccess>
                ),
            },
            {
                path: "book",
                element: (
                    <RequireRouteAccess routeKey="bookAppointment">
                        <BookAppointmentPage/>
                    </RequireRouteAccess>
                ),
            },
            {
                path: "appointments",
                element: (
                    <RequireRouteAccess routeKey="patientAppointments">
                        <PatientAppointmentsPage/>
                    </RequireRouteAccess>
                ),
            },
            {
                path: "profile",
                element: (
                    <RequireRouteAccess routeKey="patientProfile">
                        <PatientProfilePage/>
                    </RequireRouteAccess>
                ),
            },
            {
                path: "settings",
                element: (
                    <RequireRouteAccess routeKey="settings">
                        <SettingsPage/>
                    </RequireRouteAccess>
                ),
            },
        ],
    },
];