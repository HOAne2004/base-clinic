export interface DoctorListDto {
    id: string;
    accountId: string;
    doctorCode: string;
    fullName: string;
    phoneNumber: string;
    email: string | null;
    departmentName: string;
    accountStatus: string;
}

export interface DoctorAdminDetailDto {
    id: string;
    accountId: string;
    departmentId: string;
    doctorCode: string;
    fullName: string;
    phoneNumber: string;
    email: string | null;
    departmentName: string;
    accountStatus: string;
    lastLoginAt: string | null;
    avatar: string | null;
    fullAddress: string | null;
    gender: boolean | null;
    dateOfBirth: string | null;
    identityNumber: string | null;
    licenseNumber: string | null;
    consultationFee: number | null;
    description: string | null;
}