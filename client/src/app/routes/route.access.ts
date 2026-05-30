import { ROLES, type UserRole } from "../providers/auth.provider.type";
import type { AppRouteKey } from "./route.config";

// Defines which user roles have access to each route in the application
export const routeAccess: Record<AppRouteKey, UserRole[]> = {
    home: [ROLES.PATIENT, ROLES.PRACTITIONER],
    findPractitioner: [ROLES.PATIENT],
    patientDashboard: [ROLES.PATIENT],
    searchResults: [ROLES.PATIENT],

    practitionerDashboard: [ROLES.PRACTITIONER],
    appointmentBooking: [ROLES.PATIENT],
    appointmentManagement: [ROLES.PRACTITIONER],

    profileManagement: [ROLES.PATIENT, ROLES.PRACTITIONER],
    notifications: [ROLES.PATIENT, ROLES.PRACTITIONER],
    settings: [ROLES.PATIENT, ROLES.PRACTITIONER],
    helpSupport: [ROLES.PATIENT, ROLES.PRACTITIONER],

    adminDashboard: [ROLES.ADMIN],
    userManagement: [ROLES.ADMIN],
}