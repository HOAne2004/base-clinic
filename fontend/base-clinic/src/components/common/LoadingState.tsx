import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";

type LoadingProps = {
    text?: string;        // Chữ hiển thị (Đang tải, Đang gửi...)
    fullScreen?: boolean; // Có muốn phủ toàn màn hình không?
};

export default function LoadingState({ text, fullScreen }: LoadingProps) {
    const content = (
        <div className="flex flex-col items-center justify-center gap-2 text-primary">
            <FontAwesomeIcon icon={faSpinner} spin className="h-8 w-8 sm:h-10 sm:w-10" />
            <span className="text-sm font-medium sm:text-base animate-pulse">{text}</span>
        </div>
    );

    // Nếu cần che toàn bộ màn hình (Dùng cho ProtectedRoute hoặc Load trang)
    if (fullScreen) {
        return (
            <div className="fixed inset-0 z-50 flex h-screen w-full items-center justify-center bg-background/80 backdrop-blur-sm">
                {content}
            </div>
        );
    }

    // Nếu chỉ cần loading trong một khu vực nhỏ (Dùng cho Form hoặc Bảng dữ liệu)
    return (
        <div className="flex w-full items-center justify-center py-8">
            {content}
        </div>
    );
}