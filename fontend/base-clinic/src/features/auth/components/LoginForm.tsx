'use client'

import { FormEvent, useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { authApi } from "../api/auth.api";
import { toast } from 'react-toastify';

type ModalProps = {
    show: boolean,
    onHide: () => void,
    keyboard?: boolean,
    backdrop?: 'static' | true,
}

export function LoginForm({ show, onHide, keyboard = true, backdrop = true }: ModalProps) {
    // 1. GỌI TẤT CẢ HOOKS Ở TRÊN CÙNG
    const router = useRouter();
    const [phoneNumber, setPhoneNumber] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!show || !keyboard) return;
        const handleEscape = (e: KeyboardEvent) => {
            if (e.key === 'Escape') onHide();
        };
        document.addEventListener('keydown', handleEscape);
        return () => document.removeEventListener('keydown', handleEscape);
    }, [show, keyboard, onHide]);

    useEffect(() => {
        if (show) {
            document.body.style.overflow = 'hidden';
        } else {
            document.body.style.overflow = '';
        }
        return () => {
            document.body.style.overflow = '';
        };
    }, [show]);

    // 2. ĐẶT LỆNH RETURN SỚM Ở DƯỚI CÙNG (SAU KHI ĐÃ GỌI HẾT HOOKS)
    if (!show) return null;

    const handleBackdropClick = () => {
        if (backdrop === 'static') return;
        onHide();
    };

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
            localStorage.setItem("accessToken", response.token);
            toast.success("Đăng nhập thành công.");
            setPhoneNumber("");
            setPassword("");
            setError("");
            onHide();
            // Gọi hàm này để báo cho component cha tắt modal đi
            router.push("/");
        } catch (error) {
            setError(error instanceof Error ? error.message : "Đăng nhập thất bại.");
            toast.error("Có lỗi xảy ra, vui lòng thử lại.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div
            className="fixed inset-0 z-50 bg-black/50 p-4 flex justify-center items-center"
            onClick={handleBackdropClick}>

            {/* Thêm onClick={(e) => e.stopPropagation()} để click vào trong form không bị đóng modal */}
            <div className="bg-white p-6 rounded-lg w-full max-w-md" onClick={(e) => e.stopPropagation()}>
                <div className="flex justify-between items-center mb-4">
                    <h2 className="font-bold text-xl">Đăng nhập</h2>
                    <button className="btn text-4xl leading-none" onClick={onHide}>&times;</button>
                </div>

                <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            Số điện thoại
                        </label>
                        <input
                            value={phoneNumber}
                            onChange={(e) => setPhoneNumber(e.target.value)} // Fix: Phải có onChange thì mới gõ được chữ
                            type="text" 
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            placeholder="098..."
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            Mật khẩu
                        </label>
                        <input
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            type="password"
                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                            placeholder="Nhập mật khẩu"
                        />
                    </div>

                    {error && <p className="text-red-500 text-sm">{error}</p>}

                    <button
                        disabled={loading}
                        className="btn bg-blue-600 hover:bg-blue-700 text-white disabled:opacity-50 disabled:cursor-not-allowed py-2 rounded-md"
                        type='submit'>
                        {loading ? 'Đang gửi...' : 'Đăng nhập'}
                    </button>
                </form>
            </div>
        </div>
    )
}