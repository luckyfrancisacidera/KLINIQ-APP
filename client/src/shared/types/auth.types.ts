export const Gender = {
    Male : 1,
    Female : 2,
    Other : 3,
} as const;

export type Gender = (typeof Gender)[keyof typeof Gender];

export interface LoginPayload {
    email: string;
    password: string;
}

export interface RegisterPayload {
    email: string;
    password: string;
    confirmPassword: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    gender: Gender;
    street: string;
    city: string;
    country: string;
    phoneNumber: string;
    emergencyContact?: string;
}

export interface SetPasswordPayload {
    invitationToken: string;
    password: string;
    confirmPassword: string;
}

export interface AuthResponse {
    userId: string;
    email: string;
    role: "Patient" | "Practitioner" | "Admin";
    accessTokenExpiresAtUtc: string;
    devAccessToken?: string;
}

export type MeResponse = AuthResponse;

export interface SetPasswordResponse {
    email: string;
    message: string;
}

export interface ForgotPasswordPayload {
    email: string;
}

export interface ResetPasswordPayload {
    email: string;
    token: string;
    password: string;
    confirmPassword: string;
}

export interface ChangePasswordPayload {
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
}
