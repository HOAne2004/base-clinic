import { env } from "@/src/config/env";

export async function apiClient<T>(
    endpoint: string,
    options?: RequestInit
): Promise<T> {
    const token =
        typeof window !== "undefined"
            ? localStorage.getItem("accessToken")
            : null;

    const response = await fetch(`${env.apiUrl}/${endpoint}`, {
        ...options,
        headers: {
            'Content-Type': 'application/json',
            ...(token
                ? {
                    Authorization: `Bearer ${token}`,
                }
                : {}),
            ...options?.headers,
        },
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || "API request failed");
    }

    return response.json();
}