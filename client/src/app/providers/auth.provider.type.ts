export const ROLES = {
    PATIENT : "patient",
    PRACTITIONER : "practitioner",
    ADMIN : "admin"
} as const;

export type UserRole = typeof ROLES[keyof typeof ROLES];

export type AuthContextValue = {
    isAuthenticated: boolean;
    role: UserRole | null;
    login: (role: UserRole) => void;
    logout: () => void;
}