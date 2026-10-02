// Map theo RoleDetailDto
export interface RoleDetailResponse {
    id: string;
    name: string;
    description: string | null;
    isSystemRole: boolean;
}

// Map theo PermissionDto
export interface PermissionResponse {
    id: string;
    name: string;
    description: string | null;
}

// Map theo PermissionGroupDto
export interface PermissionGroupResponse {
    resource: string;
    permissions: PermissionResponse[];
}