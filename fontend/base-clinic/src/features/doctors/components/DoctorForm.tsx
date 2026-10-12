// src/components/doctors/DoctorForm.tsx

"use client";

import { useState, useEffect } from "react";
import { DoctorAdminDetailDto } from "../types/response";
import { CreateDoctorRequest } from "../types/request";
import { useDepartmentLookup } from "../../departments/api/useDepartmentLookup.api";

type Mode = "create" | "edit";

interface DoctorFormProps {
    mode: Mode;
    initialData?: DoctorAdminDetailDto;
    onSubmit: (data: CreateDoctorRequest) => Promise<void>;
    loading?: boolean;
}

export function DoctorForm({ mode, initialData, onSubmit, loading }: DoctorFormProps) {
    const { departments, loading: loadingDepts } = useDepartmentLookup();

    const [form, setForm] = useState<CreateDoctorRequest>({
        fullName: "",
        phoneNumber: "",
        email: "",
        password: "",
        departmentId: "",
        avatar: "",
        fullAddress: "",
        gender: undefined,
        dateOfBirth: "",
        identityNumber: "",
        licenseNumber: "",
        consultationFee: undefined,
        description: "",
    });

    // Đổ dữ liệu vào form khi ở mode edit
    useEffect(() => {
        if (mode === "edit" && initialData) {
            setForm({
                fullName: initialData.fullName,
                phoneNumber: initialData.phoneNumber,
                email: initialData.email ?? "",
                password: "",
                departmentId: initialData.departmentId,  
                avatar: initialData.avatar ?? "",
                fullAddress: initialData.fullAddress ?? "",
                gender: initialData.gender ?? undefined,
                dateOfBirth: initialData.dateOfBirth?.split("T")[0] ?? "",
                identityNumber: initialData.identityNumber ?? "",
                licenseNumber: initialData.licenseNumber ?? "",
                consultationFee: initialData.consultationFee ?? undefined,
                description: initialData.description ?? "",
            });
        }
    }, [mode, initialData]);

    const handleChange = (
        e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
    ) => {
        const { name, value, type } = e.target;
        setForm((prev) => ({
            ...prev,
            [name]: type === "number" ? (value === "" ? undefined : Number(value)) : value,
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        await onSubmit(form);
    };

    const isEdit = mode === "edit";

    return (
        <form onSubmit={handleSubmit} className="bg-white p-6 rounded-lg shadow space-y-6">
            {/* ==== THÔNG TIN TÀI KHOẢN (chỉ hiện khi tạo mới) ==== */}
            {!isEdit && (
                <section>
                    <h2 className="font-semibold text-lg border-b pb-2 mb-4">
                        Thông tin tài khoản
                    </h2>
                    <div className="grid grid-cols-2 gap-4">
                        <Input label="Họ tên *" name="fullName" value={form.fullName} onChange={handleChange} required />
                        <Input label="Số điện thoại *" name="phoneNumber" value={form.phoneNumber} onChange={handleChange} required />
                        <Input label="Email" name="email" type="email" value={form.email} onChange={handleChange} />
                        <Input label="Mật khẩu *" name="password" type="password" value={form.password} onChange={handleChange} required minLength={6} />
                    </div>
                </section>
            )}

            {/* ==== THÔNG TIN BÁC SĨ ==== */}
            <section>
                <h2 className="font-semibold text-lg border-b pb-2 mb-4">Thông tin bác sĩ</h2>
                <div className="grid grid-cols-2 gap-4">
                    {/* Dropdown Khoa */}
                    <div>
                        <label className="block text-sm font-medium mb-1">
                            Khoa {!isEdit && "*"}
                        </label>
                        <select
                            name="departmentId"
                            value={form.departmentId}
                            onChange={handleChange}
                            required
                            className="w-full border rounded px-3 py-2 disabled:bg-gray-100"
                        >
                            <option value="">
                                {loadingDepts ? "Đang tải..." : "-- Chọn khoa --"}
                            </option>
                            {departments.map((d) => (
                                <option key={d.id} value={d.id}>
                                    {d.name}
                                </option>
                            ))}
                        </select>
                        {isEdit && (
                            <p className="text-xs text-gray-500 mt-1">
                                Không thể thay đổi khoa sau khi tạo.
                            </p>
                        )}
                    </div>

                    <Input label="Avatar (URL)" name="avatar" value={form.avatar} onChange={handleChange} />
                    <Input label="Địa chỉ" name="fullAddress" value={form.fullAddress} onChange={handleChange} />

                    <div>
                        <label className="block text-sm font-medium mb-1">Giới tính</label>
                        <select
                            name="gender"
                            value={form.gender === undefined ? "" : form.gender ? "true" : "false"}
                            onChange={(e) =>
                                setForm((p) => ({
                                    ...p,
                                    gender: e.target.value === "" ? undefined : e.target.value === "true",
                                }))
                            }
                            className="w-full border rounded px-3 py-2"
                        >
                            <option value="">-- Chọn --</option>
                            <option value="true">Nam</option>
                            <option value="false">Nữ</option>
                        </select>
                    </div>

                    <Input label="Ngày sinh" name="dateOfBirth" type="date" value={form.dateOfBirth} onChange={handleChange} />
                    <Input label="CMND/CCCD" name="identityNumber" value={form.identityNumber} onChange={handleChange} />
                    <Input label="Số giấy phép" name="licenseNumber" value={form.licenseNumber} onChange={handleChange} />
                    <Input
                        label="Phí khám (VNĐ)"
                        name="consultationFee"
                        type="number"
                        value={form.consultationFee ?? ""}
                        onChange={handleChange}
                        min={0}
                    />
                </div>

                <div className="mt-4">
                    <label className="block text-sm font-medium mb-1">Mô tả</label>
                    <textarea
                        name="description"
                        value={form.description}
                        onChange={handleChange}
                        rows={3}
                        className="w-full border rounded px-3 py-2"
                    />
                </div>
            </section>

            <div className="flex gap-3 pt-4 border-t">
                <button
                    type="submit"
                    disabled={loading}
                    className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
                >
                    {loading ? "Đang lưu..." : isEdit ? "Cập nhật" : "Tạo mới"}
                </button>
                <button
                    type="button"
                    onClick={() => history.back()}
                    className="border px-6 py-2 rounded hover:bg-gray-50"
                >
                    Hủy
                </button>
            </div>
        </form>
    );
}

function Input({
    label,
    ...props
}: { label: string } & React.InputHTMLAttributes<HTMLInputElement>) {
    return (
        <div>
            <label className="block text-sm font-medium mb-1">{label}</label>
            <input {...props} className="w-full border rounded px-3 py-2" />
        </div>
    );
}