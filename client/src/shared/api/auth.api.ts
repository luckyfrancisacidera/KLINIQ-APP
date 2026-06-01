import type { AuthResponse, LoginPayload, MeResponse, RegisterPayload, SetPasswordPayload, SetPasswordResponse } from "../types/auth.types";
import axios from "./axios";

export const authApi = {
    //AUTH API
    login: (payload: LoginPayload) => axios.post<AuthResponse>("/auth/login", payload),

    register: (payload: RegisterPayload) => axios.post<AuthResponse>("/auth/register", payload),

    logout: () => axios.post("/auth/logout"),

    me: () => axios.get<MeResponse>("/auth/me"),
    
    setPassword: (payload: SetPasswordPayload) => axios.post<SetPasswordResponse>("/auth/set-password", payload),
};