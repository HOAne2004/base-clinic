// src/app/(dashboard)/doctors/[id]/page.tsx

"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { doctorsApi } from "@/src/features/doctors/api/doctor.api";
import { DoctorAdminDetailDto } from "@/src/features/doctors/types/response";
import ProtectedRoute from "@/src/components/common/ProtectedRoute";
export default function DoctorDetailPage() {
    const { id } = useParams<{ id: string }>();
    const router = useRouter();
    const [doctor, setDoctor] = useState<DoctorAdminDetailDto | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!id) return;
        doctorsApi
            .getDoctorDetail(id)
            .then(setDoctor)
            .catch((err) => alert(err.message))
            .finally(() => setLoading(false));
    }, [id]);

    if (loading) return <div className="p-6">Đang tải...</div>;
    if (!doctor) return <div className="p-6">Không tìm thấy bác sĩ.</div>;

    return (
        <div className="p-6 max-w-3xl mx-auto">
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-2xl font-bold">Chi tiết bác sĩ</h1>
                <button
                    onClick={() => router.push(`/admin/doctors/${id}/edit`)}
                    className="bg-yellow-500 text-white px-4 py-2 rounded"
                >
                    Sửa
                </button>
            </div>

            <div className="bg-white p-6 rounded-lg shadow grid grid-cols-2 gap-4">
                <Field label="Mã bác sĩ" value={doctor.doctorCode} />
                <Field label="Họ tên" value={doctor.fullName} />
                <Field label="SĐT" value={doctor.phoneNumber} />
                <Field label="Email" value={doctor.email} />
                <Field label="Khoa" value={doctor.departmentName} />
                <Field label="Trạng thái" value={doctor.accountStatus} />
                <Field label="Đăng nhập cuối" value={doctor.lastLoginAt ? new Date(doctor.lastLoginAt).toLocaleString() : "—"} />
                <Field label="Giới tính" value={doctor.gender === null ? "—" : doctor.gender ? "Nam" : "Nữ"} />
                <Field label="Ngày sinh" value={doctor.dateOfBirth ? new Date(doctor.dateOfBirth).toLocaleDateString() : "—"} />
                <Field label="CMND/CCCD" value={doctor.identityNumber} />
                <Field label="Số giấy phép" value={doctor.licenseNumber} />
                <Field label="Phí khám" value={doctor.consultationFee?.toLocaleString()} />
                <Field label="Địa chỉ" value={doctor.fullAddress} className="col-span-2" />
                <Field label="Mô tả" value={doctor.description} className="col-span-2" />
            </div>
        </div>
    );
}

function Field({ label, value, className = "" }: { label: string; value: any; className?: string }) {
    return (
        <ProtectedRoute>
            <div className={className}>
                <div className="text-sm text-gray-500">{label}</div>
                <div className="font-medium">{value || "—"}</div>
            </div>
        </ProtectedRoute>

    );
}