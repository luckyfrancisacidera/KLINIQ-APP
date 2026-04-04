import { createBrowserRouter } from "react-router-dom"
import { patientRoutes } from "./patient.routes";
import { AppShellRoute } from "./appshell.route";

export const router = createBrowserRouter([
    {
    path: "/",
    element: <AppShellRoute/>,
    children: [
    ...patientRoutes
    ] }
]);

