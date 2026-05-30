import type { ReactNode } from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../providers/AuthProviders";
import { ROLES } from "../providers/auth.provider.type";
import type { AppRouteKey } from "./route.config";
import { routeAccess } from "./route.access";

// REQUIRE AUTH
export const RequireAuth = ({ children }: { children: ReactNode }) => {
  const { isAuthenticated, isLoading } = useAuth();
 
  if (isLoading) return null;
 
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
 
  return <>{children}</>;
};

// REQUIRE ROLE
export const RequireRouteAccess = ({routeKey, children} : {routeKey: AppRouteKey, children: ReactNode}) => {
  const { user } = useAuth();

  const allowedRoles = routeAccess[routeKey];

  if (!user || !allowedRoles.includes(user.role)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>;
};

// PUBLIC ROUTE
export const PublicRoute = ({ children }: { children: ReactNode }) => {
  const { isAuthenticated, isLoading, user } = useAuth();
 
  if (isLoading) return null;
 
  if (isAuthenticated) {
    if (user?.role === ROLES.ADMIN) return <Navigate to="/admin/dashboard" replace />;
    if (user?.role === ROLES.PRACTITIONER) return <Navigate to="/practitioner/dashboard" replace />;
    return <Navigate to="/patient/dashboard" replace />;
  }
 
  return <>{children}</>;
};
