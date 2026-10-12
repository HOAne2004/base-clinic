import {
    faGaugeHigh,
    faUserDoctor,
    faCalendarCheck,
    faHospital,
    faShieldHalved,
    faGear,
} from "@fortawesome/free-solid-svg-icons"
import { IconDefinition } from "@fortawesome/fontawesome-svg-core"

export type AdminNavItem = {
    label: string
    href: string
    icon: IconDefinition
    /** Nếu có children → là nhóm có thể mở rộng */
    children?: { label: string; href: string }[]
    /** Danh sách role được phép thấy mục này. Bỏ trống = mọi role */
    roles?: string[]
}

export const ADMIN_NAV: AdminNavItem[] = [
    {
        label: "Dashboard",
        href: "/admin",
        icon: faGaugeHigh,
    },
    {
        label: "Quản lý Bác sĩ",
        href: "/admin/doctors",
        icon: faUserDoctor,
    },
    {
        label: "Quản lý Lịch khám",
        href: "/admin/appointments",
        icon: faCalendarCheck,
    },
    {
        label: "Quản lý Khoa",
        href: "/admin/departments",
        icon: faHospital,
    },
    {
        label: "Quản lý Phân quyền",
        href: "/admin/roles",
        icon: faShieldHalved,
        // roles: ["admin"],   // bật khi tích hợp RBAC
    },
    {
        label: "Cài đặt",
        href: "/admin/settings",
        icon: faGear,
        children: [
            { label: "Thông tin phòng khám", href: "/admin/settings/clinic" },
            { label: "Tài khoản", href: "/admin/settings/account" },
            { label: "Thông báo", href: "/admin/settings/notifications" },
        ],
    },
]