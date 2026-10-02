import { apiClient } from "@/src/lib/api/client"
import { RoleDetailResponse, PermissionGroupResponse } from "../types/response"

export const rolesApi = {
    getAllRoles: () =>
        apiClient<RoleDetailResponse[]>("api/roles", {
            method: "GET",
        }),

    getAllPermissions: () =>
        apiClient<PermissionGroupResponse[]>("api/roles/permissions", {
            method: "GET",
        }),

    getRolePermissions: (roleId: string) =>
        apiClient<string[]>(`api/roles/${roleId}/permissions`, {
            method: "GET",
        }),

    assignPermissions: (roleId: string, permissionIds: string[]) =>
        apiClient<{ message: string }>(`api/roles/${roleId}/permissions`, {
            method: "PUT",
            body: JSON.stringify(permissionIds),
        }),
};