export interface ClinicSummaryDto {
    id : string;
    name : string;
    latitude : number;
    longitude : number;
}

export interface BreakDto {
    id : string;
    startTime : string;
    endTime : string;
}

export interface ScheduleSummaryDto {
    id : string;
    day: string;
    startTime: string;
    endTime: string;
    appointmentDurationMinutes: number;
    isAvailable: boolean;
    breaks: BreakDto[];
}

export interface PractitionerDto {
    id : string;
    userId : string;
    firstName : string;
    lastName : string;
    licenseNumber : string;
    specializations : string[];
    clinicId : string | null;
    clinic: ClinicSummaryDto | null;
}

export interface PractitionerDetailDto extends PractitionerDto {
    schedules: ScheduleSummaryDto[];
}

export interface AvailableSlotDto {
    scheduleId: string;
    date: string;
    dayOfWeek: string;
    slots: string[];    
}

export interface UpdatePractitionerPayload {
    firstName : string;
    lastName : string;
    specializations : string[];
}

export interface CreateSchedulePayload {
    day: string;
    startTime: string;
    endTime: string;
    appointmentDurationMinutes: number;
}

export type UpdateSchedulePayload = CreateSchedulePayload;

export interface AddBreakPayload {
    breakStart : string;
    breakEnd : string;
}
