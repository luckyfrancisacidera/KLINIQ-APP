import { createContext, useContext, useState, type PropsWithChildren } from "react";
import type { AuthContextValue, UserRole } from "./auth.provider.type";

const AuthContext = createContext<AuthContextValue | null>(null);

export const AuthProvider = ({children} : PropsWithChildren) => {
    const [role, setRole] = useState<UserRole | null >(null);

    const value :AuthContextValue = {
        isAuthenticated: true, 
        role: role ||"patient", // Default role for testing, replace with dynamic role assignment
        login: (newRole: UserRole) => setRole(newRole),
        logout: () => setRole(null)    
    };

   return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export const useAuth = () => {
    const context = useContext(AuthContext);

    if(!context){
        throw new Error("useAuth must be used within an AuthProvider");
    }   

    return context;
}