'use client';
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/src/contexts/AuthContext";
import { toast } from "react-toastify";
import LoadingState from "@/src/components/common/LoadingState"; // Import component

export default function ProtectedRoute({ children }: { children: React.ReactNode }) {
    const { user, isLoading } = useAuth();
    const router = useRouter();

    useEffect(() => {
        if (!isLoading && !user) {
            toast.warning("Vui lòng đăng nhập để truy cập trang này.");
            router.push("/");
        }
    }, [user, isLoading, router]);

    // Dùng Loading chung!
    if (isLoading) {
        return <LoadingState fullScreen text="Đang kiểm tra quyền truy cập..." />;
    }

    if (!user) return null;

    return <>{children}</>;
}