export const ROLES = {
    PATIENT : "Patient",
    PRACTITIONER : "Practitioner",
    ADMIN : "Admin  "
} as const;

export type UserRole = (typeof ROLES)[keyof typeof ROLES];

export interface AuthUser {
    userId : string;
    email : string;
    role : UserRole;
}

export type AuthContextValue = {
    user : AuthUser | null;
    isAuthenticated : boolean;
    isLoading : boolean;
    setUser : (user : AuthUser | null) => void;
    logout: () => void;
}