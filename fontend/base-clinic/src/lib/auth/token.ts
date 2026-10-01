// Định nghĩa kiểu dữ liệu cho User (Dựa theo AccountPublicInfoDto ở BE)
export interface UserProfile {
    id: string;
    fullName: string;
    email: string | null;
    phoneNumber: string;
    status: number;
    isEmailVerified: boolean;
    isPhoneNumberVerified: boolean;
    roles: string[];
}

export const TokenService = {
    // 1. Quản lý Access Token
    getToken: () => {
        if (typeof window === "undefined") return null; // Tránh lỗi SSR trên Next.js
        return localStorage.getItem("accessToken");
    },
    setToken: (token: string) => localStorage.setItem("accessToken", token),
    removeToken: () => localStorage.removeItem("accessToken"),

    // 2. Quản lý thông tin User
    getUser: (): UserProfile | null => {
        if (typeof window === "undefined") return null;
        const userStr = localStorage.getItem("user");
        return userStr ? JSON.parse(userStr) : null;
    },
    setUser: (user: UserProfile) => localStorage.setItem("user", JSON.stringify(user)),
    removeUser: () => localStorage.removeItem("user"),

    // 3. Quản lý Refresh Token
    getRefreshToken: () => {
        if (typeof window === "undefined") return null;
        return localStorage.getItem("refreshToken");
    },
    setRefreshToken: (token: string) => localStorage.setItem("refreshToken", token),
    removeRefreshToken: () => localStorage.removeItem("refreshToken"),

    // 4. Tạo hàm xóa sạch mọi thứ cho tiện
    clearAll: () => {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("user");
    }
};