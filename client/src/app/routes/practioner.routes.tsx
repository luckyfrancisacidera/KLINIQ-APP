// PRACTITIONER ROUTES
import {lazy} from "react";
import { Outlet, type RouteObject} from "react-router-dom";
import {RequireAuth, RequireRouteAccess} from "./route.guard";

const PractitionerDashboardPage = lazy(() => import("@features/practitioners/pages/PractitionerDashboardPage"));
const PractitionerAppointmentsPage = lazy(() => import("@features/practitioners/pages/PractitionerAppointmentsPage"));
const ScheduleManagementPage = lazy(() => import("@features/practitioners/pages/ScheduleManagementPage"));
const PractitionerProfilePage = lazy(() => import("@features/practitioners/pages/PractitionerProfilePage"));
const SettingsPage = lazy(() => import("@shared/pages/SettingsPage"));

const PractitionerRouteLayout = () => (
    <RequireAuth>
        <Outlet/>
    </RequireAuth>
);

export const practitionerRoutes : RouteObject[] = [
    {
        path : "practitioner",
        element: <PractitionerRouteLayout/>,
        children: [
            {
                path: "dashboard",
                element: (
                    <RequireRouteAccess routeKey="practitionerDashboard">
                        <PractitionerDashboardPage/>
                    </RequireRouteAccess>
                ),
            },
            {
                path: "appointments",
                element: (
                    <RequireRouteAccess routeKey="practitionerAppointments">
                        <PractitionerAppointmentsPage/>
                    </RequireRouteAccess>
                ),
            },
            {
                path: "schedule",
                element: (
                    <RequireRouteAccess routeKey="scheduleManagement">
                        <ScheduleManagementPage/>
                    </RequireRouteAccess>
                ),
            },
            {
                path: "profile",
                element: (
                    <RequireRouteAccess routeKey="practitionerProfile">
                        <PractitionerProfilePage/>
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
        ]
    }
]