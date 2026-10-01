import ProtectedRoute from "@/src/components/common/ProtectedRoute";

export default function ProfilePage() {
    return (
        <ProtectedRoute>
            <div className="mx-auto max-w-4xl px-4 py-10">
                <h1 className="mb-6 text-2xl font-bold">Thông tin cá nhân</h1>
                <p>Đây là trang thông tin cá nhân của bệnh nhân.</p>
            </div>
        </ProtectedRoute>
    );
}