import axios from "axios";
import type { PaginatedResult } from "../types/common.types";
import type { AddBreakPayload, AvailableSlotDto, CreateSchedulePayload, PractitionerDetailDto, PractitionerDto, ScheduleSummaryDto, UpdatePractitionerPayload, UpdateSchedulePayload } from "../types/practitioner.types";

export const practitionerApi = {

    getAll : (params? : { 
        search? : string;
        specialization? : string;
        page? : number;
        pageSize? : number;
    }) => axios.get<PaginatedResult<PractitionerDto>>("/practitioners", { params }),

    getById : (id : string) => axios.get<PractitionerDetailDto>(`/practitioners/${id}`),

    update : (id : string, payload : UpdatePractitionerPayload) => axios.put<PractitionerDetailDto>(`/practitioners/${id}`, payload),

    delete: (id : string) => axios.delete(`/practitioners/${id}`),

    // SCHEULES 
    getSchedules : (id : string) => axios.get<ScheduleSummaryDto[]>(`/practitioners/${id}/schedules`),

    getAvailableSlots : (id : string, from? : string, to? : string) => axios.get<AvailableSlotDto[]>(`/practitioners/${id}/available-slots`, { params : { from, to } }),

    createSchedule : (id : string, payload : CreateSchedulePayload) => axios.post<ScheduleSummaryDto>(`/practitioners/${id}/schedules`, payload),

    updateSchedule : (practitionerId : string, scheduleId : string, payload : UpdateSchedulePayload) => axios.put<ScheduleSummaryDto>(`/practitioners/${practitionerId}/schedules/${scheduleId}`, payload),

    //BREAKS
    addBreak : (practitionerId : string, scheduleId : string, payload : AddBreakPayload) => axios.post<ScheduleSummaryDto>(`/practitioners/${practitionerId}/schedules/${scheduleId}/breaks`, payload),

    deleteBreak : (practitionerId : string, scheduleId : string, breakId : string) => axios.delete(`/practitioners/${practitionerId}/schedules/${scheduleId}/breaks/${breakId}`),
}