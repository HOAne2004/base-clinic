'use client'

import { useState, useEffect } from "react";
import { toast } from "react-toastify";
import { rolesApi } from "@/src/features/roles/api/roles.api";
import { RoleDetailResponse, PermissionGroupResponse } from "@/src/features/roles/types/response";
import LoadingState from "@/src/components/common/LoadingState";

export default function RolePermissionManager() {
    // State lưu dữ liệu danh mục
    const [roles, setRoles] = useState<RoleDetailResponse[]>([]);
    const [permissionGroups, setPermissionGroups] = useState<PermissionGroupResponse[]>([]);

    // State lưu trạng thái tương tác
    const [selectedRole, setSelectedRole] = useState<RoleDetailResponse | null>(null);
    const [activePermissionIds, setActivePermissionIds] = useState<string[]>([]);

    // State UI
    const [isLoadingInit, setIsLoadingInit] = useState(true);
    const [isLoadingRoleData, setIsLoadingRoleData] = useState(false);
    const [isSaving, setIsSaving] = useState(false);

    // 1. Tải danh sách Role và Danh mục Quyền khi mount
    useEffect(() => {
        const fetchInitialData = async () => {
            try {
                const [rolesData, groupsData] = await Promise.all([
                    rolesApi.getAllRoles(),
                    rolesApi.getAllPermissions()
                ]);
                setRoles(rolesData);
                setPermissionGroups(groupsData);

                // Mặc định chọn Role đầu tiên nếu có
                if (rolesData.length > 0) {
                    handleSelectRole(rolesData[0]);
                }
            } catch (error: any) {
                toast.error(error.message || "Lỗi khi tải dữ liệu phân quyền.");
            } finally {
                setIsLoadingInit(false);
            }
        };
        fetchInitialData();
    }, []);

    // 2. Hàm gọi API lấy danh sách ID quyền mỗi khi đổi Role
    const handleSelectRole = async (role: RoleDetailResponse) => {
        setSelectedRole(role);
        setIsLoadingRoleData(true);
        try {
            const currentPerms = await rolesApi.getRolePermissions(role.id);
            setActivePermissionIds(currentPerms);
        } catch (error: any) {
            toast.error("Không thể lấy danh sách quyền của chức danh này.");
        } finally {
            setIsLoadingRoleData(false);
        }
    };

    // 3. Xử lý logic Tích/Bỏ tích Checkbox
    const togglePermission = (permissionId: string) => {
        setActivePermissionIds(prev =>
            prev.includes(permissionId)
                ? prev.filter(id => id !== permissionId)
                : [...prev, permissionId]
        );
    };

    // (Nâng cao) Tích chọn/Bỏ chọn toàn bộ quyền trong 1 Resource
    const toggleGroup = (group: PermissionGroupResponse) => {
        const groupPermIds = group.permissions.map(p => p.id);
        const isAllSelected = groupPermIds.every(id => activePermissionIds.includes(id));

        if (isAllSelected) {
            setActivePermissionIds(prev => prev.filter(id => !groupPermIds.includes(id)));
        } else {
            setActivePermissionIds(prev => Array.from(new Set([...prev, ...groupPermIds])));
        }
    };

    // 4. Gọi API Lưu
    const handleSave = async () => {
        if (!selectedRole) return;
        setIsSaving(true);
        try {
            await rolesApi.assignPermissions(selectedRole.id, activePermissionIds);
            toast.success(`Đã cập nhật quyền cho chức danh: ${selectedRole.name}`);
        } catch (error: any) {
            toast.error(error.message || "Lỗi khi lưu phân quyền.");
        } finally {
            setIsSaving(false);
        }
    };

    if (isLoadingInit) return <LoadingState text="Đang tải dữ liệu hệ thống..." />;

    return (
        <div className="flex flex-col gap-6">
            {/* HÀNG 1: CHỌN ROLE */}
            <div className="bg-surface p-4 rounded-lg shadow-sm border border-border">
                <h2 className="text-lg font-bold mb-4 text-text">Chọn Chức danh (Role)</h2>
                <div className="flex flex-wrap gap-2">
                    {roles.map(role => (
                        <button
                            key={role.id}
                            onClick={() => handleSelectRole(role)}
                            className={`px-4 py-2 rounded-md text-sm font-medium transition-colors border
                                ${selectedRole?.id === role.id
                                    ? 'bg-primary text-white border-primary shadow-md'
                                    : 'bg-surface-muted text-text border-border hover:bg-border-muted'
                                }
                            `}
                        >
                            {role.name} {role.isSystemRole && <span className="text-xs opacity-75">(Hệ thống)</span>}
                        </button>
                    ))}
                </div>
            </div>

            {/* HÀNG 2: DANH SÁCH QUYỀN */}
            <div className="bg-surface p-4 rounded-lg shadow-sm border border-border flex-1">
                <div className="flex justify-between items-center mb-4 border-b border-border pb-4">
                    <div>
                        <h2 className="text-lg font-bold text-text">
                            Cấu hình quyền cho: <span className="text-primary">{selectedRole?.name}</span>
                        </h2>
                        <p className="text-sm text-text-muted mt-1">{selectedRole?.description}</p>
                    </div>

                    <button
                        onClick={handleSave}
                        disabled={isSaving || isLoadingRoleData}
                        className="btn bg-success text-white px-6 py-2 hover:opacity-90 transition-opacity"
                    >
                        {isSaving ? "Đang lưu..." : "Lưu thay đổi"}
                    </button>
                </div>

                {isLoadingRoleData ? (
                    <LoadingState text={`Đang tải quyền của ${selectedRole?.name}...`} />
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                        {permissionGroups.map(group => {
                            const groupPermIds = group.permissions.map(p => p.id);
                            const isAllSelected = groupPermIds.every(id => activePermissionIds.includes(id));

                            return (
                                <div key={group.resource} className="border border-border rounded-lg overflow-hidden bg-surface">
                                    {/* Header của Nhóm */}
                                    <div className="bg-surface-muted px-4 py-3 border-b border-border flex items-center justify-between">
                                        <span className="font-semibold text-text capitalize">{group.resource}</span>
                                        <button
                                            onClick={() => toggleGroup(group)}
                                            className="text-xs font-medium text-primary hover:underline"
                                        >
                                            {isAllSelected ? "Bỏ chọn tất cả" : "Chọn tất cả"}
                                        </button>
                                    </div>

                                    {/* Danh sách các quyền trong nhóm */}
                                    <div className="p-4 flex flex-col gap-3">
                                        {group.permissions.map(permission => (
                                            <label key={permission.id} className="flex items-start gap-3 cursor-pointer group">
                                                <input
                                                    type="checkbox"
                                                    checked={activePermissionIds.includes(permission.id)}
                                                    onChange={() => togglePermission(permission.id)}
                                                    className="mt-1 h-4 w-4 rounded border-border text-primary focus:ring-primary cursor-pointer bg-surface"
                                                />
                                                <div className="flex flex-col">
                                                    <span className="text-sm font-medium text-text group-hover:text-primary transition-colors">
                                                        {permission.name}
                                                    </span>
                                                    {permission.description && (
                                                        <span className="text-xs text-text-muted">{permission.description}</span>
                                                    )}
                                                </div>
                                            </label>
                                        ))}
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                )}
            </div>
        </div>
    );
}