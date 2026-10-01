export function formatCurrency(
    value: number,
    currency = "VND"
): string {
    return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency,
    }).format(value);
}