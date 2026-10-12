export interface DepartmentLookupDto {
    id: string;
    name: string;
}

export interface DepartmentDetailDto {
    id: string;
    name: string;
    departmentCode: string;
    imageUrl: string | null;
    description: string | null;
    isActive: boolean;
}