import { useEffect, useState } from "react";
import { DepartmentLookupDto } from "../types/response";
import { departmentsApi } from "./departments.api";

export function useDepartmentLookup() {
    const [departments, setDepartments] = useState<DepartmentLookupDto[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        departmentsApi
            .getLookupList()
            .then(setDepartments)
            .catch((err) => console.error("Load departments failed:", err))
            .finally(() => setLoading(false));
    }, []);

    return { departments, loading };
}