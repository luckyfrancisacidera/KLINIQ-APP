import {lazy} from "react";
import { Outlet, type RouteObject} from "react-router-dom";
import {RequireAuth, RequireRouteAccess} from "./route.guard";

const AdminDashboardPage = lazy(() => import("@features/admin/pages/AdminDashboardPage"));
const PatientManagementPage = lazy(() => import("@features/admin/pages/PatientManagementPage"));
const PractitionerManagementPage = lazy(() => import("@features/admin/pages/PractitionerManagementPage"));
const AccountRequestsPage = lazy(() => import("@features/admin/pages/AccountRequestsPage"));

const AdminRouteLayout = () => (
     <RequireAuth>
        <Outlet/>
    </RequireAuth>
);

export const adminRoutes : RouteObject[] = [
    {
        path: "admin",
        element: <AdminRouteLayout/>,
        children: [
            {
                path: "dashboard",
                element: (
                    <RequireRouteAccess routeKey="adminDashboard">
                        <AdminDashboardPage/>
                    </RequireRouteAccess>
                )
            },
            {
                path: "patients",
                element: (
                    <RequireRouteAccess routeKey="patientManagement">
                        <PatientManagementPage/>
                    </RequireRouteAccess>
                )
            },
            {
                path: "practitioners",
                element: (
                    <RequireRouteAccess routeKey="practitionerManagement">
                        <PractitionerManagementPage/>
                    </RequireRouteAccess>
                )
            },
            {
                path: "account-requests",
                element: (
                    <RequireRouteAccess routeKey="accountRequests">
                        <AccountRequestsPage/>
                    </RequireRouteAccess>
                )
            }
        ]
    }
]



