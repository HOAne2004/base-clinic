// Map tương đương với AccountPublicInfoDto của Backend
export interface AccountPublicInfoResponse {
    id: string; // Guid trong C# khi sang JSON/TS sẽ biến thành string
    fullName: string;
    email: string | null;
    phoneNumber: string;
    status: AccountStatus;
    isEmailVerified: boolean;
    isPhoneNumberVerified: boolean;
    roles: string[];
}

// Map tương đương với AuthResultDto của Backend
export interface LoginResponse {
    accessToken: string;
    user: AccountPublicInfoResponse;
}