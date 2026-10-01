import { env } from "@/src/config/env";
import { TokenService } from "@/src/lib/auth/token";

// Biến cờ đánh dấu hệ thống có đang trong quá trình lấy Token mới hay không
let isRefreshing = false;
// Hàng đợi chứa các Request bị fail do 401 trong lúc chờ Token mới
let failedQueue: Array<{ resolve: (token: string) => void, reject: (error: any) => void }> = [];

const processQueue = (error: any, token: string | null = null) => {
    failedQueue.forEach(prom => {
        if (error) { prom.reject(error); }
        else { prom.resolve(token!); }
    });
    failedQueue = [];
};

export async function apiClient<T>(
    endpoint: string,
    options?: RequestInit
): Promise<T> {
    const token = TokenService.getToken(); // Lấy token từ TokenService

    const headers = new Headers(options?.headers);
    headers.set('Content-Type', 'application/json');
    if (token) headers.set('Authorization', `Bearer ${token}`);

    let response = await fetch(`${env.apiUrl}/${endpoint}`, {
        ...options,
        headers,
    });

    // BẮT LỖI 401: XỬ LÝ REFRESH TOKEN
    if (response.status === 401) {
        const refreshToken = TokenService.getRefreshToken();

        // Nếu không có Refresh Token để cứu vãn -> Đăng xuất luôn
        if (!refreshToken) {
            TokenService.clearAll();
            window.location.href = "/";
            throw new Error("Phiên đăng nhập hết hạn.");
        }

        // Nếu CHƯA có ai đi đổi Token, ta sẽ làm người đầu tiên (Khóa cờ lại)
        if (!isRefreshing) {
            isRefreshing = true;
            try {
                // Gọi API cấp lại Token của Backend (Giả định URL là /api/auth/refresh)
                const refreshRes = await fetch(`${env.apiUrl}/api/auth/refresh`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        accessToken: token,
                        refreshToken: refreshToken
                    })
                });

                if (!refreshRes.ok) throw new Error("Refresh failed");

                const refreshData = await refreshRes.json();

                // Lưu Token mới
                TokenService.setToken(refreshData.accessToken);
                if (refreshData.refreshToken) {
                    TokenService.setRefreshToken(refreshData.refreshToken);
                }

                // Phát Token mới cho những API đang xếp hàng
                processQueue(null, refreshData.accessToken);

                // Gọi lại chính Request gốc đang bị lỗi với Token mới
                headers.set('Authorization', `Bearer ${refreshData.accessToken}`);
                response = await fetch(`${env.apiUrl}/${endpoint}`, { ...options, headers });
            } catch (error) {
                // Refresh thất bại (VD: Refresh Token cũng hết hạn 7 ngày)
                processQueue(error, null);
                TokenService.clearAll();
                window.location.href = "/";
                throw new Error("Phiên đăng nhập đã hết hạn hoàn toàn. Vui lòng đăng nhập lại.");
            } finally {
                isRefreshing = false; // Mở khóa
            }
        } else {
            // Nếu CÓ người đang đi đổi Token rồi, ta chỉ việc xếp hàng chờ
            return new Promise((resolve, reject) => {
                failedQueue.push({
                    resolve: (newToken: string) => {
                        headers.set('Authorization', `Bearer ${newToken}`);
                        // Sau khi nhận Token mới từ người đi đổi, tự gọi lại Request gốc
                        fetch(`${env.apiUrl}/${endpoint}`, { ...options, headers })
                            .then(res => res.json())
                            .then(resolve)
                            .catch(reject);
                    },
                    reject: (err: any) => reject(err)
                });
            });
        }
    }

    // Xử lý các lỗi khác ngoài 401
    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || "Đã xảy ra lỗi kết nối với máy chủ.");
    }

    return response.json();
}