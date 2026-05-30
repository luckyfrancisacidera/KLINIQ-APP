// This file defines the types for the application's routing system. It includes the keys for all the routes used in the application, which can be used to ensure type safety when navigating between different pages or components.
export type AppRouteKey = 

    // PATIENT ROUTES
    | "patientDashboard"
    | "bookAppointment"
    | "patientAppointments"
    | "patientProfile"

    // PRACTITIONER ROUTES
    | "practitionerDashboard"
    | "practitionerAppointments"
    | "scheduleManagement"
    | "practitionerProfile"

    // ADMIN ROUTES
    | "adminDashboard"
    | "patientManagement"
    | "practitionerManagement"
    | "accountRequests"

    // SHARED ROUTES
    | "settings"