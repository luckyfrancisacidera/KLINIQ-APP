import axios from "axios";
import type { AccountRequestDto, ApprovePayload, RejectPayload, SubmitAccountRequestPayload } from "../types/accountRequest.types";

export const accountRequestApi = {
    submit : (payload : SubmitAccountRequestPayload) => {
        const form = new FormData();
        form.append("firstName", payload.firstName);
        form.append("lastName", payload.lastName);
        form.append("email", payload.email);
        form.append("licenseNumber", payload.licenseNumber);
        payload.specializations.forEach((spec) => form.append("specializations", spec));
        form.append("street", payload.street);
        form.append("city", payload.city);
        form.append("country", payload.country);
        form.append("clinicName", payload.clinicName);
        form.append("clinicLatitude", payload.clinicLatitude.toString());
        form.append("clinicLongitude", payload.clinicLongitude.toString());
        form.append("prcLicense", payload.prcLicense);
        form.append("governmentId", payload.governmentId);
        form.append("professionalPhoto", payload.professionalPhoto);
        form.append("cv", payload.cv);

        return axios.post<AccountRequestDto>("/account-requests/submit", form, { headers : {"Content-Type" : "multipart/form-data"}});
    },

    approve: (id: string, payload : ApprovePayload) => axios.post(`/account-requests/${id}/approve`, payload),

    reject: (id: string, payload : RejectPayload) => axios.post(`/account-requests/${id}/reject`, payload),
}