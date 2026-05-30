import type { PaginatedResult } from "../types/common.types";
import type { PatientDto, UpdatePatientPayload } from "../types/patient.types";
import axios from "./axios";

export const patientApi = {
    
    getAll : (page = 1, pageSize =20) => axios.get<PaginatedResult<PatientDto>>("patient", {params: {page, pageSize}}),

    getById : (id: string) => axios.get<PatientDto>(`patient/${id}`),

    update: (id: string, payload: UpdatePatientPayload) => axios.put(`/patient/${id}`, payload),

    delete: (id: string) => axios.delete(`/patient/${id}`)

}