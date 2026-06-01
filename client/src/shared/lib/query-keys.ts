export const queryKeys = {
  auth: {
    me: ["auth", "me"] as const,
  },
  patients: {
    all: (page: number) => ["patients", page] as const,
    detail: (id: string) => ["patients", id] as const,
  },
  practitioners: {
    all: (params?: object) => ["practitioners", params] as const,
    detail: (id: string) => ["practitioners", id] as const,
    schedules: (id: string) => ["practitioners", id, "schedules"] as const,
    slots: (id: string, from?: string, to?: string) => ["practitioners", id, "slots", from, to] as const,
  },
  appointments: {
    byPatient: (patientId: string) => ["appointments", "patient", patientId] as const,
    byPractitioner: (practitionerId: string) => ["appointments", "practitioner", practitionerId] as const,
    detail: (id: string) => ["appointments", id] as const,
  },
}
