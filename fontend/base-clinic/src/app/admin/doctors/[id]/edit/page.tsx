// src/app/(dashboard)/doctors/[id]/edit/page.tsx

"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { DoctorAdminDetailDto } from "@/src/features/doctors/types/response";
import { doctorsApi } from "@/src/features/doctors/api/doctor.api";
import { CreateDoctorRequest } from "@/src/features/doctors/types/request";
import { DoctorForm } from "@/src/features/doctors/components/DoctorForm";
import { toast } from "react-toastify";

export default function EditDoctorPage() {
    const { id } = useParams<{ id: string }>();
    const router = useRouter();
    const [doctor, setDoctor] = useState<DoctorAdminDetailDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        if (!id) return;
        doctorsApi
            .getDoctorDetail(id)
            .then(setDoctor)
            .catch((err) => alert(err.message))
            .finally(() => setLoading(false));
    }, [id]);

    const handleSubmit = async (data: CreateDoctorRequest) => {
        try {
            setSaving(true);
            // UpdateDoctorCommand chỉ nhận các field profile, không nhận account info
            await doctorsApi.updateDoctor(id, {
                avatar: data.avatar,
                fullAddress: data.fullAddress,
                gender: data.gender,
                dateOfBirth: data.dateOfBirth,
                identityNumber: data.identityNumber,
                licenseNumber: data.licenseNumber,
                consultationFee: data.consultationFee,
                description: data.description,
                departmentId: data.departmentId,
            });
            toast.success("Cập nhật thành công!");
            router.push(`admin/doctors/${id}`);
        } catch (err: any) {
            alert(err.message);
        } finally {
            setSaving(false);
        }
    };

    if (loading) return <div className="p-6">Đang tải...</div>;
    if (!doctor) return <div className="p-6">Không tìm thấy bác sĩ.</div>;

    return (
        <div className="p-6 max-w-3xl mx-auto">
            <h1 className="text-2xl font-bold mb-6">Sửa thông tin bác sĩ</h1>
            <DoctorForm mode="edit" initialData={doctor} onSubmit={handleSubmit} loading={saving} />
        </div>
    );
}