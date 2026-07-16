export interface AccountRequestSummaryDto {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    specializations: string[];
    street: string;
    city: string;
    country: string;
    status: "Pending" | "Approved" | "Rejected";
    createdAtUtc: string;
}

export interface AccountRequestDto {
    id: string;
    firstName: string;
    lastName: string;
    email : string;
    licenseNumber : string;
    specializations : string[];
    street : string;
    city : string;
    country : string;
    clinicLatitude : number;
    clinicLongitude : number;

    prcLicensePath : string;
    governmentIdPath : string;
    professionalPhotoPath : string;
    cvPath : string;
    status : string;
    adminNote : string | null;
    isInvitationUsed : boolean;
    invitationExpiresAt : string | null;
    createdAtUtc : string;
}

export interface SubmitAccountRequestPayload {
    firstName: string;
    lastName: string;
    email : string;
    licenseNumber : string;
    specializations : string[];
    street : string;
    city : string;
    country : string;
    clinicName : string;
    clinicLatitude : number;
    clinicLongitude : number;
    prcLicense : File;
    governmentId : File;
    professionalPhoto : File;
    cv : File;
}

export interface ApprovePayload {
    notes? : string;
}

export interface RejectPayload {
    reason : string;
}

