export interface CreateDoctorRequest {
    fullName: string;
    phoneNumber: string;
    email?: string;
    password: string;
    departmentId: string;
    avatar?: string;
    fullAddress?: string;
    gender?: boolean;
    dateOfBirth?: string;
    identityNumber?: string;
    licenseNumber?: string;
    consultationFee?: number;
    description?: string;
}

export interface UpdateDoctorRequest {
    avatar?: string;
    fullAddress?: string;
    gender?: boolean;
    dateOfBirth?: string;
    identityNumber?: string;
    licenseNumber?: string;
    consultationFee?: number;
    description?: string;
    departmentId?: string;
}