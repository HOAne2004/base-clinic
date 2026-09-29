import { apiClient } from "@/src/lib/api/client"
import { LoginRequest, RegisterRequest } from "../types/request"
import { LoginResponse } from "../types/response"

export const authApi = {
    register: (data: RegisterRequest) =>
        apiClient<{ message: string }>("api/auth/register", {
            method: "POST",
            body: JSON.stringify(data),
        }),

    login: (data: LoginRequest) =>
        apiClient<LoginResponse>("api/auth/login", {
            method: "POST",
            body: JSON.stringify(data),
        }),
    logout: () =>
        apiClient<{ message: string }>("/api/auth/logout", {
            method: "POST",
        }),
};