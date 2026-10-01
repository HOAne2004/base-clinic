'use client'

import { createContext, useContext, useState, useEffect, ReactNode } from "react";
import { useRouter } from "next/navigation";
import { jwtDecode } from "jwt-decode";
import { TokenService, UserProfile } from "@/src/lib/auth/token";

// 1. Định nghĩa khuôn mẫu cho "Tổng đài" Auth
interface AuthContextType {
    user: UserProfile | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (accessToken: string, refreshToken: string, user: UserProfile) => void;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// 2. Tạo Provider (Nhà cung cấp dữ liệu)
export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<UserProfile | null>(null);
    const [isLoading, setIsLoading] = useState(true); // Mặc định là đang tải khi mới vào trang
    const router = useRouter();

    // Chạy 1 lần duy nhất khi người dùng mở trang hoặc F5
    useEffect(() => {
        checkLoginStatus();
    }, []);

    const checkLoginStatus = () => {
        const token = TokenService.getToken();
        const savedUser = TokenService.getUser();

        if (token && savedUser) {
            try {
                // Giải mã token CHỈ để lấy thời gian hết hạn (exp)
                const decoded: any = jwtDecode(token);
                const currentTime = Date.now() / 1000; // Đổi ra giây cho cùng chuẩn với JWT

                if (decoded.exp < currentTime) {
                    // Token đã hết hạn
                    console.log("Token expired. Logging out...");
                    logout();
                } else {
                    // Token còn hạn -> Khôi phục user vào State
                    setUser(savedUser);
                }
            } catch (error) {
                // Token bị hỏng hoặc sai định dạng
                logout();
            }
        } else {
            // Không có token trong máy
            setUser(null);
        }

        // Quá trình kiểm tra hoàn tất
        setIsLoading(false);
    };

    // Hàm gọi khi đăng nhập thành công từ LoginForm
    const login = (accessToken: string, refreshToken: string, userData: UserProfile) => {
        TokenService.setToken(accessToken);
        TokenService.setRefreshToken(refreshToken);
        TokenService.setUser(userData);
        setUser(userData);
    };

    // Hàm gọi khi người dùng bấm Đăng xuất
    const logout = () => {
        TokenService.clearAll();
        setUser(null);
        router.push("/");
    };

    return (
        <AuthContext.Provider value={{ user, isAuthenticated: !!user, isLoading, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

// 3. Tạo một Custom Hook để các component khác xài cho lẹ
export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error("useAuth phải được sử dụng bên trong AuthProvider");
    }
    return context;
};