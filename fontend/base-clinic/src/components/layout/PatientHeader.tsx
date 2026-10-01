'use client'

import { useState } from "react";

import { LoginForm } from "@/src/features/auth/components/LoginForm";
import Modal from "@/src/components/ui/Modal";
import Dropdown from "@/src/components/ui/Dropdown";

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { faUserCircle, faClockRotateLeft, faSignOutAlt, faChevronDown } from "@fortawesome/free-solid-svg-icons"

import { useAuth } from "@/src/contexts/AuthContext";
import { useRouter } from "next/dist/client/components/navigation";
import { toast } from "react-toastify/unstyled";

export default function PatientHeader() {
    const [showLoginModal, setShowLoginModal] = useState(false);
    const { user, logout, isLoading } = useAuth();
    const router = useRouter();

    const handleLogout = () => {
        logout();
        toast.success("Đăng xuất thành công.");
        router.push("/"); // Chuyển hướng về trang chủ sau khi đăng xuất
    }
    return (
        <header className="glass sticky top-0 z-40 w-full">
            <div className="flex items-center justify-between gap-2 px-8 py-2">

                {/* Brand: logo + tên */}
                <button onClick={() => router.push("/")} className="flex min-w-0 items-center gap-2 cursor-pointer">
                    <img
                        src="/favicon.ico"
                        alt="Base Clinic"
                        className="h-7 w-7 shrink-0 sm:h-8 sm:w-8"
                    />
                    <h1 className="truncate font-bold text-base sm:text-xl lg:text-2xl">
                        Base Clinic
                    </h1>
                </button>

                {/* Hotline — ẩn trên mobile nhỏ, hiện từ sm trở lên */}
                <span className="hidden whitespace-nowrap text-sm text-text-muted sm:inline lg:text-base">
                    Hotline: <strong className="text-text">1900.0000</strong>
                </span>

                {/* Nút đăng nhập */}
                {isLoading ? (
                    <div className="h-10 w-24 animate-pulse rounded-md bg-gray-200"></div>
                ) : user ? (
                    // UI KHI ĐÃ ĐĂNG NHẬP (Dùng Dropdown Component)
                    <Dropdown
                        trigger={
                            <div className="flex items-center gap-2 rounded-lg bg-surface-muted px-3 py-2 text-sm font-medium text-text hover:bg-border-muted transition-colors border border-border">
                                <FontAwesomeIcon icon={faUserCircle} className="text-primary h-5 w-5" />
                                <span className="hidden sm:inline-block max-w-150px truncate">
                                    {user.fullName} {/* Lấy thẳng fullName từ DTO */}
                                </span>
                                <FontAwesomeIcon icon={faChevronDown} className="text-xs" />
                            </div>
                        }
                    >
                        <div className="px-4 py-2 border-b border-border sm:hidden">
                            <p className="text-sm font-semibold text-text truncate">{user.fullName}</p>
                        </div>

                        <button
                            onClick={() => router.push('/profile')}
                            className="flex w-full items-center gap-3 px-4 py-2 text-sm text-text hover:bg-surface-muted transition-colors"
                        >
                            <FontAwesomeIcon icon={faUserCircle} className="text-primary w-4" />
                            Thông tin cá nhân
                        </button>

                        <button
                            onClick={() => router.push('/history')}
                            className="flex w-full items-center gap-3 px-4 py-2 text-sm text-text hover:bg-surface-muted transition-colors"
                        >
                            <FontAwesomeIcon icon={faClockRotateLeft} className="text-secondary w-4" />
                            Lịch sử khám
                        </button>

                        <div className="my-1 border-t border-border"></div>

                        <button
                            onClick={handleLogout} // Gọi hàm logout
                            className="flex w-full items-center gap-3 px-4 py-2 text-sm text-error hover:bg-red-50 transition-colors"
                        >
                            <FontAwesomeIcon icon={faSignOutAlt} className="w-4" />
                            Đăng xuất
                        </button>
                    </Dropdown>
                ) : (
                    // UI KHI CHƯA ĐĂNG NHẬP
                    <button
                        onClick={() => setShowLoginModal(true)}
                        className="btn shrink-0 bg-primary px-3 py-2 text-sm text-white hover:bg-primary-dark sm:px-4 sm:text-base"
                    >
                        Đăng nhập
                    </button>
                )}
            </div>
            <Modal
                show={showLoginModal}
                onHide={() => setShowLoginModal(false)}
                title="Đăng nhập"
            >
                <LoginForm onSuccess={() => setShowLoginModal(false)} />
            </Modal>
        </header>
    )
}