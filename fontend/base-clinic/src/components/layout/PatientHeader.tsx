'use client'

import { useState } from "react"
import { LoginForm } from "@/src/features/auth/components/LoginForm";

export default function PatientHeader() {
    const [showLoginModal, setShowLoginModal] = useState(false);

    return (
        <header className="flex glass justify-between items-center p-2">
            <div className="flex justify-center items-center gap-2">
                <img className="w-8 h-auto" src="./favicon.ico"></img>
                <h1 className="font-bold  text-2xl ">Base Clinic</h1>
            </div>

            <span>Hotline: 1900.0000</span>
            <button
                onClick={() => setShowLoginModal(true)}
                className="mb-6 text-2xl font-bold"
            >Đăng nhập</button>
            <LoginForm
                show={showLoginModal}
                onHide={() => setShowLoginModal(false)} />
        </header>
    )
}