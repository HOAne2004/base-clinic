// src/app/(dashboard)/doctors/page.tsx

"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { doctorsApi } from "@/src/features/doctors/api/doctor.api";
import { DoctorListDto } from "@/src/features/doctors/types/response";

export default function DoctorsPage() {
    const [doctors, setDoctors] = useState<DoctorListDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchDoctors = async () => {
        try {
            setLoading(true);
            const data = await doctorsApi.getAllDoctors();
            setDoctors(data);
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchDoctors();
    }, []);

    const handleDelete = async (id: string, name: string) => {
        if (!confirm(`Bạn có chắc muốn xóa bác sĩ "${name}"?`)) return;
        try {
            await doctorsApi.deleteDoctor(id);
            fetchDoctors(); // Refresh
        } catch (err: any) {
            alert(err.message);
        }
    };

    if (loading) return <div className="p-6">Đang tải...</div>;
    if (error) return <div className="p-6 text-red-500">Lỗi: {error}</div>;

    return (
        <div className="p-6">
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-2xl font-bold">Quản lý bác sĩ</h1>
                <Link
                    href="doctors/create"
                    className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                >
                    + Thêm bác sĩ
                </Link>
            </div>

            <div className="overflow-x-auto bg-white rounded-lg shadow">
                <table className="min-w-full">
                    <thead className="bg-gray-50">
                        <tr>
                            <th className="px-4 py-3 text-left text-sm font-medium text-gray-600">Mã BS</th>
                            <th className="px-4 py-3 text-left text-sm font-medium text-gray-600">Họ tên</th>
                            <th className="px-4 py-3 text-left text-sm font-medium text-gray-600">SĐT</th>
                            <th className="px-4 py-3 text-left text-sm font-medium text-gray-600">Email</th>
                            <th className="px-4 py-3 text-left text-sm font-medium text-gray-600">Khoa</th>
                            <th className="px-4 py-3 text-left text-sm font-medium text-gray-600">Trạng thái</th>
                            <th className="px-4 py-3 text-right text-sm font-medium text-gray-600">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100">
                        {doctors.map((doc) => (
                            <tr key={doc.id} className="hover:bg-gray-50">
                                <td className="px-4 py-3 text-sm">{doc.doctorCode}</td>
                                <td className="px-4 py-3 text-sm font-medium">{doc.fullName}</td>
                                <td className="px-4 py-3 text-sm">{doc.phoneNumber}</td>
                                <td className="px-4 py-3 text-sm">{doc.email || "—"}</td>
                                <td className="px-4 py-3 text-sm">{doc.departmentName}</td>
                                <td className="px-4 py-3 text-sm">
                                    <span
                                        className={`px-2 py-1 rounded text-xs ${doc.accountStatus === "Active"
                                            ? "bg-green-100 text-green-700"
                                            : "bg-red-100 text-red-700"
                                            }`}
                                    >
                                        {doc.accountStatus}
                                    </span>
                                </td>
                                <td className="px-4 py-3 text-sm text-right space-x-2">
                                    <Link
                                        href={`/admin/doctors/${doc.id}`}
                                        className="text-blue-600 hover:underline"
                                    >
                                        Xem
                                    </Link>
                                    <Link
                                        href={`/admin/doctors/${doc.id}/edit`}
                                        className="text-yellow-600 hover:underline"
                                    >
                                        Sửa
                                    </Link>
                                    <button
                                        onClick={() => handleDelete(doc.id, doc.fullName)}
                                        className="text-red-600 hover:underline"
                                    >
                                        Xóa
                                    </button>
                                </td>
                            </tr>
                        ))}
                        {doctors.length === 0 && (
                            <tr>
                                <td colSpan={7} className="px-4 py-8 text-center text-gray-500">
                                    Chưa có bác sĩ nào.
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}