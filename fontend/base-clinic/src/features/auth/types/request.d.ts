export interface RegisterRequest{
    fullName : string;
    phoneNumber: string
    password: string;
}

export interface LoginRequest{
    phoneNumber: string;
    password: string;
}