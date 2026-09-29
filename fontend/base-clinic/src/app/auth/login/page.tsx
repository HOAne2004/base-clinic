'use client'

import { LoginForm } from "@/src/features/auth/components/LoginForm";
import { useState } from "react";

export default function LoginPage() {
    const [showLoginModal, setShowLoginModal] = useState(false);

    return (
        <main className="mx-auto max-w-md px-4 py-10">
            <button
            onClick={() => setShowLoginModal(true)}
             className="mb-6 text-2xl font-bold">
                Đăng nhập
            </button>

            <LoginForm 
            show = {showLoginModal}
            onHide={() => setShowLoginModal(false)}
            />
        </main>
    );
}