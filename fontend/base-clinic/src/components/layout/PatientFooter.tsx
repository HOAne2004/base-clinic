'use client'

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import {
    faLocationDot,
    faPhone,
    faEnvelope,
    faClock,
} from "@fortawesome/free-solid-svg-icons"
import {
    faFacebook,
    faYoutube,
    faLinkedinIn,
} from "@fortawesome/free-brands-svg-icons"
// Fake data — sẽ thay bằng dữ liệu thật sau
const CLINIC = {
    name: "Base Clinic",
    description:
        "Phòng khám đa khoa với đội ngũ bác sĩ giàu kinh nghiệm, mang đến dịch vụ chăm sóc sức khỏe tận tâm và chất lượng cao.",
    address: "123 Nguyễn Văn Cừ, Phường 4, Quận 5, TP. Hồ Chí Minh",
    hotline: "1900.0000",
    email: "contact@baseclinic.vn",
    workingHours: [
        { day: "Thứ 2 – Thứ 6", time: "07:00 – 20:00" },
        { day: "Thứ 7", time: "07:00 – 17:00" },
        { day: "Chủ nhật", time: "07:00 – 12:00" },
    ],
}

const QUICK_LINKS = [
    { label: "Trang chủ", href: "/" },
    { label: "Giới thiệu", href: "/about" },
    { label: "Đội ngũ bác sĩ", href: "/doctors" },
    { label: "Chuyên khoa", href: "/specialties" },
    { label: "Bảng giá dịch vụ", href: "/pricing" },
]

const SUPPORT_LINKS = [
    { label: "Đặt lịch khám", href: "/booking" },
    { label: "Tra cứu kết quả", href: "/results" },
    { label: "Câu hỏi thường gặp", href: "/faq" },
    { label: "Chính sách bảo mật", href: "/privacy" },
    { label: "Điều khoản sử dụng", href: "/terms" },
]

const SOCIALS = [
    { icon: faFacebook, href: "#", label: "Facebook" },
    { icon: faYoutube, href: "#", label: "YouTube" },
    { icon: faLinkedinIn, href: "#", label: "LinkedIn" },
]

export default function PatientFooter() {
    return (
        <footer className="bg-secondary text-white">
            {/* ---------- Main ---------- */}
            <div className="container-app grid gap-8 py-10 sm:grid-cols-2 lg:grid-cols-4 lg:gap-10 lg:py-14">
                {/* Col 1: Brand + description + socials */}
                <div className="sm:col-span-2 lg:col-span-1">
                    <div className="mb-3 flex items-center gap-2">
                        <img
                            src="/favicon.ico"
                            alt={CLINIC.name}
                            className="h-8 w-8"
                        />
                        <h3 className="text-xl font-bold">{CLINIC.name}</h3>
                    </div>

                    <p className="mb-5 text-sm leading-relaxed text-white/80">
                        {CLINIC.description}
                    </p>

                    <div className="flex gap-3">
                        {SOCIALS.map((s) => (
                            <a
                                key={s.label}
                                href={s.href}
                                aria-label={s.label}
                                className="flex h-9 w-9 items-center justify-center rounded-full bg-white/10 text-white transition-colors hover:bg-white hover:text-secondary"
                            >
                                <FontAwesomeIcon icon={s.icon} className="h-4 w-4" />
                            </a>
                        ))}
                    </div>
                </div>

                {/* Col 2: Quick links */}
                <div>
                    <h4 className="mb-4 text-base font-semibold uppercase tracking-wide">
                        Về chúng tôi
                    </h4>
                    <ul className="space-y-2 text-sm text-white/80">
                        {QUICK_LINKS.map((link) => (
                            <li key={link.label}>
                                <a
                                    href={link.href}
                                    className="inline-block transition-colors hover:text-white hover:underline"
                                >
                                    {link.label}
                                </a>
                            </li>
                        ))}
                    </ul>
                </div>

                {/* Col 3: Support links */}
                <div>
                    <h4 className="mb-4 text-base font-semibold uppercase tracking-wide">
                        Hỗ trợ khách hàng
                    </h4>
                    <ul className="space-y-2 text-sm text-white/80">
                        {SUPPORT_LINKS.map((link) => (
                            <li key={link.label}>
                                <a
                                    href={link.href}
                                    className="inline-block transition-colors hover:text-white hover:underline"
                                >
                                    {link.label}
                                </a>
                            </li>
                        ))}
                    </ul>
                </div>

                {/* Col 4: Contact + hours */}
                <div className="sm:col-span-2 lg:col-span-1">
                    <h4 className="mb-4 text-base font-semibold uppercase tracking-wide">
                        Liên hệ
                    </h4>

                    <ul className="space-y-3 text-sm text-white/80">
                        <li className="flex items-start gap-3">
                            <FontAwesomeIcon
                                icon={faLocationDot}
                                className="mt-0.5 h-4 w-4 shrink-0 text-white"
                            />
                            <span>{CLINIC.address}</span>
                        </li>
                        <li className="flex items-start gap-3">
                            <FontAwesomeIcon
                                icon={faPhone}
                                className="mt-0.5 h-4 w-4 shrink-0 text-white"
                            />
                            <a
                                href={`tel:${CLINIC.hotline.replace(/\./g, "")}`}
                                className="hover:text-white hover:underline"
                            >
                                Hotline: {CLINIC.hotline}
                            </a>
                        </li>
                        <li className="flex items-start gap-3">
                            <FontAwesomeIcon
                                icon={faEnvelope}
                                className="mt-0.5 h-4 w-4 shrink-0 text-white"
                            />
                            <a
                                href={`mailto:${CLINIC.email}`}
                                className="hover:text-white hover:underline"
                            >
                                {CLINIC.email}
                            </a>
                        </li>
                        <li className="flex items-start gap-3">
                            <FontAwesomeIcon
                                icon={faClock}
                                className="mt-0.5 h-4 w-4 shrink-0 text-white"
                            />
                            <div className="space-y-1">
                                {CLINIC.workingHours.map((h) => (
                                    <div key={h.day} className="flex gap-2">
                                        <span className="min-w-[100px] text-white/70">
                                            {h.day}:
                                        </span>
                                        <span>{h.time}</span>
                                    </div>
                                ))}
                            </div>
                        </li>
                    </ul>
                </div>
            </div>

            {/* ---------- Bottom bar ---------- */}
            <div className="border-t border-white/15">
                <div className="container-app flex flex-col items-center justify-between gap-2 py-4 text-xs text-white/70 sm:flex-row">
                    <span>
                        © {new Date().getFullYear()} {CLINIC.name}. All rights reserved.
                    </span>
                    <span>
                        Created by{" "}
                        <span className="font-semibold text-white">HOAne</span>
                    </span>
                </div>
            </div>
        </footer>
    )
}