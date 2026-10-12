import { apiClient } from "@/src/lib/api/client";
import { CreateDoctorRequest, UpdateDoctorRequest } from "../types/request";
import { DoctorAdminDetailDto, DoctorListDto } from "../types/response";


export const doctorsApi = {
    // Lấy danh sách bác sĩ (Admin)
    getAllDoctors: () =>
        apiClient<DoctorListDto[]>("api/doctors", {
            method: "GET",
        }),

    // Lấy chi tiết bác sĩ (Admin)
    getDoctorDetail: (id: string) =>
        apiClient<DoctorAdminDetailDto>(`api/doctors/${id}/admin-detail`, {
            method: "GET",
        }),

    // Tạo bác sĩ mới
    createDoctor: (data: CreateDoctorRequest) =>
        apiClient<{ message: string; data: string }>("api/doctors", {
            method: "POST",
            body: JSON.stringify(data),
        }),

    // Cập nhật bác sĩ
    updateDoctor: (id: string, data: UpdateDoctorRequest) =>
        apiClient<{ message: string }>(`api/doctors/${id}`, {
            method: "PUT",
            body: JSON.stringify({ ...data, id }),
        }),

    // Xóa bác sĩ
    deleteDoctor: (id: string) =>
        apiClient<{ message: string }>(`api/doctors/${id}`, {
            method: "DELETE",
        }),
};