import type { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../providers/AuthProviders";
import type { AppRouteKey } from "./route.config";
import { routeAccess } from "./route.access";

// REQUIRE AUTH
export const RequireAuth = ({ children }: { children: ReactNode }) => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

// REQUIRE ROLE
export const RequireRouteAccess = ({routeKey, children} : {routeKey: AppRouteKey, children: ReactNode}) => {
  const {role} = useAuth();

  const allowedRoles = routeAccess[routeKey]; 
  
  if(!role || !allowedRoles.includes(role)) {    
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>; 
} 

// PUBLIC ROUTE
export const PublicRoute = ({children} : {children: ReactNode}) => {
  const {isAuthenticated, role} = useAuth();

  if (isAuthenticated){
    if (role === "admin") {
      return <Navigate to="/admin/dashboard" replace />;
    }
    if (role === "practitioner") {
      return <Navigate to="/practitioner/dashboard" replace />;
    }
    return <Navigate to="/patient/dashboard" replace />;
  }

  return <>{children}</>;
}
