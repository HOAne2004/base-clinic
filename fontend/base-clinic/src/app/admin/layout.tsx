'use client'

import { useState } from "react"
import AdminSidebar from "@/src/features/admin/components/AdminSidebar"
import AdminHeader from "@/src/features/admin/components/AdminHeader"

export default function AdminLayout({
    children,
}: {
    children: React.ReactNode
}) {
    const [mobileOpen, setMobileOpen] = useState(false)

    return (
        <div className="min-h-screen bg-background">
            <AdminSidebar
                mobileOpen={mobileOpen}
                onMobileClose={() => setMobileOpen(false)}
            />

            <div className="lg:pl-64">
                <AdminHeader onOpenSidebar={() => setMobileOpen(true)} />

                <main className="p-4 sm:p-6">{children}</main>
            </div>
        </div>
    )
}