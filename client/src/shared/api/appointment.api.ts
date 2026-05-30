import axios from "axios";
import type { AppointmentDto, BookAppointmentPayload } from "../types/appointment.types";
import type { PaginatedResult } from "../types/common.types";

export const appointmentApi = {

    getById : (id : string) => axios.get<AppointmentDto>(`/appointments/${id}`),

    getByPatient : (patientId: string, page = 1, pageSize = 20) => axios.get<PaginatedResult<AppointmentDto>>(`/appointments/patient/${patientId}`, { params : { page, pageSize } }),

    getByPractitioner : (practitionerId : string, page = 1 , pageSize = 20) => axios.get<PaginatedResult<AppointmentDto>>(`/appointments/practitioner/${practitionerId}`, { params : { page, pageSize } }),

    book : (payload : BookAppointmentPayload) => axios.post<AppointmentDto>("/appointments", payload),

    confirm : (id : string) => axios.post(`/appointments/${id}/confirm`),

    cancel : (id : string) => axios.post(`/appointments/${id}/cancel`),

    complete : (id : string) => axios.post(`/appointments/${id}/complete`),
};