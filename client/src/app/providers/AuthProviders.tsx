import { createContext, useContext, useEffect, useState, type PropsWithChildren } from "react";
import type { AuthContextValue, AuthUser, UserRole} from "./auth.provider.type";
import {authApi} from "@shared/api/auth.api";

const AuthContext = createContext<AuthContextValue | null>(null);

export const AuthProvider = ({children} : PropsWithChildren) => {
    const [user, setUser] = useState<AuthUser | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        authApi.me()
            .then(({data}) => {
                setUser({
                    userId : data.userId,
                    email : data.email,
                    role : data.role as UserRole,
                });
            })
            .catch(() => setUser(null))
            .finally(() => setIsLoading(false))
    }, []);

    const logout = async () => {
        await authApi.logout().catch(() => {});
        setUser(null);
    }

    const value :AuthContextValue = {
        user,
        isAuthenticated: user !== null,
        isLoading,
        setUser,
        logout
    };

   return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if(!context){
        throw new Error("useAuth must be used within an AuthProvider");
    }   

    return context;
};