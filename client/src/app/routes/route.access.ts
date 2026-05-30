import { ROLES, type UserRole } from "../providers/auth.provider.type";
import type { AppRouteKey } from "./route.config";

export const routeAccess: Record<AppRouteKey, UserRole[]> = {
  // ── Patient ───────────────────────────────────────────────────────────────
  patientDashboard:      [ROLES.PATIENT],
  bookAppointment:       [ROLES.PATIENT],
  patientAppointments:   [ROLES.PATIENT],
  patientProfile:        [ROLES.PATIENT],

  // ── Practitioner ──────────────────────────────────────────────────────────
  practitionerDashboard:    [ROLES.PRACTITIONER],
  practitionerAppointments: [ROLES.PRACTITIONER],
  scheduleManagement:       [ROLES.PRACTITIONER],
  practitionerProfile:      [ROLES.PRACTITIONER],

  // ── Admin ─────────────────────────────────────────────────────────────────
  adminDashboard:            [ROLES.ADMIN],
  patientManagement:         [ROLES.ADMIN],
  practitionerManagement:    [ROLES.ADMIN],
  accountRequests:           [ROLES.ADMIN],

  // ── Shared ────────────────────────────────────────────────────────────────
  settings:      [ROLES.PATIENT, ROLES.PRACTITIONER],
};