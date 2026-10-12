'use client'

import { useState } from "react"
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import {
    faBars,
    faBell,
    faMagnifyingGlass,
    faChevronDown,
    faRightFromBracket,
    faUser,
} from "@fortawesome/free-solid-svg-icons"
import { toast } from "react-toastify"

import { useAuth } from "@/src/contexts/AuthContext"

type Props = {
    onOpenSidebar: () => void
    /** Sau này truyền user thật vào */
    user?: { name: string; role: string }
}

export default function AdminHeader({
    onOpenSidebar,
    user = { name: "Nguyễn Văn A", role: "Quản trị viên" },
}: Props) {
    const [userMenuOpen, setUserMenuOpen] = useState(false)
    const { logout } = useAuth()

    const handleLogout = () => {
        setUserMenuOpen(false)
        logout()
        toast.success("Đăng xuất thành công.")
    }

    return (
        <header className="sticky top-0 z-20 flex h-14 items-center gap-3 border-b border-border bg-surface px-3 sm:px-4">
            {/* Hamburger — chỉ mobile */}
            <button
                onClick={onOpenSidebar}
                aria-label="Mở menu"
                className="flex h-9 w-9 items-center justify-center rounded-lg text-text-muted hover:bg-surface-muted lg:hidden"
            >
                <FontAwesomeIcon icon={faBars} className="h-4 w-4" />
            </button>

            {/* Search — ẩn trên mobile rất nhỏ */}
            <div className="relative hidden flex-1 max-w-md sm:block">
                <FontAwesomeIcon
                    icon={faMagnifyingGlass}
                    className="pointer-events-none absolute left-3 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-text-light"
                />
                <input
                    type="search"
                    placeholder="Tìm bệnh nhân, lịch hẹn..."
                    className="input-base pl-9"
                />
            </div>

            <div className="ml-auto flex items-center gap-1 sm:gap-2">
                {/* Search icon — chỉ mobile nhỏ */}
                <button
                    aria-label="Tìm kiếm"
                    className="flex h-9 w-9 items-center justify-center rounded-lg text-text-muted hover:bg-surface-muted sm:hidden"
                >
                    <FontAwesomeIcon icon={faMagnifyingGlass} className="h-4 w-4" />
                </button>

                {/* Notifications */}
                <button
                    aria-label="Thông báo"
                    className="relative flex h-9 w-9 items-center justify-center rounded-lg text-text-muted hover:bg-surface-muted"
                >
                    <FontAwesomeIcon icon={faBell} className="h-4 w-4" />
                    <span className="absolute right-1.5 top-1.5 h-2 w-2 rounded-full bg-error" />
                </button>

                {/* User menu */}
                <div className="relative">
                    <button
                        onClick={() => setUserMenuOpen((v) => !v)}
                        className="flex items-center gap-2 rounded-lg px-2 py-1.5 hover:bg-surface-muted"
                    >
                        <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary/15 text-sm font-semibold text-primary">
                            {user.name.charAt(0)}
                        </div>
                        <div className="hidden text-left leading-tight sm:block">
                            <div className="text-sm font-semibold text-text">
                                {user.name}
                            </div>
                            <div className="text-xs text-text-muted">{user.role}</div>
                        </div>
                        <FontAwesomeIcon
                            icon={faChevronDown}
                            className="hidden h-3 w-3 text-text-muted sm:block"
                        />
                    </button>

                    {userMenuOpen && (
                        <>
                            {/* Click ra ngoài để đóng */}
                            <div
                                className="fixed inset-0 z-10"
                                onClick={() => setUserMenuOpen(false)}
                                aria-hidden="true"
                            />
                            <div className="absolute right-0 top-full z-20 mt-2 w-52 overflow-hidden rounded-lg border border-border bg-surface shadow-lg">
                                <button className="flex w-full items-center gap-3 px-4 py-2.5 text-sm text-text hover:bg-surface-muted">
                                    <FontAwesomeIcon icon={faUser} className="h-3.5 w-3.5" />
                                    Thông tin cá nhân
                                </button>
                                <button
                                    type="button"
                                    onClick={handleLogout}
                                    className="flex w-full items-center gap-3 px-4 py-2.5 text-sm text-error hover:bg-error/10"
                                >
                                    <FontAwesomeIcon
                                        icon={faRightFromBracket}
                                        className="h-3.5 w-3.5"
                                    />
                                    Đăng xuất
                                </button>
                            </div>
                        </>
                    )}
                </div>
            </div>
        </header>
    )
}