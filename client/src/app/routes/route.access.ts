import type { UserRole } from "../providers/auth.provider.type";
import type { AppRouteKey } from "./route.config";

// Defines which user roles have access to each route in the application
export const routeAccess: Record<AppRouteKey, UserRole[]> = {
    home: ["patient", "practitioner"],
    findPractitioner: ["patient"],
    patientDashboard: ["patient"],
    searchResults: ["patient"],

    practitionerDashboard: ["practitioner"],
    appointmentBooking: ["patient"],
    appointmentManagement: ["practitioner"],

    profileManagement: ["patient", "practitioner"],
    notifications: ["patient", "practitioner"],
    settings: ["patient", "practitioner"],
    helpSupport: ["patient", "practitioner"],

    adminDashboard: ["admin"],
    userManagement: ["admin"],
}