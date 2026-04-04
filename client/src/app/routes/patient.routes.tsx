// PATIENT ROUTES
import {lazy} from "react";
import { Outlet, type RouteObject} from "react-router-dom";
import {RequireAuth, RequireRouteAccess} from "./route.guard";

const PatientDashboardPage = lazy(() => import("@features/patient/pages/PatientDashboardPage"));

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
                )
            }
        ]
    }
]