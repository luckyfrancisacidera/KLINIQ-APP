export interface PatientDto {
    id: string;
    userId: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    age: number;
    gender: string;
    street: string;
    city: string;
    country: string;
    phoneNumber: string | null;
    emergencyContact : string | null;
}

export interface UpdatePatientPayload {
    firstName: string;
    lastName: string;
    street: string;
    city: string;
    country: string;
    phoneNumber?: string;
    emergencyContact?: string;
}

