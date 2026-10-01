'use client'

import { useState } from "react"
import { LoginForm } from "@/src/features/auth/components/LoginForm"
import Modal from "@/src/components/ui/Modal"

export default function PatientHeader() {
    const [showLoginModal, setShowLoginModal] = useState(false)

    return (
        <header className="glass sticky top-0 z-40 w-full">
            <div className="flex items-center justify-between gap-2 px-8 py-2">

                {/* Brand: logo + tên */}
                <div className="flex min-w-0 items-center gap-2">
                    <img
                        src="/favicon.ico"
                        alt="Base Clinic"
                        className="h-7 w-7 shrink-0 sm:h-8 sm:w-8"
                    />
                    <h1 className="truncate font-bold text-base sm:text-xl lg:text-2xl">
                        Base Clinic
                    </h1>
                </div>

                {/* Hotline — ẩn trên mobile nhỏ, hiện từ sm trở lên */}
                <span className="hidden whitespace-nowrap text-sm text-text-muted sm:inline lg:text-base">
                    Hotline: <strong className="text-text">1900.0000</strong>
                </span>

                {/* Nút đăng nhập */}
                <button
                    onClick={() => setShowLoginModal(true)}
                    className="btn shrink-0 bg-primary px-3 py-2 text-sm text-white hover:bg-primary-dark sm:px-4 sm:text-base"
                >
                    Đăng nhập
                </button>
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