'use client'

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import {
    faCalendarCheck,
    faCircleCheck,
    faStar,
    faShieldHalved,
} from "@fortawesome/free-solid-svg-icons"
import AuroraSection from "@/src/components/common/AuroraSection"

// ---------- Fake data (thay bằng dữ liệu thật sau) ----------
const HERO = {
    title: "Chăm sóc sức khỏe toàn diện cho cả gia đình bạn",
    content:
        "Đội ngũ bác sĩ giàu kinh nghiệm, trang thiết bị hiện đại, quy trình thăm khám nhanh chóng. Đặt lịch ngay hôm nay để được tư vấn miễn phí.",
    ctaLabel: "Đặt lịch khám",
    ctaHref: "/patient/appointments",
    image: "/hero-banners/examine-patients.png",
    imageAlt: "Bác sĩ Base Clinic đang tư vấn cho bệnh nhân",
    highlights: [
        "Bác sĩ chuyên khoa đầu ngành",
        "Không chờ đợi — đặt lịch trước",
        "Bảo mật thông tin tuyệt đối",
    ],
    stats: [
        { value: "15+", label: "Năm kinh nghiệm" },
        { value: "50k+", label: "Bệnh nhân tin tưởng" },
        { value: "4.9", label: "Đánh giá trung bình" },
    ],
}

export default function PatientHeroBanner() {
    return (
        <AuroraSection>
            <div className="container-app relative grid items-center gap-10 py-12 lg:grid-cols-2 lg:gap-16 lg:py-20">
                {/* ---------- LEFT: Content ---------- */}
                <div className="order-2 lg:order-1">
                    {/* Trust badge */}
                    <div className="mb-5 inline-flex items-center gap-2 rounded-full bg-white/70 px-3 py-1.5 text-xs font-medium text-primary shadow-sm ring-1 ring-primary/20 backdrop-blur">
                        <FontAwesomeIcon icon={faShieldHalved} className="h-3.5 w-3.5" />
                        <span>Phòng khám đạt chuẩn Bộ Y tế</span>
                    </div>

                    {/* Title */}
                    <h1 className="mb-4 text-3xl font-bold leading-tight text-gray-900 sm:text-4xl lg:text-5xl">
                        {HERO.title}
                    </h1>

                    {/* Short content */}
                    <p className="mb-6 max-w-xl text-base leading-relaxed text-gray-600 sm:text-lg">
                        {HERO.content}
                    </p>

                    {/* Highlights */}
                    <ul className="mb-7 space-y-2.5">
                        {HERO.highlights.map((item) => (
                            <li key={item} className="flex items-center gap-2.5 text-sm text-gray-700 sm:text-base">
                                <FontAwesomeIcon
                                    icon={faCircleCheck}
                                    className="h-4 w-4 shrink-0 text-secondary"
                                />
                                <span>{item}</span>
                            </li>
                        ))}
                    </ul>

                    {/* CTA + secondary info */}
                    <div className="flex flex-wrap items-center gap-4">
                        <a
                            href={HERO.ctaHref}
                            className="btn bg-primary px-6 py-3 text-base text-white shadow-lg shadow-primary/30 transition-transform hover:bg-primary-dark hover:-translate-y-0.5"
                        >
                            <FontAwesomeIcon icon={faCalendarCheck} className="h-4 w-4" />
                            {HERO.ctaLabel}
                        </a>

                        <div className="flex items-center gap-2 text-sm text-gray-600">
                            <div className="flex text-amber-400">
                                {Array.from({ length: 5 }).map((_, i) => (
                                    <FontAwesomeIcon key={i} icon={faStar} className="h-3.5 w-3.5" />
                                ))}
                            </div>
                            <span>4.9/5 từ 2.000+ bệnh nhân</span>
                        </div>
                    </div>

                    {/* Stats */}
                    <div className="mt-9 grid grid-cols-3 gap-4 border-t border-gray-200/70 pt-6">
                        {HERO.stats.map((s) => (
                            <div key={s.label}>
                                <div className="text-xl font-bold text-primary sm:text-2xl">
                                    {s.value}
                                </div>
                                <div className="text-xs text-gray-500 sm:text-sm">
                                    {s.label}
                                </div>
                            </div>
                        ))}
                    </div>
                </div>

                {/* ---------- RIGHT: Image ---------- */}
                <div className="relative order-1 lg:order-2">
                    {/* Decorative frame */}
                    <div className="absolute inset-0 -rotate-3 rounded-3xl bg-secondary/20" />
                    <div className="absolute inset-0 rotate-3 rounded-3xl bg-primary/20" />

                    {/* Image container */}
                    <div className="relative overflow-hidden rounded-3xl shadow-2xl">
                        <img
                            src={HERO.image}
                            alt={HERO.imageAlt}
                            className="aspect-[4/5] w-full object-cover sm:aspect-[5/5] lg:aspect-[4/5]"
                        />

                        {/* Floating badge — bottom left */}
                        <div className="absolute bottom-4 left-4 flex items-center gap-2 rounded-xl bg-white/95 px-3 py-2 shadow-lg backdrop-blur">
                            <div className="flex h-9 w-9 items-center justify-center rounded-full bg-secondary/15 text-secondary">
                                <FontAwesomeIcon icon={faShieldHalved} className="h-4 w-4" />
                            </div>
                            <div className="leading-tight">
                                <div className="text-xs font-semibold text-gray-900">
                                    Vô trùng 100%
                                </div>
                                <div className="text-[11px] text-gray-500">
                                    Trang thiết bị y tế
                                </div>
                            </div>
                        </div>

                        {/* Floating badge — top right */}
                        <div className="absolute right-4 top-4 flex items-center gap-2 rounded-xl bg-white/95 px-3 py-2 shadow-lg backdrop-blur">
                            <div className="flex text-amber-400">
                                <FontAwesomeIcon icon={faStar} className="h-3 w-3" />
                            </div>
                            <div className="leading-tight">
                                <div className="text-xs font-semibold text-gray-900">
                                    Top 10
                                </div>
                                <div className="text-[11px] text-gray-500">
                                    Phòng khám uy tín
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </AuroraSection>
    )
}