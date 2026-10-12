import { apiClient } from "@/src/lib/api/client";
import { DepartmentLookupDto, DepartmentDetailDto } from "@/src/features/departments/types/response";

export const departmentsApi = {
    // Dùng cho dropdown chọn khoa (không cần quyền)
    getLookupList: () =>
        apiClient<DepartmentLookupDto[]>("api/departments/lookup", {
            method: "GET",
        }),

    // Dùng cho trang quản lý khoa (cần permission)
    getAll: () =>
        apiClient<DepartmentDetailDto[]>("api/departments", {
            method: "GET",
        }),
};