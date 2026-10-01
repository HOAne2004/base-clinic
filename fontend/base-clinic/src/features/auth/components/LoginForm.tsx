'use client'

import { FormEvent, useState, useEffect } from "react";
import { useRouter } from "next/navigation";

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPhone, faLock, faSpinner, faEye, faEyeSlash } from "@fortawesome/free-solid-svg-icons";

import { authApi } from "../api/auth.api";
import { toast } from 'react-toastify';

import {formatPhone} from "@/src/lib/utils/format-phone";

import { useAuth } from "@/src/contexts/AuthContext";

type Props = {
    onSuccess?: () => void;
}

export function LoginForm({ onSuccess }: Props) {
    // 1. GỌI TẤT CẢ HOOKS Ở TRÊN CÙNG
    const router = useRouter();
    const { login } = useAuth(); // Lấy hàm login từ AuthContext
    const [phoneNumber, setPhoneNumber] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [showPassword, setShowPassword] = useState(false);

    // 2. GỌI CÁC HOOKS useEffect() Ở DƯỚI
    useEffect(() => {
        // Reset error khi người dùng gõ lại
        if (error) setError("");
    }, [phoneNumber, password]);
    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        setLoading(true);
        setError("");

        if (!phoneNumber.trim() || !password.trim()) {
            setError("Vui lòng nhập đầy đủ thông tin.");
            toast.warning("Vui lòng nhập đầy đủ thông tin.");
            setLoading(false); // Fix: Cần tắt loading nếu validate xịt
            return;
        }

        try {
            const response = await authApi.login({ phoneNumber, password });
            console.log("Result: ", response);
            login(response.accessToken, response.refreshToken, response.user); // Lưu token và user vào AuthContext
            toast.success("Đăng nhập thành công.");
            setPhoneNumber("");
            setPassword("");
            setError("");
            // Đóng modal và chuyển hướng về trang chủ
            onSuccess?.();
            router.push("/");
        } catch (error) {
            setError("Đăng nhập thất bại.");
            console.error("Error: ", error);
            toast.error("Có lỗi xảy ra, vui lòng thử lại.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            {/* Phone number */}
            <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">
                    Số điện thoại
                </label>
                <div className="relative">
                    <FontAwesomeIcon
                        icon={faPhone}
                        className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400"
                    />
                    <input
                        type="text"
                        value={formatPhone(phoneNumber)}
                        onChange={(e) => setPhoneNumber(e.target.value)}
                        placeholder="098..."
                        className="input-base pl-10 text-xl"
                    />
                </div>
            </div>

            {/* Password */}
            <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">
                    Mật khẩu
                </label>
                <div className="relative">
                    <FontAwesomeIcon
                        icon={faLock}
                        className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400"
                    />
                    <input
                        type={showPassword ? "text" : "password"}
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Nhập mật khẩu"
                        className="input-base pl-10 text-xl"
                    />
                    <button
                        type="button"
                        onClick={() => setShowPassword(!showPassword)}
                        className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                    >
                        {showPassword ? (
                            <FontAwesomeIcon icon={faEye} />
                        ) : (
                            <FontAwesomeIcon icon={faEyeSlash} />
                        )}
                    </button>
                </div>
            </div>

            {error && <p className="text-sm text-red-500">{error}</p>}

            <button
                type="submit"
                disabled={loading}
                className="btn bg-primary py-2 text-white hover:bg-primary-dark"
            >
                {loading ? (
                    <>
                        <FontAwesomeIcon icon={faSpinner} spin className="h-4 w-4" />
                        Đang gửi...
                    </>
                ) : (
                    "Đăng nhập"
                )}
            </button>
        </form>
    );
}