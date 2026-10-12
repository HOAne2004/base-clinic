'use client'

import { useEffect, useState } from "react"
import Link from "next/link"
import { usePathname } from "next/navigation"
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import {
    faChevronDown,
    faChevronLeft,
    faXmark,
} from "@fortawesome/free-solid-svg-icons"
import { ADMIN_NAV, AdminNavItem } from "../config/adminNav.config"

type Props = {
    /** Trạng thái mở drawer trên mobile, do AdminHeader điều khiển */
    mobileOpen: boolean
    onMobileClose: () => void
}

export default function AdminSidebar({ mobileOpen, onMobileClose }: Props) {
    const pathname = usePathname()

    // Desktop: collapsed (chỉ icon) hay expanded (icon + label)
    const [collapsed, setCollapsed] = useState(false)
    // Nhóm nào đang mở rộng (dùng label làm key)
    const [openGroups, setOpenGroups] = useState<Record<string, boolean>>({})

    // Tự mở nhóm chứa route hiện tại
    useEffect(() => {
        const next: Record<string, boolean> = {}
        for (const item of ADMIN_NAV) {
            if (item.children?.some((c) => pathname.startsWith(c.href))) {
                next[item.label] = true
            }
        }
        if (Object.keys(next).length) {
            setOpenGroups((prev) => ({ ...prev, ...next }))
        }
    }, [pathname])

    // Đóng drawer mobile mỗi khi đổi route
    useEffect(() => {
        onMobileClose()
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [pathname])

    const toggleGroup = (label: string) =>
        setOpenGroups((prev) => ({ ...prev, [label]: !prev[label] }))

    const isActive = (href: string) =>
        href === "/admin" ? pathname === "/admin" : pathname.startsWith(href)

    return (
        <>
            {/* Overlay mobile */}
            <div
                onClick={onMobileClose}
                aria-hidden="true"
                className={`fixed inset-0 z-30 bg-black/50 transition-opacity lg:hidden ${mobileOpen ? "opacity-100" : "pointer-events-none opacity-0"
                    }`}
            />

            {/* Sidebar */}
            <aside
                className={[
                    "fixed inset-y-0 left-0 z-40 flex flex-col border-r border-border bg-surface",
                    "transition-[transform,width] duration-300 ease-in-out",
                    // Desktop width tuỳ collapsed
                    collapsed ? "lg:w-[72px]" : "lg:w-64",
                    // Mobile: trượt vào/ra
                    mobileOpen ? "translate-x-0" : "-translate-x-full",
                    "lg:translate-x-0",
                    "w-64",
                ].join(" ")}
            >
                {/* Brand */}
                <div className="flex h-14 items-center justify-between border-b border-border px-4">
                    <Link href="/admin" className="flex min-w-0 items-center gap-2">
                        <img
                            src="/favicon.ico"
                            alt="Base Clinic"
                            className="h-7 w-7 shrink-0"
                        />
                        {!collapsed && (
                            <span className="truncate font-bold text-base">
                                Base Clinic
                            </span>
                        )}
                    </Link>

                    {/* Nút đóng trên mobile */}
                    <button
                        onClick={onMobileClose}
                        aria-label="Đóng menu"
                        className="flex h-8 w-8 items-center justify-center rounded-lg text-text-muted hover:bg-surface-muted lg:hidden"
                    >
                        <FontAwesomeIcon icon={faXmark} className="h-4 w-4" />
                    </button>
                </div>

                {/* Nav */}
                <nav className="flex-1 overflow-y-auto px-2 py-3">
                    <ul className="space-y-1">
                        {ADMIN_NAV.map((item) => (
                            <NavEntry
                                key={item.label}
                                item={item}
                                collapsed={collapsed}
                                isActive={isActive}
                                open={!!openGroups[item.label]}
                                onToggle={() => toggleGroup(item.label)}
                                pathname={pathname}
                            />
                        ))}
                    </ul>
                </nav>

                {/* Nút thu gọn — chỉ desktop */}
                <div className="hidden border-t border-border p-2 lg:block">
                    <button
                        onClick={() => setCollapsed((v) => !v)}
                        className="flex w-full items-center gap-3 rounded-lg px-3 py-2 text-sm text-text-muted transition-colors hover:bg-surface-muted hover:text-text"
                        aria-label={collapsed ? "Mở rộng menu" : "Thu gọn menu"}
                    >
                        <FontAwesomeIcon
                            icon={faChevronLeft}
                            className={`h-3.5 w-3.5 transition-transform ${collapsed ? "rotate-180" : ""
                                }`}
                        />
                        {!collapsed && <span>Thu gọn</span>}
                    </button>
                </div>
            </aside>
        </>
    )
}

/* ---------- NavEntry ---------- */

type NavEntryProps = {
    item: AdminNavItem
    collapsed: boolean
    isActive: (href: string) => boolean
    open: boolean
    onToggle: () => void
    pathname: string
}

function NavEntry({
    item,
    collapsed,
    isActive,
    open,
    onToggle,
    pathname,
}: NavEntryProps) {
    const active = isActive(item.href)
    const hasChildren = !!item.children?.length

    // ----- Có children: nhóm mở rộng -----
    if (hasChildren) {
        return (
            <li>
                <button
                    onClick={onToggle}
                    title={collapsed ? item.label : undefined}
                    className={[
                        "flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm transition-colors",
                        active
                            ? "bg-primary/10 font-semibold text-primary"
                            : "text-text-muted hover:bg-surface-muted hover:text-text",
                        collapsed ? "justify-center" : "",
                    ].join(" ")}
                >
                    <FontAwesomeIcon icon={item.icon} className="h-4 w-4 shrink-0" />
                    {!collapsed && (
                        <>
                            <span className="flex-1 truncate text-left">{item.label}</span>
                            <FontAwesomeIcon
                                icon={faChevronDown}
                                className={`h-3 w-3 transition-transform ${open ? "rotate-180" : ""
                                    }`}
                            />
                        </>
                    )}
                </button>

                {!collapsed && open && (
                    <ul className="mt-1 space-y-1 pl-9">
                        {item.children!.map((child) => {
                            const childActive = pathname === child.href
                            return (
                                <li key={child.href}>
                                    <Link
                                        href={child.href}
                                        className={[
                                            "block truncate rounded-lg px-3 py-2 text-sm transition-colors",
                                            childActive
                                                ? "bg-primary/10 font-semibold text-primary"
                                                : "text-text-muted hover:bg-surface-muted hover:text-text",
                                        ].join(" ")}
                                    >
                                        {child.label}
                                    </Link>
                                </li>
                            )
                        })}
                    </ul>
                )}
            </li>
        )
    }

    // ----- Không có children: link đơn -----
    return (
        <li>
            <Link
                href={item.href}
                title={collapsed ? item.label : undefined}
                className={[
                    "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm transition-colors",
                    active
                        ? "bg-primary/10 font-semibold text-primary"
                        : "text-text-muted hover:bg-surface-muted hover:text-text",
                    collapsed ? "justify-center" : "",
                ].join(" ")}
            >
                <FontAwesomeIcon icon={item.icon} className="h-4 w-4 shrink-0" />
                {!collapsed && <span className="truncate">{item.label}</span>}
            </Link>
        </li>
    )
}