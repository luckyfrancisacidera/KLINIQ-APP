export interface AppointmentDto {
    id : string;
    patientId : string;
    practitionerId : string;
    clinicId : string;
    scheduleId : string;
    durationMinutes : number;
    status : "Pending" | "Confirmed" | "Cancelled" | "Completed";
    reason : string;
    notes : string | null;
}

export interface BookAppointmentPayload {
    scheduleId : string;
    appointmentDate : string;
    slotTime : string;
    reason? : string;
}
