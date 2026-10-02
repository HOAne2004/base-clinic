'use client'
import ProtectedRoute from "@/src/components/common/ProtectedRoute";
import { useRouter } from "next/navigation";

export default function AdminPage() {
  const router = useRouter();

  return (
    <ProtectedRoute>
      <div>
        <h1>Admin Page</h1>
        <p>Welcome to the admin dashboard.</p>
        <button onClick={() => router.push("/admin/roles")}>Go to Roles Management</button>
      </div>
    </ProtectedRoute>
  );
}