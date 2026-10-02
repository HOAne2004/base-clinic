import ProtectedRoute from "@/src/components/common/ProtectedRoute";
import RolePermissionManager from "@/src/features/roles/components/RolePermissionManager";

export default function RolesPage(){
    return(
        <ProtectedRoute>
            <main className="container-app py-8 bg-gray-50 min-h-screen">
                <RolePermissionManager/>
            </main>
        </ProtectedRoute>
    )
}