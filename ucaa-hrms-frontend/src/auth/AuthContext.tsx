import React, { createContext, useContext, useMemo, useState } from "react";
import apiClient from "../api/apiClient";
import { AuthResponse } from "../types/models";

interface AuthState {
  token: string | null;
  email: string | null;
  role: string | null;
}

interface AuthContextValue extends AuthState {
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [state, setState] = useState<AuthState>(() => ({
    token: localStorage.getItem("ucaa_token"),
    email: localStorage.getItem("ucaa_email"),
    role: localStorage.getItem("ucaa_role"),
  }));

  const login = async (email: string, password: string) => {
    const response = await apiClient.post<AuthResponse>("/auth/login", { email, password });
    const auth = response.data;
    localStorage.setItem("ucaa_token", auth.accessToken);
    localStorage.setItem("ucaa_email", auth.email);
    localStorage.setItem("ucaa_role", auth.role);
    setState({ token: auth.accessToken, email: auth.email, role: auth.role });
  };

  const logout = () => {
    localStorage.removeItem("ucaa_token");
    localStorage.removeItem("ucaa_email");
    localStorage.removeItem("ucaa_role");
    setState({ token: null, email: null, role: null });
  };

  const value = useMemo(
    () => ({
      ...state,
      login,
      logout,
      isAuthenticated: Boolean(state.token),
    }),
    [state]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
}
