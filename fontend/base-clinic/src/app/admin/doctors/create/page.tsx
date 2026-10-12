// src/app/(dashboard)/doctors/create/page.tsx

"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { CreateDoctorRequest } from "@/src/features/doctors/types/request";
import { doctorsApi } from "@/src/features/doctors/api/doctor.api";
import { DoctorForm } from "@/src/features/doctors/components/DoctorForm";
import { toast } from "react-toastify";

export default function CreateDoctorPage() {
    const router = useRouter();
    const [loading, setLoading] = useState(false);
 
    const handleSubmit = async (data: CreateDoctorRequest) => {
        try {
            setLoading(true);
            const res = await doctorsApi.createDoctor(data);
            toast.success("Thêm bác sĩ thành công!");
            router.push(`/doctors/${res.data}`);
        } catch (err: any) {
            alert(err.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="p-6 max-w-3xl mx-auto">
            <h1 className="text-2xl font-bold mb-6">Thêm bác sĩ mới</h1>
            <DoctorForm mode="create" onSubmit={handleSubmit} loading={loading} />
        </div>
    );
}