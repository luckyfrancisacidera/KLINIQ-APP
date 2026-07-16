export type AppointmentStatus = "Pending" | "Confirmed" | "InQueue" | "InConsultation" | "Cancelled" | "Completed"

export interface AppointmentDto {
  id: string
  patientId: string
  practitionerId: string
  clinicId: string
  scheduledAt: string
  durationMinutes: number
  status: AppointmentStatus
  reason: string | null
  notes: string | null
  queuedAtUtc: string | null
  consultationStartedAtUtc: string | null
  completedAtUtc: string | null
}

export interface BookAppointmentPayload {
  scheduleId: string
  appointmentDate: string
  slotTime: string
  reason?: string
}

export interface RescheduleAppointmentPayload {
  scheduleId: string
  appointmentDate: string
  slotTime: string
}
