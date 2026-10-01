export function formatPhone(phone: string): string {
    if (!phone) return "";

    const normalized = phone.replace(/\D/g, "");

    if (normalized.length !== 10) {
        return phone;
    }

    return normalized.replace(
        /(\d{4})(\d{3})(\d{3})/,
        "$1 $2 $3"
    );
}